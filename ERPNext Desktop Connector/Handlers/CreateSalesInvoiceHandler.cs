using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using Sage.Peachtree.API.Validations;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class CreateSalesInvoiceHandler: AbstractDocumentHandler
    {
        public CreateSalesInvoiceHandler(Company company, ILogger logger,
            EmployeeInformation employeeInformation = null) : base(company, logger, employeeInformation)
        {
        }

        public override object Handle(object request)
        {
            Logger.Information("Version {@Version}", Settings.Version);
            var salesInvoice = CreateNewSalesInvoice(request as SalesInvoiceDocument);
            SetNext(salesInvoice != null ? new LogSalesInvoiceHandler(Company, Logger, EmployeeInformation) : null);
            return base.Handle(request);
        }

        private SalesInvoice CreateNewSalesInvoice(SalesInvoiceDocument document)
        {
            var customerDocument = GetCustomerFromErpNext(document.CustomerName);
            var salesInvoice = Company.Factories.SalesInvoiceFactory.Create();
            EntityReference<Customer> customerEntityReference = null;
            if (customerDocument != null && customerDocument.OldCustomerId != null)
            {
                customerEntityReference = GetCustomerEntityReference(customerDocument.OldCustomerId);
            }
            if (customerEntityReference == null)
            {
                Logger.Debug("Customer {@name} in {@Document} was not found in Sage.", document.Customer,
                    document.Name);
                salesInvoice = null;
                SetNext(new CreateCustomerHandler(Company, Logger, EmployeeInformation));
                Logger.Debug("Customer {@name} has been queued for creation in Sage", document.Customer);
                return salesInvoice;
            }
            
            salesInvoice = _createNewSalesInvoice(document, salesInvoice, customerEntityReference);
            return salesInvoice;
        }

        private SalesInvoice _createNewSalesInvoice(SalesInvoiceDocument document, SalesInvoice salesInvoice,
            EntityReference<Customer> customerEntityReference)
        {
            try
            {
                salesInvoice.CustomerReference = customerEntityReference;
                salesInvoice.CustomerPurchaseOrderNumber = document.PoNo;
                salesInvoice.CustomerNote = document.NotesOrSpecialInstructions;
                salesInvoice.Date = document.PostingDate;
                salesInvoice.DateDue = document.DueDate;
                salesInvoice.DiscountAmount = document.DiscountAmount;
                salesInvoice.ReferenceNumber = document.Name;
                salesInvoice.ShipDate = document.ShipDate;
                salesInvoice.ShipVia = document.ShippingMethod;
                salesInvoice.TermsDescription = document.PaymentTermsTemplate;
                salesInvoice.CustomerPurchaseOrderNumber = document.PoNo;
                AddSalesRep(salesInvoice, document);
                AddShipAddress(salesInvoice);
                AddSalesOrderData(document, salesInvoice);

                salesInvoice.Save();
                Logger.Information("Sales Invoice - {@Name} was saved successfully", document.Name);
            }
            catch (Sage.Peachtree.API.Exceptions.RecordInUseException)
            {
                // abort. The unsaved data will eventually be re-queued
                salesInvoice = null;
                Logger.Debug("Record is in use. {@Name} will be sent back to the queue", document.Name);
            }
            catch (ArgumentException e)
            {
                salesInvoice = null;
                Logger.Debug("There was a problem with creating {@Name}. It will be sent back to the queue", document.Name);
                Logger.Debug("There error is {@E}", e.Message);
            }
            catch (Sage.Peachtree.API.Exceptions.ValidationException e)
            {
                Logger.Debug("Validation failed.");
                Logger.Debug(e.Message);
                if (e.ProblemList.OfType<DuplicateValueProblem>().Any(item => item.PropertyName == "ReferenceNumber"))
                {
                    Logger.Debug("{@Name} is already in Sage so will notify ERPNext", document.Name);
                }
                else
                {
                    Logger.Debug("{@Name} will be sent back to the queue", document.Name);
                    salesInvoice = null;
                }
            }
            catch (Exception e)
            {
                salesInvoice = null;
                Logger.Debug(e, e.Message);
                Logger.Debug("{@E}", e);
            }

            return salesInvoice;
        }

        private void AddSalesOrderData(SalesInvoiceDocument invoiceDocument, SalesInvoice salesInvoice)
        {
            // if the references List below has more than one item, that could potentially cause problems.
            // In my chats with Sherri, I was assured invoices map to sales orders in a one-to-one manner.
            // Still, I'm assuming this 'contract' can be broken in which case, i'll let error handling
            // take over while the problem is discussed with EC
            var references = (List<string>)GetSalesOrderReferences(invoiceDocument);
            foreach (var reference in references)
            {
                if (!String.IsNullOrEmpty(reference))
                {
                    LoadSalesOrderFromName(reference, out var salesOrders);
                    // Sage takes just one Sales Order in the Sales invoice. SalesOrderList should only have one item
                    AddSalesOrderData(salesInvoice, reference, salesOrders.FirstOrDefault(), invoiceDocument.Items);
                }
            }
        }

        private static IEnumerable<string> GetSalesOrderReferences(SalesInvoiceDocument salesInvoice)
        {
            var cache = new List<string>();
            // this seems more readable than a LINQ expression. Probably faster too.
            foreach (var item in salesInvoice.Items)
            {
                if (!cache.Contains(item.SalesOrder))
                {
                    cache.Add(item.SalesOrder);
                }
            }

            return cache;
        }

        private void AddShipAddress(SalesInvoice salesInvoice)
        {
            var customer = Company.Factories.CustomerFactory.Load(salesInvoice.CustomerReference);
            var contact = customer.ShipToContact;
            salesInvoice.ShipToAddress.Name = customer.Name;
            salesInvoice.ShipToAddress.Address.Zip = contact.Address.Zip;
            salesInvoice.ShipToAddress.Address.Address1 = contact.Address.Address1;
            salesInvoice.ShipToAddress.Address.Address2 = contact.Address.Address2;
            salesInvoice.ShipToAddress.Address.City = contact.Address.City;
            salesInvoice.ShipToAddress.Address.State = contact.Address.State;
            salesInvoice.ShipToAddress.Address.Country = contact.Address.Country;
        }

        private void AddSalesRep(SalesInvoice salesInvoice, SalesInvoiceDocument document)
        {
            if (document.SalesRep == null) return;
            var salesRep = EmployeeInformation.Data[document.SalesRep];
            salesInvoice.SalesRepresentativeReference = salesRep;
        }

        private void AddSalesOrderData(SalesInvoice salesInvoice, string salesOrderReference, SalesOrder salesOrder,
            List<SalesInvoiceItem> salesInvoiceItems)
        {
            if (string.IsNullOrEmpty(salesOrderReference) || salesOrder == null) return;
            AddSalesOrderLinesFromSalesOrder(salesInvoice, salesOrderReference, salesOrder, salesInvoiceItems);
        }

        private void AddSalesOrderLinesFromSalesOrder(SalesInvoice salesInvoice, string salesOrderReference,
            SalesOrder salesOrders, List<SalesInvoiceItem> salesInvoiceItems)
        {
            // note: ElectroComp has said they will not be having multiple sales orders linked
            // to a single sales invoice.
            var salesOrderLines = salesOrders?.SalesOrderLines;
            if (salesOrderLines == null) return;
            var candidateSalesInvoiceItems = salesInvoiceItems.Where((item) => item.SalesOrder == salesOrderReference);
            var invoiceItems = candidateSalesInvoiceItems.ToList();

            // So here we anticipate where the invoice only covers part of the lines of the original
            // sales order and also where the invoice has more lines than the original
            for (var i = 0; i < invoiceItems.Count(); i++)
            {
                SetSalesInvoiceSalesOrderLineData(salesInvoice, salesOrderLines[i], invoiceItems[i]);
            }
        }

        private void SetSalesInvoiceSalesOrderLineData(SalesInvoice salesInvoice,
            SalesOrderLine salesOrderLine, SalesInvoiceItem salesInvoiceItem)
        {
            if (salesInvoiceItem.ForFreight == 1)
            {
                salesInvoice.FreightAmount = salesInvoiceItem.Amount;
            }
            else
            {
                var _ = salesInvoice.AddSalesOrderLine(salesOrderLine);
                _.Quantity = salesInvoiceItem.Qty;
                _.Amount = salesInvoiceItem.Amount;
                _.UnitPrice = Decimal.Divide(salesInvoiceItem.Amount, salesInvoiceItem.Qty); // _.CalculateUnitCost(_.Quantity, _.Amount);
                _.Description = GetLineDescription(salesInvoiceItem);
            }
        }

        private void LoadSalesOrderFromName(string salesOrderReference, out SalesOrderList salesOrderLines)
        {
            salesOrderLines = Company.Factories.SalesOrderFactory.List();
            var filter = LoadModifiers.Create();
            var property = FilterExpression.Property("SalesOrder.ReferenceNumber");
            var value = FilterExpression.StringConstant(salesOrderReference);
            var filterParams = FilterExpression.Contains(property, value);
            filter.Filters = filterParams;
            salesOrderLines.Load(filter);
        }
    }
}
