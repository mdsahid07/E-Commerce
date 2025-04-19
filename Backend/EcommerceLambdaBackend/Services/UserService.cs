using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using EcommerceLambdaBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using EcommerceLambdaBackend.DTO;

namespace EcommerceLambdaBackend.Services
{
    public class UserService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "Users";


        public UserService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var response = await _dynamoDb.GetItemAsync(TableName, new Dictionary<string, AttributeValue>
            {
                ["email"] = new AttributeValue { S = email }
            });

            return response.Item != null && response.Item.Count > 0;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var context = new DynamoDBContext(_dynamoDb);

                // Directly load the user based on email (partition key)
                var user = await context.LoadAsync<User>(email);
                Console.WriteLine($"Loaded user: {user?.Username}, PasswordHash: {user?.PasswordHash}");

                return user; // Will return null if the user is not found
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Error occurred: {ex.Message}");
                throw; // Rethrow exception to handle further up the stack
            }
        }

        public async Task<UserDTO> GetUserByEmailAsync2(string email)
        {
            var context = new DynamoDBContext(_dynamoDb);

            var scanConditions = new List<ScanCondition>
    {
        new ScanCondition("Email", ScanOperator.Equal, email)
    };

            var search = context.ScanAsync<UserDTO>(scanConditions);
            var results = await search.GetNextSetAsync();

            return results.FirstOrDefault();
        }



        public async Task CreateUserAsync(User user)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                ["email"] = new AttributeValue { S = user.Email },
                ["userId"] = new AttributeValue { S = user.UserId },
                ["username"] = new AttributeValue { S = user.Username },
                ["passwordHash"] = new AttributeValue { S = user.PasswordHash },
                ["createdAt"] = new AttributeValue { S = user.CreatedAt }
            };

            await _dynamoDb.PutItemAsync(new PutItemRequest
            {
                TableName = TableName,
                Item = item
            });
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
