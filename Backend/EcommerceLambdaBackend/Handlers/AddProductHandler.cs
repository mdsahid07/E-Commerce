using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EcommerceLambdaBackend.Models;
using EcommerceLambdaBackend.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EcommerceLambdaBackend.Handlers
{
    public class AddProductHandler
    {
        private readonly DynamoDBContext _context;
        private readonly ProductService _productService;

        public AddProductHandler()
        {
            var client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(client);
            _productService = new ProductService(client);
        }

        private async Task<string> GenerateUniqueProductIdAsync()
        {
            string productId;
            var random = new Random();

            do
            {
                int number = random.Next(10000, 99999);
                productId = $"prod_{number}";
            }
            while (await _productService.ProductIdExistsAsync(productId));

            return productId;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Body))
                    return CreateResponse(400, "Request body is missing");

                var inputProducts = JsonSerializer.Deserialize<List<ProductRequest>>(request.Body);

                if (inputProducts == null || inputProducts.Count == 0)
                    return CreateResponse(400, "Invalid or empty product list");

                var createdProducts = new List<Product>();

                foreach (var inputProduct in inputProducts)
                {
                    if (string.IsNullOrEmpty(inputProduct.Name) || inputProduct.Price <= 0)
                        continue; // optionally log or collect invalid ones

                    string productId = await GenerateUniqueProductIdAsync();
                    var newProduct = new Product
                    {
                        ProductId = productId,
                        Name = inputProduct.Name,
                        Description = inputProduct.Description,
                        Price = inputProduct.Price,
                        ImageUrl = inputProduct.ImageUrl,
                        CreatedAt = DateTime.UtcNow.ToString("o")
                    };

                    await _context.SaveAsync(newProduct);
                    createdProducts.Add(newProduct);
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = 201,
                    Body = JsonSerializer.Serialize(createdProducts),
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" }
                    }
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"AddProduct error: {ex}");
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

        private class ProductRequest
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("price")]
            public double Price { get; set; }

            [JsonPropertyName("imageurl")]
            public string ImageUrl { get; set; }
        }
    }
}
