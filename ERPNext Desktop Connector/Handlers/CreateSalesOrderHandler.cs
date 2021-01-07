using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Validations;
using Serilog;
using System;
using System.Linq;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class CreateSalesOrderHandler: AbstractDocumentHandler
    {
        public CreateSalesOrderHandler(Company c, ILogger logger, EmployeeInformation employeeInformation = null) : base(c, logger, employeeInformation) { }


        public override object Handle(object request)
        {
            Logger.Information("Version {@Version}", Settings.Version);
            var salesOrder = CreateNewSalesOrder(request as SalesOrderDocument);
            if (GetNext() == null)
            {
                this.SetNext(salesOrder != null ? new LogSalesOrderHandler(Company, Logger, EmployeeInformation) : null);
            }
            return base.Handle(request);
        }

        private SalesOrder CreateNewSalesOrder(SalesOrderDocument document)
        {
            var customerDocument = GetCustomerFromErpNext(document.CustomerName);
            var salesOrder = Company.Factories.SalesOrderFactory.Create();
            EntityReference<Customer> customerEntityReference = null;
            if (customerDocument != null && customerDocument.OldCustomerId != null)
            {
                customerEntityReference = GetCustomerEntityReference(customerDocument.OldCustomerId);
            }
            if (customerEntityReference == null)
            {
                Logger.Debug("Customer {@name} in {@Document} was not found in Sage.", document.Customer, document.Name);
                salesOrder = null;
                SetNext(new CreateCustomerHandler(Company, Logger, EmployeeInformation));
                Logger.Debug("Customer {@name} has been queued for creation in Sage", document.Customer);
            }
            else if (salesOrder != null)
            {
                try
                {
                    salesOrder.CustomerReference = customerEntityReference;
                    salesOrder.CustomerPurchaseOrderNumber = document.PoNo;
                    salesOrder.CustomerNote = document.NotesOrSpecialInstructions;
                    salesOrder.Date = document.TransactionDate;
                    salesOrder.DiscountAmount = document.DiscountAmount;
                    salesOrder.DiscountDate = document.TransactionDate;
                    salesOrder.ReferenceNumber = document.Name;
                    salesOrder.ShipByDate = document.DeliveryDate < document.TransactionDate ? document.TransactionDate : document.DeliveryDate;
                    if (document.DeliveryDate <= document.TransactionDate)
                    {
                        Logger.Information("{@Name} has delivery date has been set to transaction date because delivery date is earlier than transaction date", document.Name);
                    }
                    salesOrder.ShipVia = document.ShippingMethod;
                    salesOrder.TermsDescription = document.PaymentTermsTemplate;
                    salesOrder.CustomerPurchaseOrderNumber = document.PoNo;
                    AddSalesRep(salesOrder, document);
                    AddShipAddress(salesOrder);

                    foreach (var line in document.Items)
                    {
                        AddLine(salesOrder, line);
                    }

                    salesOrder.Save();
                    Logger.Information("Sales Order - {0} was saved successfully", document.Name);
                }
                catch (Sage.Peachtree.API.Exceptions.RecordInUseException)
                {
                    // abort. The unsaved data will eventually be re-queued
                    salesOrder = null;
                    Logger.Debug("Record is in use. {@Name} will be sent back to the queue", document.Name);
                }
                catch (ArgumentException e)
                {
                    salesOrder = null;
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
                        salesOrder = null;
                    }
                }
                catch (Exception e)
                {
                    salesOrder = null;
                    Logger.Debug(e, e.Message);
                    Logger.Debug("{@E}", e);
                }
            }
            return salesOrder;
        }

        private void AddShipAddress(SalesOrder salesOrder)
        {
            var customer = Company.Factories.CustomerFactory.Load(salesOrder.CustomerReference);
            var contact = customer.ShipToContact;
            salesOrder.ShipToAddress.Name = customer.Name;
            salesOrder.ShipToAddress.Address.Zip = contact.Address.Zip;
            salesOrder.ShipToAddress.Address.Address1 = contact.Address.Address1;
            salesOrder.ShipToAddress.Address.Address2 = contact.Address.Address2;
            salesOrder.ShipToAddress.Address.City = contact.Address.City;
            salesOrder.ShipToAddress.Address.State = contact.Address.State;
            salesOrder.ShipToAddress.Address.Country = contact.Address.Country;
        }

        private void AddSalesRep(SalesOrder salesOrder, SalesOrderDocument document)
        {
            if (document.SalesRep == null) return;
            var salesRep = EmployeeInformation.Data[document.SalesRep];
            salesOrder.SalesRepresentativeReference = salesRep;
        }

        /**
         * Adds a `SalesOrderLine` to a `SalesOrder` object and populates it.
         * Note:
         * If the given `SalesOrderItem.ForFreight` property is 1, the amount is
         * is added to `SalesOrder.Freight` and no `SalesOrderLine` is added.
         * If the given `SalesOrderItem.ForHandling` property is 1, no `SalesOrderLine`
         * is added to the `SalesOrderLine`.
         */
        private void AddLine(SalesOrder salesOrder, SalesOrderItem line)
        {
            if (line.ForFreight == 1)
            {
                salesOrder.FreightAmount = line.Amount;
            }
            else if (line.ForHandling != 1)
            {
                var _ = salesOrder.AddLine();
                var itemReference = GetItemEntityReference(line.ItemCode);
                if (itemReference == null)
                {
                    Logger.Debug("Item {@name} was not found in Sage.", line.ItemCode);
                    Logger.Debug("Item {@name} needs to be created in Sage", line.ItemCode);
                    return;
                }

                var item = LoadInventoryItem(itemReference);
                _.AccountReference = GetSalesAccountReference(item);
                _.Quantity = line.Qty;
                _.Description = GetLineDescription(line);
                _.UnitPrice = Decimal.Divide(line.Amount, line.Qty);    // _.CalculateUnitCost(_.Quantity, _.Amount);
                _.Amount = _.CalculateAmount(_.Quantity, _.UnitPrice);
                _.InventoryItemReference = itemReference;
            }
        }

        private static EntityReference<Account> GetSalesAccountReference(InventoryItem item)
        {
            switch (item)
            {
                case ServiceItem serviceItem:
                    return serviceItem.SalesAccountReference;
                case StockItem stockItem:
                    return stockItem.SalesAccountReference;
                case ActivityItem activityItem:
                    return activityItem.SalesAccountReference;
                case AssemblyItem assemblyItem:
                    return assemblyItem.SalesAccountReference;
                case ChargeItem chargeItem:
                    return chargeItem.SalesAccountReference;
                case LaborItem laborItem:
                    return laborItem.SalesAccountReference;
                case MasterStockItem masterStockItem:
                    return masterStockItem.SalesAccountReference;
                case NonStockItem nonStockItem:
                    return nonStockItem.SalesAccountReference;
                case SerializedAssemblyItem serializedAssemblyItem:
                    return serializedAssemblyItem.SalesAccountReference;
                case SerializedStockItem serializedStockItem:
                    return serializedStockItem.SalesAccountReference;
                case SubStockItem subStockItem:
                    return subStockItem.SalesAccountReference;
                default:
                    return null;
            }
        }
    }
}
