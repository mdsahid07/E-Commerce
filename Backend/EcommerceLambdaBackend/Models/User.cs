using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Models
{
    [DynamoDBTable("Users")] // Use your actual table name
    public class User
    {
        [DynamoDBProperty("userid")] // This is your Partition Key
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty("username")]
        public string Username { get; set; }

       
        [DynamoDBProperty("email")]
        public string Email { get; set; }

        [DynamoDBProperty("passwordHash")]
        public string PasswordHash { get; set; }

        [DynamoDBProperty("createdat")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }

}
