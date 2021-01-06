using Sage.Peachtree.API;
using Serilog;
using System;
using System.Collections.Generic;

namespace ERPNext_Desktop_Connector.Objects
{
    class EmployeeInformation
    {
        public ILogger Logger { get; set; }
        public Company Company { get; set; }
        public Dictionary<string, EntityReference<Employee>> Data;
        public EmployeeInformation(ILogger logger = null)
        {
            if (logger != null)
            {
                Logger = logger;
            }
        }

        public Dictionary<string, EntityReference<Employee>> Load(Company company)
        {
            EmployeeList employeeList = company.Factories.EmployeeFactory.List();
            employeeList.Load();
            Dictionary<string, EntityReference<Employee>> employeeDictionary = new Dictionary<string, EntityReference<Employee>>();

            foreach (var employee in employeeList)
            {
                try
                {
                    employeeDictionary.Add(employee.Name, employee.Key);
                }
                catch (ArgumentException e)
                {
                    Logger.Debug(e, e.Message);
                }
            }

            Data = employeeDictionary;

            return employeeDictionary;
        }
    }
}
