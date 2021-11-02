using Meeting_4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Meeting_4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CosmosClient _client;
        private Container bookContainer;
        private Container authorsContainer;

        public HomeController(ILogger<HomeController> logger, CosmosClient client)
        {
            _logger = logger;
            _client = client;
            bookContainer = _client.GetContainer("BooksShop", "Books");
            authorsContainer = _client.GetContainer("BooksShop", "Authors");
        }

        public async Task<IActionResult> Index()
        {
            string query = $"SELECT * FROM c";
            QueryDefinition definition = new QueryDefinition(query);
            FeedIterator<Book> iterator = bookContainer.GetItemQueryIterator<Book>(definition);
            List<Book> books = new List<Book>();
            while (iterator.HasMoreResults)
            {
                FeedResponse<Book> feed = await iterator.ReadNextAsync();
                foreach (Book item in feed.Resource)
                {
                    books.Add(item);
                }
            }
            return View(books);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Book book)
        {
            string authorId = '"' + book.Author.Id + '"'; // Представляете, только так находит в БД, одинарные кавычки не канают 🙃
            string query = $"SELECT * FROM c WHERE c.id = {authorId}";
            QueryDefinition definition = new QueryDefinition(query);
            FeedIterator<Author> iterator = authorsContainer.GetItemQueryIterator<Author>(definition);
            List<Author> authors = new List<Author>();
            while (iterator.HasMoreResults)
            {
                FeedResponse<Author> feed = await iterator.ReadNextAsync();
                foreach (Author item in feed.Resource)
                {
                    authors.Add(item);
                }
            }

            if (authors.Count != 0)
            {
                book.Author = authors[0];
                ItemResponse<Book> res = await bookContainer.CreateItemAsync<Book>(book, new PartitionKey(book.Title));
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddAuthor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAuthor(Author author)
        {
            ItemResponse<Author> res = await authorsContainer.CreateItemAsync<Author>(author, new PartitionKey(author.Name));
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
