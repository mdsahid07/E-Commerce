using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EcommerceLambdaBackend.Models;
using System.Text.Json;

namespace EcommerceLambdaBackend.Handlers
{
    public class GetAllProductsHandler
    {
        private readonly DynamoDBContext _context;

        public GetAllProductsHandler()
        {
            var client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(client);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("Fetching all products from DynamoDB...");

                var scanConditions = new List<ScanCondition>(); // No filters, get all items
                var allProducts = await _context.ScanAsync<Product>(scanConditions).GetRemainingAsync();

                var json = JsonSerializer.Serialize(allProducts);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = json,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" }
                    }
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error fetching products: {ex}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonSerializer.Serialize(new { message = "Internal server error" }),
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" }
                    }
                };
            }
        }
    }
}
