using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class LogSupplierCreatedHandler: AbstractDocumentHandler
    {
        public LogSupplierCreatedHandler(Company company, ILogger logger, EmployeeInformation employeeInformation) : base(company, logger, employeeInformation) { }

        public override object Handle(object request)
        {
            LogSupplier(request as SupplierDocument);
            // no new handler as we have reached the end of the chain.
            return base.Handle(request);
        }

        private void LogSupplier(SupplierDocument supplierDocument)
        {
            var url = $"{Properties.Settings.Default.ServerAddress}/api/resource/Sage 50 Export Log";
            var resource = new Resource(url);
            resource.LogSupplier(supplierDocument);
        }
    }
}
