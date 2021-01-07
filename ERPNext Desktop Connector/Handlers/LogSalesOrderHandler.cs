using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class LogSalesOrderHandler: AbstractDocumentHandler
    {
        public LogSalesOrderHandler(Company c, ILogger logger, EmployeeInformation employeeInformation) : base(c, logger, employeeInformation) { }

        public override object Handle(object request)
        {
            LogSalesOrder(request as SalesOrderDocument);
            return base.Handle(request);
        }

        private void LogSalesOrder(SalesOrderDocument document)
        {
            var url = $"{Properties.Settings.Default.ServerAddress}/api/resource/Sage 50 Export Log";
            var resource = new Resource(url);
            resource.LogSalesOrder(document);
        }
    }
}
