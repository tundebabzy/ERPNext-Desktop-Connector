using System.Collections.Generic;

namespace ERPNext_Desktop_Connector.Objects
{
    public class CustomerDocument
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string CustomerEmail { get; set; }
        public string CompanyWebsite { get; set; }
        public int Disabled { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string FaxNumber { get; set; }
        public string SalesRep { get; set; }
        public string ShipVia { get; set; }
        public string OldCustomerId { get; set; }
        public string Website { get; set; }
        public string EmailId { get; set; }
        public string PaymentTerms { get; set; }
        public string Doctype { get; set; }
        public List<ContactDocument> Contacts { get; set; }
        public List<AddressDocument> Addresses { get; set; }
    }
}
