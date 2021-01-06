using ERPNext_Desktop_Connector.Objects;
using RestSharp;

namespace ERPNext_Desktop_Connector.Commands
{
    class SalesOrderCommand
    {
        private readonly Resource _receiver;

        public SalesOrderCommand(string serverUrl = "https://portal.electrocomptr.com")
        {
            _receiver = new Resource(serverUrl);
        }

        public IRestResponse<SalesOrderResponse> Execute()
        {
            var documents = _receiver.GetSalesOrderList();
            return documents;
        }
    }
}
