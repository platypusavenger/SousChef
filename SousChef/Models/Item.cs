using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class Item
    {
        public int id { get; set; }
        public int description { get; set; }
        public bool isPerishable { get; set; }
        public int expiration { get; set; }
        public string image { get; set; }
    }
}