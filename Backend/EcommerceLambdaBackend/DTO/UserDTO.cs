using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.DTO
{
   
    [DynamoDBTable("Users")]
    public class UserDTO
    {
        [DynamoDBHashKey] // Make sure this matches the actual PK
        public string Email { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }
    }
}
