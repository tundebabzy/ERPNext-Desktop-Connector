using ERPNext_Desktop_Connector.Objects;
using RestSharp;

namespace ERPNext_Desktop_Connector.Commands
{
    internal class SupplierCommand
    {
        private readonly Resource _receiver;

        public SupplierCommand(string supplierName, string serverUrl = "https://portal.electrocomptr.com")
        {
            _receiver = new Resource(serverUrl, supplierName);
        }

        public IRestResponse<SupplierResponse> Execute()
        {
            IRestResponse<SupplierResponse> documents = _receiver.GetSupplierDetails();
            return documents;
        }
    }
}
