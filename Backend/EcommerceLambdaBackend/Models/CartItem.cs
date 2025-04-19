using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Models
{
    [DynamoDBTable("Cart")]
    public class CartItem
    {
        [DynamoDBHashKey("userId")]
        public string UserId { get; set; }

        [DynamoDBProperty("productId")]
        public string ProductId { get; set; }

        [DynamoDBProperty("quantity")]
        public int Quantity { get; set; }

        [DynamoDBProperty("addedAt")]
        public string AddedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }

}
