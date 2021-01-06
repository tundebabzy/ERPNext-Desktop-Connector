using System;

namespace ERPNext_Desktop_Connector.Objects
{
    public class PurchaseOrderItem
    {
        public decimal Amount { get; set; }
        public string Carrier { get; set; }
        public string DateCode { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Doctype { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Uom { get; set; }
        public string Warehouse { get; set; }
    }
}
