using System.Collections.Generic;
using Newtonsoft.Json;

namespace CosmosDBExample
{
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string OrderDescription { get; set; }
        public string OrderTitle { get; set; }
        public string BuyerSurname { get; set; }
        public Address Address { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Manager Manager { get; set; }
    }
}