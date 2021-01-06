using System;

namespace ERPNext_Desktop_Connector.Objects
{
    public class SalesOrderItem
    {
        public decimal Amount { get; set; }
        public string Carrier { get; set; }
        public string DateCode { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Doctype { get; set; }
        public int ForFreight { get; set; }
        public int ForHandling { get; set; }
        public string ItemCode { get; set; }
        public decimal LotCharge { get; set; }
        public string Manufacturer { get; set; }
        public string Msl { get; set; }
        public string Name { get; set; }
        public string PartNumber { get; set; }
        public string PartNumber2 { get; set; }
        public string Pitch { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal UnitPrice { get; set; }
        public string Uom { get; set; }
        public string Warehouse { get; set; }
        public string Width { get; set; }
    }
}
