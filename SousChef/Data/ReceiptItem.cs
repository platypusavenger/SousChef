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
    
    public partial class ReceiptItem
    {
        public int id { get; set; }
        public int receiptId { get; set; }
        public int itemId { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual Receipt Receipt { get; set; }
    }
}
