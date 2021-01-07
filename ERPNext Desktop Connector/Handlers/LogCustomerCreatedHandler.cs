using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class LogCustomerCreatedHandler: AbstractDocumentHandler
    {
        public LogCustomerCreatedHandler(Company company, ILogger logger) : base(company, logger) { }

        public override object Handle(object request)
        {
            LogCustomer(request as CustomerDocument);
            // no new handler as we have reached the end of the chain.
            return base.Handle(request);
        }

        private void LogCustomer(CustomerDocument customerDocument)
        {
            var url = $"{Properties.Settings.Default.ServerAddress}/api/resource/Sage 50 Export Log";
            var resource = new Resource(url);
            resource.LogCustomer(customerDocument);
        }
    }
}
