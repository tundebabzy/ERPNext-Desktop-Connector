using ERPNext_Desktop_Connector.Commands;
using ERPNext_Desktop_Connector.Events;
using ERPNext_Desktop_Connector.Handlers;
using ERPNext_Desktop_Connector.Objects;
using ERPNext_Desktop_Connector.Options;
using Sage.Peachtree.API;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Timers;

namespace ERPNext_Desktop_Connector
{
    public class Connector
    {
        private const string CompanyName = "Electro-Comp Tape & Reel Services, LLC";
        private string ApplicationId = Properties.Settings.Default.ApplicationId;
        private bool _canRequest = true;
        private Timer _timer;
        private ILogger Logger { get; set; }
        private static PeachtreeSession Session { get; set; }
        public static Company Company { get; set; }
        public ConcurrentQueue<object> Queue = new ConcurrentQueue<object>();
        private EmployeeInformation EmployeeInformation { get; } = new EmployeeInformation();

        public event EventHandler ConnectorStarted;
        public event EventHandler ConnectorStopped;
        public event EventHandler<EventDataArgs> PeachtreeInformation;
        public event EventHandler<EventDataArgs> ConnectorInformation;
        private void OpenCompany(CompanyIdentifier companyId)
        {
            // Request authorization from Sage 50 for our third-party application.
            try
            {
                OnPeachtreeInformation(EventData("Requesting access to your company in Sage 50..."));
                var authorizationResult = Session.RequestAccess(companyId);

                // Check to see we were granted access to Sage 50 company, if so, go ahead and open the company.
                if (authorizationResult == AuthorizationResult.Granted)
                {
                    Company = Session.Open(companyId);
                    Logger.Information("Authorization granted");
                    OnPeachtreeInformation(EventData("Access to your company was granted"));
                    InitSalesRepresentativeList();
                }
                else // otherwise, display a message to user that there was insufficient access.
                {
                    Logger.Error("Authorization request was not successful - {0}. Will retry.", authorizationResult);
                    OnPeachtreeInformation(EventData("Still waiting for authorization to access your company..."));
                }
            }
            catch (Sage.Peachtree.API.Exceptions.LicenseNotAvailableException e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData("Could not open your company because it seems like you do not have a license."));
            }
            catch (Sage.Peachtree.API.Exceptions.RecordInUseException e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData("Could not open your company as one or more records are in use."));
            }
            catch (Sage.Peachtree.API.Exceptions.AuthorizationException e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData($"Could not open your company as authorization failed. {e.Message}"));
            }
            catch (Sage.Peachtree.API.Exceptions.PeachtreeException e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData($"Could not open your company due to a Sage 50 internal error. {e.Message}"));
            }
            catch (Exception e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData($"Something went wrong. {e.Message}"));
            }
        }

        private void InitSalesRepresentativeList()
        {
            EmployeeInformation.Logger = Logger;
            EmployeeInformation.Load(Company);
        }

        private void CloseCompany()
        {
            Company?.Close();
            OnPeachtreeInformation(EventData("Company was closed"));
        }

        private void OpenSession(string appKeyId)
        {
            try
            {
                if (Session != null)
                {
                    CloseSession();
                }

                // create new session                                
                Session = new PeachtreeSession();

                // start the new session
                Session.Begin(appKeyId);
                OnPeachtreeInformation(EventData("Sage 50 session has started"));
            }
            catch (Sage.Peachtree.API.Exceptions.ApplicationIdentifierExpiredException e)
            {
                Logger.Debug(e, "Your application identifier has expired.");
                OnPeachtreeInformation(EventData("Your application identifier has expired"));
            }
            catch (Sage.Peachtree.API.Exceptions.ApplicationIdentifierRejectedException e)
            {
                Logger.Debug(e, "Your application identifier was rejected.");
                OnPeachtreeInformation(EventData("Your application identifier was rejected."));
            }
            catch (Sage.Peachtree.API.Exceptions.PeachtreeException e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData(e.Message));
            }
            catch (Exception e)
            {
                Logger.Debug(e, e.Message);
                OnPeachtreeInformation(EventData(e.Message));
            }
        }

        private EventDataArgs EventData(string v)
        {
            var args = new EventDataArgs
            {
                Text = v
            };
            return args;
        }

        // Closes current Sage 50 Session
        //
        private void CloseSession()
        {
            if (Session != null && Session.SessionActive)
            {
                Session.End();
                OnPeachtreeInformation(EventData("Sage 50 session ended"));
            }
        }

        public Connector()
        {
            SetupLogger();
            Logger.Information("initializing object");
        }

        private void SetupLogger()
        {
            const string path = @"%PROGRAMDATA%\IWERPNextPoll\Logs\log-.txt";
            var logFilePath = Environment.ExpandEnvironmentVariables(path);
            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void OnStart()
        {
            _canRequest = true;
            Logger.Information("Service started");
            Logger.Debug("State = {0}", _canRequest);
            OpenSession(ApplicationId);
            StartTimer();
        }

        private void ClearQueue()
        {
            Logger.Information("Version {@Version}", Settings.Version);
            if (Queue.IsEmpty || Company == null || Company.IsClosed) return;
            var handler = new DocumentTypeHandler(Company, Logger, EmployeeInformation);
            while (Queue.TryDequeue(out var document) && Session.SessionActive)
            {
                handler.Handle(document);
                OnConnectorInformation(EventData("Busy"));
            }
            OnConnectorInformation(EventData("No more documents to process at the moment"));
        }


        private void StartTimer()
        {
            _timer = new Timer
            {
                Interval = Settings.TimerInterval
            };
            _timer.Elapsed += OnTimer;
            _timer.Start();
            OnConnectorStarted(EventArgs.Empty);
            Logger.Information("Timer started");
            Logger.Information("Timer interval is {0} minutes", _timer.Interval / 60000);
        }

        /**
         * Starts the process of getting documents from ERPNext and pushing
         * them into Sage 50.
         * When there is no active session or the service cannot connect to
         * the company, `OnTimer` will fail silently.
         */
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            Logger.Information("Timer callback called");
            if (!_canRequest || (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 17))
            {
                Logger.Debug("Service cannot request: {0}, {1}", _canRequest, DateTime.Now.Hour);
                var message = _canRequest ? "Connector is idling till 5pm" : "Connector is stopped";
                OnConnectorInformation(EventData(message));
                return;
            }

            if (Company == null || Company.IsClosed)
            {
                DiscoverAndOpenCompany();
            }
            else
            {
                Logger.Debug("Company is null: {0}; Company is closed: {1}", Company == null, Company?.IsClosed);
            }

            if (Session != null && Session.SessionActive && Company != null)
            {
                if (!Company.IsClosed)
                {
                    GetDocumentsThenProcessQueue();
                }
                else
                {
                    OnConnectorInformation(EventData("Could not fetch data because company is closed"));
                    Logger.Debug("Session is null: {0}, Session is active: {1}, Company is null: {2}", Session == null, Session?.SessionActive, Company == null);
                }
            }
            else
            {
                OnConnectorInformation(EventData($"Session is initialized: {Session != null}; Session is active: {Session != null && Session.SessionActive}; Company is initialized: {Company != null}"));
                Logger.Debug("Session is initialized: {0}", Session != null);
                Logger.Debug("Session is active: {0}", Session != null && Session.SessionActive);
                Logger.Debug("Company is initialized: {0}", Company != null);
            }
        }

        private void GetDocumentsThenProcessQueue()
        {
            GetDocuments();
            ClearQueue();
        }

        private CompanyIdentifier DiscoverCompany()
        {
            bool Predicate(CompanyIdentifier c) { return c.CompanyName == CompanyName; }
            var companies = Session.CompanyList();
            var company = companies.Find(Predicate);
            return company;
        }

        private void DiscoverAndOpenCompany()
        {
            var company = DiscoverCompany();
            if (company != null)
            {
                OpenCompany(company);
            } else
            {
                OnPeachtreeInformation(EventData("No company was found."));
            }
        }

        /**
         * Pull documents from ERPNext and queue them for processing.
         * The documents pulled are Sales Orders and Purchase Orders, in that order
         */
        private void GetDocuments()
        {
            var purchaseOrderCommand = new PurchaseOrderCommand(serverUrl: $"{Settings.ServerUrl}/api/method/electro_erpnext.utilities.purchase_order.get_purchase_orders_for_sage");
            var salesOrderCommand = new SalesOrderCommand(serverUrl: $"{Settings.ServerUrl}/api/method/electro_erpnext.utilities.sales_order.get_sales_orders_for_sage");
            var salesInvoiceCommand = new SalesInvoiceCommand(serverUrl: $"{Settings.ServerUrl}/api/method/electro_erpnext.utilities.sales_invoice.get_sales_invoices_for_sage");

            var salesOrders = salesOrderCommand.Execute();
            OnConnectorInformation(EventData($"ERPNext sent {salesOrders?.Data.Message.Count} sales orders."));
            var purchaseOrders = purchaseOrderCommand.Execute();
            OnConnectorInformation(EventData($"ERPNext sent {purchaseOrders?.Data.Message.Count} sales orders."));
            var salesInvoices = salesInvoiceCommand.Execute();
            OnConnectorInformation(EventData($"ERPNext sent {salesInvoices?.Data.Message.Count} sales orders."));

            SendToQueue(salesOrders.Data);
            SendToQueue(purchaseOrders.Data);
            SendToQueue(salesInvoices.Data);
        }

        /**
         * Push documents to internal queue
         */
        private void SendToQueue(SalesOrderResponse response)
        {
            if (response?.Message == null) return;
            foreach (var item in response.Message)
            {
                this.Queue.Enqueue(item);
            }

        }

        private void SendToQueue(PurchaseOrderResponse response)
        {
            if (response?.Message == null) return;
            foreach (var item in response.Message)
            {
                this.Queue.Enqueue(item);
            }

        }

        private void SendToQueue(SalesInvoiceResponse response)
        {
            if (response?.Message == null) return;
            foreach (var item in response.Message)
            {
                this.Queue.Enqueue(item);
            }
        }

        public void OnStop()
        {
            _canRequest = false;
            CloseCompany();
            CloseSession();
            StopTimer();
            Logger.Debug("Timer stopped");
        }

        private void StopTimer()
        {
            _timer?.Stop();
            _timer?.Close();
            OnConnectorStopped(EventArgs.Empty);
            OnConnectorInformation(EventData("Connector has cleaned up its connection to Sage 50 and is now idling"));
        }

        protected virtual void OnConnectorStarted(EventArgs e)
        {
            EventHandler eventHandler = ConnectorStarted;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnConnectorInformation(EventDataArgs e)
        {
            var eventHandler = ConnectorInformation;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnConnectorStopped(EventArgs e)
        {
            var eventHandler = ConnectorStopped;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnPeachtreeInformation(EventDataArgs e)
        {
            var eventHandler = PeachtreeInformation;
            eventHandler?.Invoke(this, e);
        }
    }
}
