using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Interfaces;
using ERPNext_Desktop_Connector.Objects;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPNext_Desktop_Connector.Handlers
{
    class AbstractDocumentHandler : IDocumentHandler
    {
        private IDocumentHandler _nextHandler;

        protected Company Company { get; set; }

        protected Dictionary<string, EntityReference> ItemReferences { get; set; }

        protected ILogger Logger { get; set; }

        protected Dictionary<string, EntityReference<Vendor>> VendorReferences { get; set; }


        protected AbstractDocumentHandler(Company c, ILogger logger)
        {
            Company = c;
            Logger = logger;
        }

        protected static CustomerDocument GetCustomerFromErpNext(string name)
        {
            var receiver = new CustomerCommand(name, $"{GetCustomerResourceServerAddress()}");
            var customerDocument = receiver.Execute();
            return customerDocument.Data.Message;
        }

        protected static SupplierDocument GetSupplierFromErpNext(string name)
        {
            var receiver = new SupplierCommand(name, $"{GetSupplierResourceServerAddress()}");
            var supplierDocument = receiver.Execute();
            return supplierDocument.Data.Message;
        }

        protected string GetLineDescription(SalesOrderItem line)
        {
            var text = line.PartNumber;
            text += String.IsNullOrEmpty(line.PartNumber2) ? "" : $", RPN:{line.PartNumber2}";
            text += String.IsNullOrEmpty(line.Manufacturer) ? "" : $", MFR:{line.Manufacturer}";
            text += String.IsNullOrEmpty(line.DateCode) ? "" : $", D/C:{line.DateCode}";
            text += String.IsNullOrEmpty(line.Msl) ? "" : $", MSL:{line.Msl}";
            return text;
        }

        protected string GetLineDescription(SalesInvoiceItem line)
        {
            var text = line.PartNumber;
            text += String.IsNullOrEmpty(line.PartNumber2) ? "" : $", RPN:{line.PartNumber2}";
            text += String.IsNullOrEmpty(line.Manufacturer) ? "" : $", MFR:{line.Manufacturer}";
            text += String.IsNullOrEmpty(line.DateCode) ? "" : $", D/C:{line.DateCode}";
            text += String.IsNullOrEmpty(line.Msl) ? "" : $", MSL:{line.Msl}";
            return text;
        }

        private static string GetCustomerResourceServerAddress()
        {
            return $"{Properties.Settings.Default.ServerAddress}/api/method/electro_erpnext.utilities.customer.get_customer_details";
        }

        protected EntityReference GetItemEntityReference(string itemCode)
        {
            try
            {
                var itemList = Company.Factories.InventoryItemFactory.List();
                var filter = GetPropertyContainsLoadModifiers("InventoryItem.ID", itemCode);
                itemList.Load(filter);
                var entity = itemList.FirstOrDefault((item => item.ID == itemCode));
                return entity?.Key;
            }
            catch (Exception e)
            {
                Logger.Debug($"Could not get item entity reference. @{e.Message}");
                return null;
            }
        }

        protected EntityReference<Customer> GetCustomerEntityReference(string documentOldCustomerId)
        {
            try
            {
                var customerList = Company.Factories.CustomerFactory.List();
                var filter = GetPropertyContainsLoadModifiers("Customer.ID", documentOldCustomerId);
                customerList.Load(filter);

                var entity = customerList.FirstOrDefault((customer => customer.ID == documentOldCustomerId));
                return entity?.Key;
            }
            catch (Exception e)
            {
                Logger.Debug($"Could not get customer entity reference. @{e.Message}");
                return null;
            }
        }

        protected EntityReference<Employee> GetSalesRepEntityReference(string employeeName)
        {
            try
            {
                var employeeList = Company.Factories.EmployeeFactory.List();
                var filter = GetPropertyContainsLoadModifiers("Employee.ID", employeeName);
                employeeList.Load(filter);

                var entity = employeeList.FirstOrDefault((employee => employee.Name == employeeName));
                return entity?.Key;
            }
            catch (Exception e)
            {
                Logger.Debug($"Could not get employee entity reference. @{e.Message}");
                return null;
            }
        }

        private static string GetSupplierResourceServerAddress()
        {
            return $"{Properties.Settings.Default.ServerAddress}/api/method/electro_erpnext.utilities.supplier.get_supplier_details";
        }

        protected EntityReference<Vendor> GetVendorEntityReference(string vendorId)
        {
            try
            {
                var vendorList = Company.Factories.VendorFactory.List();
                var filter = GetPropertyContainsLoadModifiers("Vendor.ID", vendorId);
                vendorList.Load(filter);

                var entity = vendorList.FirstOrDefault((vendor => vendor.ID == vendorId));
                return entity?.Key;
            }
            catch (Exception e)
            {
                Logger.Debug($"Could not get vendor entity reference. @{e.Message}");
                return null;
            }
        }

        private static LoadModifiers GetPropertyContainsLoadModifiers(string propertyString, string identifier)
        {
            var filter = LoadModifiers.Create();
            var property = FilterExpression.Property(propertyString);
            var value = FilterExpression.StringConstant(identifier);
            var filterParams = FilterExpression.Contains(property, value);
            filter.Filters = filterParams;
            return filter;
        }

        protected InventoryItem LoadInventoryItem(EntityReference itemReference)
        {
            var specializedType = itemReference.SpecializedType;
            if (specializedType == typeof(ServiceItem))
            {
                return Company.Factories.ServiceItemFactory.Load(itemReference as EntityReference<ServiceItem>);
            }

            if (specializedType == typeof(StockItem))
            {
                return Company.Factories.StockItemFactory.Load(itemReference as EntityReference<StockItem>);
            }

            if (specializedType == typeof(DescriptionOnlyItem))
            {
                return Company.Factories.DescriptionOnlyItemFactory.Load(
                    itemReference as EntityReference<DescriptionOnlyItem>);
            }

            if (specializedType == typeof(ActivityItem))
            {
                return Company.Factories.ActivityItemFactory.Load(itemReference as EntityReference<ActivityItem>);
            }

            if (specializedType == typeof(AssemblyItem))
            {
                return Company.Factories.AssemblyItemFactory.Load(itemReference as EntityReference<AssemblyItem>);
            }

            if (specializedType == typeof(ChargeItem))
            {
                return Company.Factories.ChargeItemFactory.Load(itemReference as EntityReference<ChargeItem>);
            }

            if (specializedType == typeof(LaborItem))
            {
                return Company.Factories.LaborItemFactory.Load(itemReference as EntityReference<LaborItem>);
            }

            if (specializedType == typeof(MasterStockItem))
            {
                return Company.Factories.MasterStockItemFactory.Load(itemReference as EntityReference<MasterStockItem>);
            }

            if (specializedType == typeof(NonStockItem))
            {
                return Company.Factories.NonStockItemFactory.Load(itemReference as EntityReference<NonStockItem>);
            }

            if (specializedType == typeof(SerializedAssemblyItem))
            {
                return Company.Factories.SerializedAssemblyItemFactory.Load(
                    itemReference as EntityReference<SerializedAssemblyItem>);
            }

            if (specializedType == typeof(SerializedStockItem))
            {
                return Company.Factories.SerializedStockItemFactory.Load(
                    itemReference as EntityReference<SerializedStockItem>);
            }

            if (specializedType == typeof(SubStockItem))
            {
                return Company.Factories.SubStockItemFactory.Load(itemReference as EntityReference<SubStockItem>);
            }

            return null;
        }

        public IDocumentHandler SetNext(IDocumentHandler handler)
        {
            this._nextHandler = handler;
            return handler;
        }

        protected IDocumentHandler GetNext()
        {
            return _nextHandler;
        }

        public virtual object Handle(object request)
        {
            return _nextHandler?.Handle(request);

        }
    }
}
