using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SousChef.Data;

namespace SousChef.Models
{
    public class InventoryModel
    {
        public int id { get; set; }
        public string description { get; set; }
        public string version { get; set; }
    }

    public class InventoryModelCurrent : InventoryModel
    {
        public List<InventoryItem> InventoryItems { get; set; }
    }

    public class InventoryModelNeeded : InventoryModel
    {
        public List<InventoryShoppingListItem> InventoryItems { get; set; }
    }

}