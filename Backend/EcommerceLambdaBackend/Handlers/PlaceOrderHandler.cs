using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EcommerceLambdaBackend.Models;
using EcommerceLambdaBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Handlers
{
    public class PlaceOrderHandler
    {
        private readonly AmazonDynamoDBClient _dynamoDb;
        private readonly DynamoDBContext _context;
        private readonly UserService _userService;

        public PlaceOrderHandler()
        {
            _dynamoDb = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_dynamoDb);
            _userService = new UserService(_dynamoDb);
        }
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("Incoming request to place an order...");

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
                    return CreateResponse(401, "Invalid or expired token"); // Return error if token is invalid or expired
                }

                // Extract userId from token (you can use different claim names based on your JWT)
                var userIdFromToken = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Assuming `sub` or `nameidentifier` claim stores the userId
                if (string.IsNullOrEmpty(userIdFromToken))
                    return CreateResponse(403, "Invalid token payload");

                var username = principal.FindFirst("username")?.Value;
                var emailFromToken = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

                if (string.IsNullOrEmpty(emailFromToken))
                    return CreateResponse(403, "Email claim not found in token");
                var userIdFromDb = await _userService.GetUserByEmailAsync(emailFromToken);
                if (string.IsNullOrEmpty(username))
                {
                    return CreateResponse(403, "Username claim not found in token");
                }

                if (string.IsNullOrEmpty(request.Body))
                    return CreateResponse(400, "Request body is missing");

                var orderRequest = System.Text.Json.JsonSerializer.Deserialize<OrderRequest>(request.Body);

                if (orderRequest == null || orderRequest.Items == null || orderRequest.Items.Count == 0)
                    return CreateResponse(400, "Invalid order data");

                // Ensure the user placing the order matches the user from token


                // Calculate the total amount of the order
                var orderId = await GenerateUniqueOrderIdAsync();

                decimal totalAmount = orderRequest.Items.Sum(item => item.Price * item.Quantity);

                var order = new Order
                {
                    OrderId = orderId,
                    UserId = userIdFromDb.UserId,
                    Email= emailFromToken,
                    Items = orderRequest.Items,
                    ShippingAddress = orderRequest.ShippingAddress,
                    TotalAmount = totalAmount
                };

                // Save the order to DynamoDB
                await _context.SaveAsync(order);

                // Clear the user's cart after placing the order
                await ClearUserCart(userIdFromDb.UserId);

                // Return successful response with Order ID
                return CreateResponse(200, $"Order placed successfully. Order ID: {orderId}");
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error placing order: {ex.Message}");
                return CreateResponse(500, "Internal server error");
            }
        }
        private async Task<string> GenerateUniqueOrderIdAsync()
        {
            string orderId;
            var random = new Random();

            do
            {
                int number = random.Next(100000, 999999);
                orderId = $"order_{number}";
            }
            while (await OrderIdExistsAsync(orderId));

            return orderId;
        }
        private async Task<bool> OrderIdExistsAsync(string orderId)
        {
            var response = await _dynamoDb.GetItemAsync("Orders", new Dictionary<string, AttributeValue>
            {
                ["orderId"] = new AttributeValue { S = orderId }
            });

            return response.Item != null && response.Item.Count > 0;
        }


        // Helper method to create a standardized API response
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

        // Example of ClearUserCart method
        private async Task ClearUserCart(string userId)
        {
            var table = Table.LoadTable(_dynamoDb, "Cart");

            // Create a QueryFilter to query based on userId (partition key)
            var filter = new QueryFilter("userId", QueryOperator.Equal, userId);

            // Perform the query on the Cart table
            var search = table.Query(filter);

            // Execute the query and retrieve all matching items
            var cartItems = await search.GetRemainingAsync();

            // Delete each item in the cart
            foreach (var item in cartItems)
            {
                await table.DeleteItemAsync(item);
            }
        }

        //private async Task ClearUserCart(string userId)
        //{
        //    var table = Table.LoadTable(_dynamoDb, "Cart");  // Make sure "Cart" is the correct table name

        //    // Create a QueryFilter to query based on userId (partition key)
        //    var filter = new QueryFilter("userId", QueryOperator.Equal, userId);

        //    // Perform the query on the Cart table
        //    var search = table.Query(filter);

        //    // Execute the query and retrieve all matching items
        //    var cartItems = await search.GetRemainingAsync();

        //    // Delete each item in the cart
        //    foreach (var item in cartItems)
        //    {
        //        await table.DeleteItemAsync(item);
        //    }
        //}


        //private APIGatewayProxyResponse CreateResponse(int statusCode, string message) =>
        //    new APIGatewayProxyResponse
        //    {
        //        StatusCode = statusCode,
        //        Body = JsonSerializer.Serialize(new { message }),
        //        Headers = new Dictionary<string, string>
        //        {
        //            { "Content-Type", "application/json" },
        //            { "Access-Control-Allow-Origin", "*" }
        //        }
        //    };

        public class OrderRequest
        {
            //[JsonPropertyName("userId")]
            //public string UserId { get; set; }

            [JsonPropertyName("items")]
            public List<OrderItem> Items { get; set; }

            [JsonPropertyName("shippingAddress")]
            public string ShippingAddress { get; set; }

            //[JsonPropertyName("paymentMethod")]
            //public string PaymentMethod { get; set; }
        }

    }
}
