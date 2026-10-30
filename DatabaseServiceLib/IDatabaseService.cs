/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Creating an interface and a service database.

  Abstract database operations via an interface. 
  Pass the connection type itself to the service constructor.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseServiceLib
{
    public interface IDatabaseService
    {
        void ExecuteQuery(string sql);
        DataTable GetDataTable(string sql);
        Task<DataTable> GetDataTableAsync(string sql);

        /// Extend the IDatabaseService interface and the OracleDbService class 
        /// so that they can accept parameters.

        /// Overload for safe parameter passing (including BoundingBox)
        /// 
        DataTable GetDataTable(string sql, params OracleParameter[] parameters);

        Task ExecuteNonQueryAsync(string sql, params OracleParameter[] parameters); 
    }
}
