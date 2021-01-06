using System.Collections.Generic;

namespace ERPNext_Desktop_Connector.Objects
{
    public class SupplierDocument
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string SupplierName { get; set; }
        public string SupplierType { get; set; }
        public string VendorEmail { get; set; }
        public string Website { get; set; }
        public int Disabled { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string FaxNumber { get; set; }
        public string VendorId { get; set; }
        public string PaymentTerms { get; set; }
        public string Doctype { get; set; }
        public List<ContactDocument> Contacts { get; set; }
        public List<AddressDocument> Addresses { get; set; }
    }
}
