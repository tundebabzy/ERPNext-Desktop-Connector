using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class LogPurchaseOrderHandler: AbstractDocumentHandler
    {
        public LogPurchaseOrderHandler(Company c, ILogger logger) : base(c, logger)
        {
        }

        public override object Handle(object request)
        {
            LogPurchaseOrder(request as PurchaseOrderDocument);
            return base.Handle(request);
        }

        private void LogPurchaseOrder(PurchaseOrderDocument document)
        {
            var url = $"{Properties.Settings.Default.ServerAddress}/api/resource/Sage 50 Export Log";
            var resource = new Resource(url);
            resource.LogPurchaseOrder(document);
        }
    }
}
