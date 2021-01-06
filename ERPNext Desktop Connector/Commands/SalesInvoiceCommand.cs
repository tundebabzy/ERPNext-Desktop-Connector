using ERPNext_Desktop_Connector.Objects;
using RestSharp;

namespace ERPNext_Desktop_Connector.Commands
{
    class SalesInvoiceCommand
    {
        private readonly Resource _receiver;

        public SalesInvoiceCommand(string serverUrl = "https://portal.electrocomptr.com")
        {
            _receiver = new Resource(serverUrl);
        }

        public IRestResponse<SalesInvoiceResponse> Execute()
        {
            IRestResponse<SalesInvoiceResponse> documents = _receiver.GetSalesInvoiceList();
            return documents;
        }
    }
}
