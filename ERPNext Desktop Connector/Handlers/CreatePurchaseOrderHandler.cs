using ERPNext_Desktop_Connector.Interfaces;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;
using System;
using System.Collections.Generic;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class CreatePurchaseOrderHandler: AbstractDocumentHandler, IResourceAddress
    {
        public CreatePurchaseOrderHandler(Company c, ILogger logger, EmployeeInformation employeeInformation = null) : base(c, logger, employeeInformation) { }
        public override object Handle(object request)
        {
            Logger.Information("Version {@Version}", Settings.Version);
            var purchaseOrder = CreateNewPurchaseOrder(request as PurchaseOrderDocument);
            if (GetNext() == null)
            {
                this.SetNext(purchaseOrder != null ? new LogPurchaseOrderHandler(Company, Logger, EmployeeInformation) : null);
            }
            return base.Handle(request);
        }

        private PurchaseOrder CreateNewPurchaseOrder(PurchaseOrderDocument purchaseOrderDocument)
        {
            var supplierDocument = GetSupplierFromErpNext(purchaseOrderDocument.Supplier);
            var supplierEntityReference = GetVendorEntityReference(supplierDocument?.VendorId);
            var purchaseOrder = Company.Factories.PurchaseOrderFactory.Create();
            if (supplierEntityReference == null)
            {
                Logger.Debug("Supplier {@name} in {@Document} was not found in Sage.", purchaseOrderDocument.Supplier, purchaseOrderDocument.Name);
                purchaseOrder = null;
                SetNext(new CreateSupplierHandler(Company, Logger, EmployeeInformation));
                Logger.Debug("Supplier {@name} has been queued for creation in Sage", purchaseOrderDocument.Supplier);
            }
            else if (purchaseOrder != null)
            {
                try
                {
                    purchaseOrder.Date = purchaseOrderDocument.TransactionDate;
                    purchaseOrder.GoodThroughDate = purchaseOrderDocument.ScheduleDate;
                    purchaseOrder.ReferenceNumber = purchaseOrderDocument.Name;
                    // purchaseOrder.VendorReference = VendorReferences[purchaseOrderDocument.Supplier];
                    purchaseOrder.VendorReference = supplierEntityReference;
                    purchaseOrder.TermsDescription = purchaseOrderDocument.PaymentTermsTemplate;
                    purchaseOrder.ShipVia = purchaseOrderDocument.ShipMethod;

                    foreach (var line in purchaseOrderDocument.Items)
                    {
                        AddLine(purchaseOrder, line);
                    }

                    purchaseOrder.Save();
                    Logger.Information("Purchase Order - {PurchaseOrderDocument} saved successfully", purchaseOrderDocument.Name);
                }
                catch (KeyNotFoundException)
                {
                    purchaseOrder = null;
                    Logger.Debug("Vendor {@Name} in {@Document} was not found", purchaseOrderDocument.Supplier, purchaseOrderDocument.Name);
                    SetNext(new CreateSupplierHandler(Company, Logger, EmployeeInformation));
                    Logger.Debug("Customer {@name} has been queued for creation in Sage", purchaseOrderDocument.Supplier);
                }
                catch (Sage.Peachtree.API.Exceptions.ValidationException e)
                {
                    Logger.Debug("Validation failed.");
                    Logger.Debug(e.Message);
                    Logger.Debug("{@Name} will be sent back to the queue", purchaseOrderDocument.Name);
                    purchaseOrder = null;
                }
                catch (Sage.Peachtree.API.Exceptions.RecordInUseException)
                {
                    purchaseOrder = null;
                    Logger.Debug("Record is in use. {@Name} will be sent back to the queue", purchaseOrderDocument.Name);
                }
                catch (Exception e)
                {
                    purchaseOrder = null;
                    Logger.Debug(e, e.Message);
                    Logger.Debug("{@E}", e);
                }
            }
            return purchaseOrder;
        }

        private void AddLine(PurchaseOrder purchaseOrderDocument, PurchaseOrderItem line)
        {
            var _ = purchaseOrderDocument.AddLine();
            var itemReference = GetItemEntityReference(line.ItemCode);
            if (itemReference == null)
            {
                Logger.Debug("{@Item} was not found in Sage. Please create it manually for to be imported", line.ItemCode);
                return;
            }
            var stockItem = Company.Factories.StockItemFactory.Load(itemReference as EntityReference<StockItem>);
            _.AccountReference = stockItem.COGSAccountReference;
            _.Quantity = line.Qty;
            _.UnitPrice = line.Rate;
            _.Amount = line.Amount;
            _.Description = line.Description;
            _.InventoryItemReference = itemReference;
        }

        public string GetResourceServerAddress()
        {
            throw new NotImplementedException();
        }
    }
}
