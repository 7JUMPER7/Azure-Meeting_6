using System;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace CosmosDBExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string endpointUrl = "https://localhost:8081";
            string authorizationKey = "AUTHKEY";
            string databaseId = "ShopDB";
            string containerId = "Orders";
            string keyPath = "/OrderTitle";
            Console.WriteLine("Hello World!");
            CosmosClient client = new CosmosClient(endpointUrl, authorizationKey);
            Database database = await CreateDatabaseAsync(client, databaseId);
            Container container = await CreateContainerAsync(client, databaseId, containerId, keyPath);
            await AddItemsToContainerAsync(container);
        }

        static async Task<Database> CreateDatabaseAsync(CosmosClient client, string databaseId)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
            System.Console.WriteLine($"Database {database.Id} created");
            return database;
        }
        static async Task<Container> CreateContainerAsync(CosmosClient client, string databaseId, string containerId, string partitionKeyPath)
        {
            Container container = await client.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(containerId, partitionKeyPath);
            System.Console.WriteLine($"Container {container.Id} created");
            return container;
        }
        static async Task AddItemsToContainerAsync(Container container)
        {
            Order order1 = new Order {
                Id = "Pfizer.1",
                OrderTitle = "Pfizer",
                Address = new Address {Country = "Ukraine", City = "Kriviy Rih", Line1 = "Matisevicha 5"},
                BuyerSurname = "Ivanov",
                Manager = new Manager {Firstname = "Zenaida", Surname = "Petrova"},
                OrderDescription = "Vanccine for Ukraine"
            };
            order1.OrderItems.AddRange(new[] {
                new OrderItem {Id = 1, Count = 1000, Price = 2, Title = "Pfizer Vaccine"},
                new OrderItem {Id = 2, Count = 1200, Price = 0.01, Title = "Syringe"},
            });
            order1.Manager.PhoneNumbers.Add(new Phone {PhoneNumber = "+380123456789"});
            order1.Manager.PhoneNumbers.Add(new Phone {PhoneNumber = "+380987654321"});

            Order order2 = new Order {
                Id = "Moderna.1",
                OrderTitle = "Moderna",
                Address = new Address {Country = "Ukraine", City = "Kriviy Rih", Line1 = "Matisevicha 7"},
                BuyerSurname = "Petrov",
                Manager = new Manager {Firstname = "Zenaida", Surname = "Ivanova"},
                OrderDescription = "Vanccine for Ukraine 2"
            };
            order2.OrderItems.AddRange(new[] {
                new OrderItem {Id = 3, Count = 500, Price = 2.5, Title = "Moderna Vaccine"},
                new OrderItem {Id = 4, Count = 600, Price = 0.01, Title = "Syringe"},
            });
            order2.Manager.PhoneNumbers.Add(new Phone {PhoneNumber = "+380123123123"});
            order2.Manager.PhoneNumbers.Add(new Phone {PhoneNumber = "+380321321321"});

            try
            {
                ItemResponse<Order> response1 = await container.ReadItemAsync<Order>("Pfizer.1", new PartitionKey("Pfizer"));
                System.Console.WriteLine($"Order with id: {response1.Resource.Id}");
                System.Console.WriteLine($"Diff: {response1.RequestCharge}");

            }
            catch(CosmosException ce) when(ce.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ItemResponse<Order> response2 = await container.CreateItemAsync<Order>(order1, new PartitionKey(order1.OrderTitle));
                System.Console.WriteLine($"Order added with id: {response2.Resource.Id}");
                System.Console.WriteLine($"Diff: {response2.RequestCharge}");
            }
        }
    }
}
