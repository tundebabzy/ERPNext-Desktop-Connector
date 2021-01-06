using System;
using System.Collections.Generic;

namespace ERPNext_Desktop_Connector.Objects
{
    public class PurchaseOrderDocument
    {
        public string AmendedFrom { get; set; }
        public string Company { get; set; }
        public string Currency { get; set; }
        public string DiscountAmount { get; set; }
        public string Docstatus { get; set; }
        public string Doctype { get; set; }
        public string GrandTotal { get; set; }
        public string Incoterms { get; set; }
        public List<PurchaseOrderItem> Items { get; set; }
        public string Name { get; set; }
        public string PaymentTermsTemplate { get; set; }
        public string RoundingAdjustment { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string ShipmentBilling { get; set; }
        public string ShipMethod { get; set; }
        public string ShippingAccountId { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingService { get; set; }
        public string Supplier { get; set; }
        public string SupplierName { get; set; }
        public string Total { get; set; }
        public string TotalQty { get; set; }
        public DateTime TransactionDate { get; set; }
        public string VendorId { get; set; }
    }
}
