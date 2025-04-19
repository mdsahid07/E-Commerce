using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EcommerceLambdaBackend.Models;
using EcommerceLambdaBackend.Services;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EcommerceLambdaBackend.Handlers;

public class SigninHandler
{
    private readonly UserService _userService;

    public SigninHandler()
    {
        var dynamoDb = new AmazonDynamoDBClient();
        _userService = new UserService(dynamoDb);
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Body))
                return CreateResponse(400, "Request body is missing");

            var loginRequest = JsonSerializer.Deserialize<LoginRequest>(request.Body);

            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                return CreateResponse(400, "Invalid input");
            

            var user = await _userService.GetUserByEmailAsync(loginRequest.Email);
            if (user == null)
                return CreateResponse(401, "Invalid credentials");

            var hashedInputPassword = UserService.HashPassword(loginRequest.Password);

            if (user.PasswordHash != hashedInputPassword)
                return CreateResponse(401, "Invalid credentials");

            var token = JwtService.GenerateToken(user);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { token }),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex.ToString());
            return CreateResponse(500, "Internal server error");
        }
    }

    private APIGatewayProxyResponse CreateResponse(int statusCode, string message)
    {
        return new APIGatewayProxyResponse
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

    private class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
