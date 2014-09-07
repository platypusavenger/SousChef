using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class ReceiptModel
    {
        public int id { get; set; }
        public int inventoryUserSourceId { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class ReceiptModelDetailed
    {
        public int id { get; set; }
        public int inventoryUserSourceId { get; set; }
        public DateTime timestamp { get; set; }
        public List<int> Items { get; set; }
    }
}