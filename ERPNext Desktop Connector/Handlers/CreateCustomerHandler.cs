using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Interfaces;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Exceptions;
using Serilog;
using System;

namespace ERPNext_Desktop_Connector.Handlers
{
    internal class CreateCustomerHandler: AbstractDocumentHandler, IResourceAddress
    {
        protected string CustomerName { get; set; }
        public CreateCustomerHandler(Company company, ILogger logger, EmployeeInformation employeeInformation) : base(company, logger, employeeInformation) { }

        public override object Handle(object request)
        {
            Logger.Information("Version {@Version}", Settings.Version);
            var customerName = (request as SalesOrderDocument)?.Customer;
            var customerDocument = customerName != null ? GetCustomerDetails(customerName) : null;
            var customer = customerDocument != null ? CreateNewCustomer(customerDocument) : null;
            this.SetNext(customer != null ? new LogCustomerCreatedHandler(Company, Logger, EmployeeInformation) : null);
            return base.Handle(customerDocument);
        }

        private CustomerDocument GetCustomerDetails(string customerName)
        {
            var receiver = new CustomerCommand(customerName, $"{GetResourceServerAddress()}");
            var document = receiver.Execute();

            return document.Data.Message;
        }

        private Customer CreateNewCustomer(CustomerDocument customerDocument)
        {
            var customer = Company.Factories.CustomerFactory.Create();
            if (customer == null)
            {
                Logger.Information("Customer data was null when trying to create Sage customer");
                return null;
            }

            if (customerDocument == null || customerDocument.Addresses.Count == 0)
            {
                Logger.Information("Customer has no address so the customer cannot be created");
                return customer;
            }
            try
            {
                customer.ID = customerDocument.OldCustomerId;    // add a field - ID to Customer doctype
                customer.Name = customerDocument.CustomerName;
                AddContact(customer, customerDocument);
                AddSalesRep(customer, customerDocument);

                customer.Save();
                Logger.Information("Customer - {Customer} saved successfully", customerDocument.CustomerName);
            }
            catch (ValidationException e)
            {
                Logger.Debug("Validation failed.");
                Logger.Debug(e.Message);
                Logger.Debug("{@Name} will be sent back to the queue", customerDocument.Name);
                customer = null;
            }
            catch (RecordInUseException)
            {
                customer = null;
                Logger.Debug("Record is in use. {@Name} will be sent back to the queue", customerDocument.Name);
            }
            catch (Exception e)
            {
                customer = null;
                Logger.Debug(e, e.Message);
                Logger.Debug("{@E}", e);
            }

            return customer;
        }

        private void AddContact(Customer customer, CustomerDocument customerDocument)
        {
            if (customerDocument.Addresses.Count > 0)
            {
                customer.Email = customerDocument.CustomerEmail;
                customer.ShipVia = customerDocument.ShipVia;
                customer.WebSiteURL = customerDocument.CompanyWebsite;
                customer.CustomerSince = DateTime.Now;
                customer.IsInactive = customerDocument.Disabled == 1;
                AddAddresses(customer, customerDocument);
                AddContacts(customer, customerDocument);
            }
            else
            {
                Logger.Information("Customer {@Name} did not have addresses so will not create a contact", customer.Name);
                ContactList contactsList = customer.Contacts;
                contactsList.Clear();
            }
        }

        private static void AddAddresses(Customer customer, CustomerDocument customerDocument)
        {
            var billingAddress = customerDocument.Addresses.Find(x => x.AddressType == "Billing");
            var shippingAddress = customerDocument.Addresses.Find(x => x.AddressType == "Shipping");
            if (billingAddress != null)
            {
                var state = TransformState(billingAddress.State);
                customer.BillToContact.Address.Address1 = billingAddress.AddressLine1;
                customer.BillToContact.Address.Address2 = billingAddress.AddressLine2;
                customer.BillToContact.Address.City = billingAddress.City;
                customer.BillToContact.Address.State = GetStateFromPredefinedStates(state);
                customer.BillToContact.Address.Zip = billingAddress.Pincode;
                customer.BillToContact.Address.Country = billingAddress.Country;
            }
            if (shippingAddress != null)
            {
                var state = TransformState(shippingAddress.State);
                customer.ShipToContact.Address.Address1 = shippingAddress.AddressLine1;
                customer.BillToContact.Address.Address2 = shippingAddress.AddressLine2;
                customer.BillToContact.Address.City = shippingAddress.City;
                customer.BillToContact.Address.State = GetStateFromPredefinedStates(state);
                customer.BillToContact.Address.Zip = shippingAddress.Pincode;
                customer.BillToContact.Address.Country = shippingAddress.Country;
            }
        }

        /// <summary>
        /// Returns the two character form of a US state.
        /// If <code>state</code> is already in two character form and is a value
        /// in <code>Settings.States</code>, the match is returned.
        /// If <code>state</code> is a key in <code>Settings.States</code>, the
        /// value of the key is returned
        /// Else, a string that is the first two characters of <code>state</code>
        /// is returned. 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static string GetStateFromPredefinedStates(string state)
        {
            if (string.IsNullOrEmpty(state)) return "";

            if (state.Length == 2 && Settings.States.ContainsValue(state.ToUpper()))
            {
                return state.ToUpper();
            }

            if (Settings.States.ContainsKey(state))
            {
                return Settings.States[state];
            }

            return state.Substring(0, 2);
        }

        /// <summary>
        /// Returns the state as either unchanged if it is two characters long
        /// or in title case if not (e.g North Carolina)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static string TransformState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return "";
            }

            if (state.Length == 2)
            {
                return state.ToUpper();
            }

            return char.ToUpper(state[0]) + state.Substring(1).ToLower();
        }

        private void AddSalesRep(Customer customer, CustomerDocument customerDocument)
        {
            EmployeeList employees = Company.Factories.EmployeeFactory.List();
            employees.Load();
            foreach (var item in employees)
            {
                if (item.Name != customerDocument.SalesRep) continue;
                customer.SalesRepresentativeReference = item.Key;
                break;
            }
        }

        private void AddContacts(Customer customer, CustomerDocument customerDocument)
        {
            foreach (var c in customerDocument.Contacts)
            {
                if (!string.IsNullOrEmpty(customer.BillToContact.CompanyName) && !string.IsNullOrEmpty(customer.ShipToContact.CompanyName))
                {
                    break;
                }
                else if (customer.BillToContact.Address.Address1 != null && (string.IsNullOrEmpty(customer.BillToContact.CompanyName)
                                                                             && (customer.BillToContact.Address.Address1 != null || customer.BillToContact.Address.Address1.Length != 0)))
                {
                    customer.BillToContact.CompanyName = customerDocument.CustomerName;
                    customer.BillToContact.Email = c.EmailId;
                    customer.BillToContact.FirstName = c.FirstName;
                    customer.BillToContact.Gender = c.Gender == "Male" ? Gender.Male : c.Gender == "Female" ? Gender.Female : Gender.NotSpecified;
                    customer.BillToContact.LastName = c.LastName;
                    customer.BillToContact.Title = c.Salutation;
                }
                else if (customer.ShipToContact.Address.Address1 != null && (string.IsNullOrEmpty(customer.ShipToContact.CompanyName)
                                                                             && (customer.ShipToContact.Address.Address1 != null || customer.ShipToContact.Address.Address1.Length != 0)))
                {
                    customer.ShipToContact.CompanyName = customerDocument.CustomerName;
                    customer.ShipToContact.Email = c.EmailId;
                    customer.ShipToContact.FirstName = c.FirstName;
                    customer.ShipToContact.Gender = c.Gender == "Male" ? Gender.Male : c.Gender == "Female" ? Gender.Female : Gender.NotSpecified;
                    customer.ShipToContact.LastName = c.LastName;
                    customer.ShipToContact.Title = c.Salutation;
                }
            }
        }

        public string GetResourceServerAddress()
        {
            return $"{Properties.Settings.Default.ServerAddress}/api/method/electro_erpnext.utilities.customer.get_customer_details";
        }
    }
}
