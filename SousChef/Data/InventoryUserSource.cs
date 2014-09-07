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
    
    public partial class InventoryUserSource
    {
        public InventoryUserSource()
        {
            this.Receipts = new HashSet<Receipt>();
        }
    
        public int id { get; set; }
        public int inventoryUserId { get; set; }
        public int sourceId { get; set; }
        public string accessKey { get; set; }
    
        public virtual InventoryUser InventoryUser { get; set; }
        public virtual Source Source { get; set; }
        public virtual ICollection<Receipt> Receipts { get; set; }
    }
}