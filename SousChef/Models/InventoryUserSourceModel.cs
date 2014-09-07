using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class InventoryUserSourceModel
    {
        public int id { get; set;}
        public int inventoryUserId { get; set; }
        public int sourceId { get; set; }
        public string accessKey { get; set; }
    }
}