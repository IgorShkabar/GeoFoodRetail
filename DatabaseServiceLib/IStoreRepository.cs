/** 
 Copyright (c) 2026 Igor Shkabar. All rights reserved.
 The file from the Class Library (.NET Standard) project.
 Isolation from the Database (for true Unit Test)

The goal of the test is not to verify that a query is sent to Oracle, 
but rather to check how the code processes the data it receives. 
The logic for interacting with `OracleConnection` should be moved into a separate 
service (e.g., `StoreRepository`), and an interface—along with the Moq 
library—should be used for testing.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseServiceLib
{
    public interface IStoreRepository
    {
        //Since StoreDto does not exist, you can use a standard tuple(int Id, double Lon, double Lat) or System.Data.DataTable.
        // Returns a collection of tuples, since StoreDto does not exist
        IEnumerable<(object StoreId, object StoreNmb, object Lon, object Lat)> GetStoresData();
    }
}
