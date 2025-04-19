using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Models
{

    [DynamoDBTable("Products")]
    public class Product
    {
        [DynamoDBProperty("productid")]
        public string ProductId { get; set; }

        [DynamoDBProperty("name")]
        public string Name { get; set; }

        [DynamoDBProperty("description")]
        public string Description { get; set; }

        [DynamoDBProperty("price")]
        public double Price { get; set; }

        [DynamoDBProperty("imageurl")]
        public string ImageUrl { get; set; }

        [DynamoDBProperty("createdat")]
        public string CreatedAt { get; set; }
    }

}
