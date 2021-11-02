using System.Collections.Generic;

namespace CosmosDBExample
{
    public class Manager
    {
        public Manager()
        {
            PhoneNumbers = new List<Phone>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public List<Phone> PhoneNumbers { get; set; }
    }
}