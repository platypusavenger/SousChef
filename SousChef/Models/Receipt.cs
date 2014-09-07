using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class Receipt
    {
        public int id { get; set; }
        public int inventoryUserSourceId { get; set; }
        public DateTime timestamp { get; set; }
    }
}