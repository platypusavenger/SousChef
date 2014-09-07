using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SousChef.Models
{
    public class ReceiptItemModel
    {
        public int id { get; set; }
        public int receiptId { get; set; }
        public int itemId { get; set; }
    }
}