using System;
using System.Collections.Generic;

namespace ERPNext_Desktop_Connector.Objects
{
    public class SalesInvoiceDocument
    {
        public string AmendedFrom { get; set; }
        public string Company { get; set; }
        public string Currency { get; set; }
        public string Customer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPurchaser { get; set; }
        public DateTime DateReceived { get; set; }
        public Decimal DiscountAmount { get; set; }
        public string Docstatus { get; set; }
        public string Doctype { get; set; }
        public DateTime DueDate { get; set; }
        public string GrandTotal { get; set; }
        public List<SalesInvoiceItem> Items { get; set; }
        public string Name { get; set; }
        public string NotesOrSpecialInstructions { get; set; }
        public string OldCustomerId { get; set; }
        public string OrderEnteredBy { get; set; }
        public string PaymentTermsTemplate { get; set; }
        public string PoNo { get; set; }
        public DateTime PostingDate { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceivingData { get; set; }
        public string RoundingAdjustment { get; set; }
        public string SalesRep { get; set; }
        public DateTime ShipDate { get; set; }
        public string ShippingMethod { get; set; }
        public string Total { get; set; }
        public string TotalQty { get; set; }
    }
}
