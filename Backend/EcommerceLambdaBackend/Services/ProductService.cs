using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Services
{
    internal class ProductService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "Products";


        public ProductService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }
        public async Task<bool> ProductIdExistsAsync(string productId)
        {
            var response = await _dynamoDb.GetItemAsync("Products", new Dictionary<string, AttributeValue>
            {
                ["productid"] = new AttributeValue { S = productId }
            });

            return response.Item != null && response.Item.Count > 0;
        }

    }
}
