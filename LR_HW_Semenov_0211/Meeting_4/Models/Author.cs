using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meeting_4.Models
{
    public class Author
    {
        public Author()
        {
            Books = new List<Book>();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
