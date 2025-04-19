using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EcommerceLambdaBackend.Models;
using EcommerceLambdaBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Handlers
{
    public class SignupHandler
    {
        private readonly UserService _userService;

        public SignupHandler()
        {
            var dynamoDb = new AmazonDynamoDBClient();
            _userService = new UserService(dynamoDb);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("Incoming request...");
                context.Logger.LogInformation($"Raw Body: {request.Body}");

                if (string.IsNullOrEmpty(request.Body))
                    return CreateResponse(400, "Request body is missing");

                var userInput = JsonSerializer.Deserialize<UserInput>(request.Body);

                context.Logger.LogInformation($"Parsed UserInput: {JsonSerializer.Serialize(userInput)}");

                if (userInput is null || string.IsNullOrEmpty(userInput.Email) || string.IsNullOrEmpty(userInput.Password))
                    return CreateResponse(400, "Invalid input!!");

                if (await _userService.UserExistsAsync(userInput.Email))
                    return CreateResponse(409, "User already exists");

                var user = new User
                {
                    Username = userInput.Username,
                    Email = userInput.Email,
                    PasswordHash = UserService.HashPassword(userInput.Password)
                };

                await _userService.CreateUserAsync(user);
                return CreateResponse(200, "User registered successfully");
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex.ToString());
                return CreateResponse(500, ex.Message);
            }
        }

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

        private class UserInput
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

    }

}
