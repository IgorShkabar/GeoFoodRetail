/** 
 * Copyright (c) 2026 Igor Shkabar. All Rights reserved.
 * The file from the Class Library (.NET Standard) project.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DatabaseServiceLib
{
    // The builder class itself, 
    // which configures the service
    public class OracleDbBuilder
    {
        // Database connection string must be private only
        private string _connectionString;

        // You can pass cmd arguments here,
        // if they are needed for configuration
        public string[] Args { get; }

        public OracleDbBuilder(string[] args)
        {
            Args = args;
        }

        // Example of a method for configuring a connection string
        public OracleDbBuilder UseConnectionString(string connectionString)
        {
            _connectionString = connectionString;            
            return this;
        }

        // Smart method: if the string is not explicitly provided, read it from appsettings.json
        public OracleDbBuilder UseConfiguration()
        {
            var cnfg = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = cnfg["ConnectionString:OracleDb"];
            return this;
        }
        
        // Метод, который финально собирает готовый сервис базы данных
        public OracleDbService Build()
        {
            if(string.IsNullOrEmpty(_connectionString))
            {
                throw new
                InvalidOperationException("Connection string not specified! Call UseConnectionString or UseConfiguration.");
            }            
            return new OracleDbService(_connectionString);
        }
    }
}

