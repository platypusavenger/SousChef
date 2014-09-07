using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class InventoryShoppingListModel
    {
        public int id { get; set; }
        public int inventoryId { get; set; }
        public int itemId { get; set; }
        public int inventoryUserId { get; set; }
    }
}