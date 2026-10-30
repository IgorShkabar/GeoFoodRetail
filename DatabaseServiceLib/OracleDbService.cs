/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Project of the Class Library (.NET Standard) type.
 */

using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseServiceLib
{
    public class OracleDbService : IDatabaseService
    {
        private readonly string _connectionString;

        public OracleDbService(string cnnStr)
        {
            _connectionString = cnnStr;
        }

        // A static method called by the user;
        // it takes an array of strings (string[] args) and returns a configurator object (builder)
        public static OracleDbBuilder CreateBuilder(string[] args)
        {
            return new OracleDbBuilder(args);
        }

        // Example of a method from the database service itself
        public void Connect()
        {
            // Connection check
            using (var cnn = new Oracle.ManagedDataAccess.Client.OracleConnection(_connectionString))
            {
                cnn.Open();
            }
            Console.WriteLine("Connection to the Oracle database has been successfully established!");
        }

        public void ExecuteQuery(string sql)
        {
            // Basic implementation for non-selection queries (INSERT, UPDATE, DELETE)
            using (var cnn = new Oracle.ManagedDataAccess.Client.OracleConnection(_connectionString))
            {
                cnn.Open();
                using (var cmnd = new Oracle.ManagedDataAccess.Client.OracleCommand(sql, cnn))
                {
                    cmnd.ExecuteNonQuery();
                }
            }
            // Database interaction logic using _connectionString
            Console.WriteLine($"Executing request via:  {_connectionString}");
        }

        // A universal method for any grids on forms
        public DataTable GetDataTable(string sql)
        {
            using (var cn = new Oracle.ManagedDataAccess.Client.OracleConnection(_connectionString))
            {
                cn.Open();
                using (var adapter = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(sql, cn))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        // Implementing overloading
        public DataTable GetDataTable(string sql, params OracleParameter[] prms)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand(sql, connection))
                {
                    if (prms != null)
                    {
                        command.Parameters.AddRange(prms);
                    }
                    using (var adapter = new OracleDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Architecturally correct (make the method asynchronous "under the hood"
        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var adapter = new OracleDataAdapter(sql, connection))
                {
                    var dataTable = new DataTable();

                    // Populating a DataTable asynchronously (available in Oracle.ManagedDataAccess)
                    await Task.Run(() => adapter.Fill(dataTable));
                    return dataTable;
                }
            }
        }

        // Universal method
        public async Task ExecuteNonQueryAsync(string sql, params OracleParameter[] prms)
        {
            using (var cnn = new OracleConnection(_connectionString))
            {
                await cnn.OpenAsync();

                using (var cmnd = new OracleCommand(sql, cnn))
                {
                    if(prms != null)
                    {
                        cmnd.Parameters.AddRange(prms);
                    }
                    await cmnd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
