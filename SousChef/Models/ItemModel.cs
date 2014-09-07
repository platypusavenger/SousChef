using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class ItemModel
    {
        public int id { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public bool isPerishable { get; set; }
        public int expiration { get; set; }
        public string image { get; set; }
    }
}