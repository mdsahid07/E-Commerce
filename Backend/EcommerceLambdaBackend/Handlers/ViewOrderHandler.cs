using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EcommerceLambdaBackend.Models;
using EcommerceLambdaBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Handlers
{
    public class ViewOrderHandler
    {
        private readonly AmazonDynamoDBClient _dynamoDb;
        private readonly DynamoDBContext _context;
        private readonly UserService _userService;

        public ViewOrderHandler()
        {
            _dynamoDb = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_dynamoDb);
            _userService = new UserService(_dynamoDb);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("Incoming request to view orders...");

                // Validate Authorization Header
                if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
                    return CreateResponse(401, "Authorization header missing");

                var token = authHeader.Replace("Bearer ", "");

                ClaimsPrincipal principal;
                try
                {
                    principal = JwtService.ValidateToken(token); // Validate the JWT token
                }
                catch (Exception)
                {
                    return CreateResponse(401, "Invalid or expired token");
                }

                var emailFromToken = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

                if (string.IsNullOrEmpty(emailFromToken))
                    return CreateResponse(403, "Email claim not found in token");

                var userFromDb = await _userService.GetUserByEmailAsync(emailFromToken);
                if (userFromDb == null)
                    return CreateResponse(404, "User not found");

                // Scan for all orders and filter by userId
                var allOrders = await _context.ScanAsync<Order>(new List<ScanCondition>()).GetRemainingAsync();
                var userOrders = allOrders.Where(o => o.Email == userFromDb.Email).ToList();

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(userOrders),
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" }
                    }
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error viewing orders: {ex.Message}");
                return CreateResponse(500, "Internal server error");
            }
        }

        // Helper method to create API response
        private APIGatewayProxyResponse CreateResponse(int statusCode, string message) =>
            new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = JsonSerializer.Serialize(new { message }),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };
    }
}
