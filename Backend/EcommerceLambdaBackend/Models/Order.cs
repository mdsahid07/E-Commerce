using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceLambdaBackend.Models
{
    [DynamoDBTable("Orders")]
    public class Order
    {
        [DynamoDBProperty("orderId")]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty("userId")]
        public string UserId { get; set; }

        [DynamoDBProperty("items")]
        public List<OrderItem> Items { get; set; }

        [DynamoDBProperty("shippingAddress")]
        public string ShippingAddress { get; set; }

        [DynamoDBProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

        [DynamoDBProperty("orderDate")]
        public string OrderDate { get; set; } = DateTime.UtcNow.ToString("o");

        [DynamoDBProperty("totalAmount")]
        public decimal TotalAmount { get; set; }
    }
}
