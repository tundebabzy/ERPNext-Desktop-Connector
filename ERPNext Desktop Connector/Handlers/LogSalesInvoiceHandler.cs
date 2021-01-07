﻿using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class LogSalesInvoiceHandler: AbstractDocumentHandler
    {
        public LogSalesInvoiceHandler(Company c, ILogger logger) : base(c, logger) { }

        public override object Handle(object request)
        {
            LogSalesInvoice(request as SalesInvoiceDocument);
            return base.Handle(request);
        }

        private void LogSalesInvoice(SalesInvoiceDocument salesInvoiceDocument)
        {
            var url = $"{Properties.Settings.Default.ServerAddress}/api/resource/Sage 50 Export Log";
            var resource = new Resource(url);
            resource.LogSalesInvoice(salesInvoiceDocument);
        }
    }
}
