using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class InventoryUserModel
    {
        public int id { get; set; }
        public int inventoryId { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public bool isVerified { get; set; }
        public string phoneNumber { get; set; }
    }
}