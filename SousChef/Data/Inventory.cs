//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SousChef.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Inventory
    {
        public Inventory()
        {
            this.InventoryUsers = new HashSet<InventoryUser>();
            this.InventoryShoppingLists = new HashSet<InventoryShoppingList>();
        }
    
        public int id { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string model { get; set; }
    
        public virtual ICollection<InventoryUser> InventoryUsers { get; set; }
        public virtual ICollection<InventoryShoppingList> InventoryShoppingLists { get; set; }
    }
}
