using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using EcommerceLambdaBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Services
{
    public class CartService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly DynamoDBContext _context;

        public CartService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
            _context = new DynamoDBContext(_dynamoDb);
        }

        public async Task AddToCartAsync(CartItem item)
        {
            await _context.SaveAsync(item);
        }
    }


}
