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
    public class AddToCartHandler
    {
        private readonly CartService _cartService;

        public AddToCartHandler()
        {
            var dynamoDb = new AmazonDynamoDBClient();
            _cartService = new CartService(dynamoDb);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Body))
                    return CreateResponse(400, "Request body is missing");

                var cartRequest = JsonSerializer.Deserialize<CartRequest>(request.Body);

                if (cartRequest is null || string.IsNullOrEmpty(cartRequest.UserId) ||
                    string.IsNullOrEmpty(cartRequest.ProductId) || cartRequest.Quantity <= 0)
                {
                    return CreateResponse(400, "Invalid request");
                }

                var cartItem = new CartItem
                {
                    UserId = cartRequest.UserId,
                    ProductId = cartRequest.ProductId,
                    Quantity = cartRequest.Quantity
                };

                await _cartService.AddToCartAsync(cartItem);
                return CreateResponse(200, "Item added to cart");
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex.ToString());
                return CreateResponse(500, "Internal server error");
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
        public class CartRequest
        {
            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("productId")]
            public string ProductId { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }
        }

    }

}
