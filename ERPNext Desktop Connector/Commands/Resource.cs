using ERPNext_Desktop_Connector.Objects;
using RestSharp;
using RestSharp.Serialization.Json;
using System;

namespace ERPNext_Desktop_Connector.Commands
{
    class Resource
    {
        private readonly IRestClient _restClient;
        private const string ApiSecret = "78e5615db2830e3";
        private const string ApiToken = "4e7b90c8fc265ac";
        private string Param { get; set; }
        public Resource(string baseUrl, string customerName = "")
        {
            Param = customerName;
            _restClient = new RestClient(baseUrl);
            _restClient.UseSerializer(() => new JsonSerializer { DateFormat = "yyyy-MM-dd" });
            _restClient.AddDefaultHeader("Authorization", $"token {ApiToken}:{ApiSecret}");
        }

        public IRestResponse<CustomerResponse> GetCustomerDetails()
        {
            var request = new RestRequest(Method.GET);
            if (!string.IsNullOrEmpty(Param))
            {
                request.AddQueryParameter("cn", Param);
            }
            var response = _restClient.Execute<CustomerResponse>(request);
            return response;
        }

        public IRestResponse<PurchaseOrderResponse> GetPurchaseOrderList()
        {
            var request = new RestRequest(Method.GET);
            var response = _restClient.Execute<PurchaseOrderResponse>(request);
            return response;
        }

        /**
         * Makes a request to the given ERPNext server and pulls the data in JSON format
         */
        public IRestResponse<SalesOrderResponse> GetSalesOrderList()
        {
            var request = new RestRequest(Method.GET);
            var response = _restClient.Execute<SalesOrderResponse>(request);
            return response;
        }

        public IRestResponse<SalesInvoiceResponse> GetSalesInvoiceList()
        {
            var request = new RestRequest(Method.GET);
            var response = _restClient.Execute<SalesInvoiceResponse>(request);
            return response;
        }

        public IRestResponse<SupplierResponse> GetSupplierDetails()
        {
            var request = new RestRequest(Method.GET);
            if (!string.IsNullOrEmpty(Param))
            {
                request.AddQueryParameter("cn", Param);
            }
            var response = _restClient.Execute<SupplierResponse>(request);
            return response;
        }

        public IRestResponse LogCustomer(CustomerDocument document)
        {
            var log = new Log
            {
                document_name = document.Name,
                export_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_type = "Customer"
            };
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(log);
            var response = _restClient.Execute(request);
            return response;
        }

        public IRestResponse LogPurchaseOrder(PurchaseOrderDocument document)
        {
            var log = new Log
            {
                document_name = document.Name,
                export_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_date = document.TransactionDate.ToString("yyyy-MM-dd"),
                document_type = "Purchase Order"
            };
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(log);
            var response = _restClient.Execute(request);
            return response;
        }

        public IRestResponse LogSalesInvoice(SalesInvoiceDocument document)
        {
            var log = new Log
            {
                document_name = document.Name,
                export_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_date = document.PostingDate.ToString("yyyy-MM-dd"),
                document_type = "Sales Invoice"
            };
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(log);
            var response = _restClient.Execute(request);
            return response;
        }

        public IRestResponse LogSalesOrder(SalesOrderDocument document)
        {
            var log = new Log
            {
                document_name = document.Name,
                export_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_date = document.TransactionDate.ToString("yyyy-MM-dd"),
                document_type = "Sales Order"
            };
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(log);
            var response = _restClient.Execute(request);
            return response;
        }

        public IRestResponse LogSupplier(SupplierDocument supplierDocument)
        {
            var log = new Log
            {
                document_name = supplierDocument.Name,
                export_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_date = DateTime.Now.ToString("yyyy-MM-dd"),
                document_type = supplierDocument.Doctype
            };

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(log);
            var response = _restClient.Execute(request);
            return response;
        }
    }
}
