/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * The file from the Class Library (.NET Standard) project.
 Production repository
 This is where you place your SQL query. This class will not be invoked during testing.
 */

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseServiceLib
{
    public class OracleStoreRepository : IStoreRepository
    {
        private readonly string _cnnStr;

        public OracleStoreRepository(string cnnStr)
        {
            _cnnStr = cnnStr;
        }

        public IEnumerable<(object StoreId, object StoreNmb, object Lon, object Lat)> GetStoresData()
        {
            var query = @"
            SELECT STORE_ID, STORE_NUMBER, 
                   SDO_UTIL.GET_X(STORE_GEOM) AS LON, 
                   SDO_UTIL.GET_Y(STORE_GEOM) AS LAT 
            FROM C##GIS_USER.STORES_LOCATIONS";

            using (var cnn = new OracleConnection(_cnnStr))
            using (var command = new OracleCommand(query, cnn))
            {
                cnn.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (reader["STORE_ID"], reader["STORE_NUMBER"], reader["LON"], reader["LAT"]);
                    }
                }
            }
        }
    }
}
