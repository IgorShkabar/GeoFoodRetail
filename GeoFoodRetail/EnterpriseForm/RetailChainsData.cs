/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 Injecting the service into the consumer (e.g., a controller or business logic)
 The consumer class knows nothing about the connection string; 
  it simply requests `IDatabaseService` in its constructor.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using DatabaseServiceLib;
using System.Windows.Forms;
using GeoFoodRetail.GIS.UI;
using System.Configuration;
using GeoFoodRetail.MainDashboard;

namespace GeoFoodRetail.EnterpriseForm
{
    internal class RetailChainsData
    {
        /// <sumarry>
        /// Injecting the service into the consumer 
        /// (e.g., a controller or business logic).
        /// </sumarry>        

        private readonly IDatabaseService _dbService;
        private readonly string _connectString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Server=myServer;Database=myDB;";

        internal static List<StoreItem> RetailList { get; set; } = new List<StoreItem>
        {
            new StoreItem("Pure Plate", 16.3726, -94.2088),
            new StoreItem("Store E", 27.5301, -122.0326),
            new StoreItem("Bazzar Co.", 12.1054, -4.5112),
            new StoreItem("Q Corp.", 5.9785, -33.2716),
            new StoreItem("Raising Cane's", 63.6166, -112.2008),
            new StoreItem("Leaf & Grain", 21.8308, -31.4150),
            new StoreItem("Bite & Go", 0.4262, -98.4975),
            new StoreItem("BJ's Wholesale Club", 20.7128, 84.0060),
            new StoreItem("Frosted Joy", 21.8781, 37.6298),
            new StoreItem("Urban Bites", 7.0522, -118.2437),
            new StoreItem("KFC", 12.7767, -46.7970),
            new StoreItem("Lay’s", 33.08, 96.80),
            new StoreItem("General Mills", 49.9746, -93.3951),
            new StoreItem("Oreo (Mondelez)", 41.8866331, -27.6500352),
            new StoreItem("Del Monte Foods", 37.9011, -122.0005),
            new StoreItem("Quaker Oats Company", 32.8037912, -96.6658896),
            new StoreItem("Land O’Lakes", 28.2189, -82.4576),
            new StoreItem("Heinz", 40.446732, -80.015816),
            new StoreItem("Publix", 30.3904165, -86.4301700),
            new StoreItem("Meijer", 43.03388, -85.62809),
            new StoreItem("Hy-Vee", 41.59367, -93.78812),
            new StoreItem("H Mart", 33.97448, -117.8376),
            new StoreItem("Trader Joe’s", 21.7563488, -138.3110107),
            new StoreItem("Whole Foods Market (Amazon)", 35.0888037, -89.8021257),
            new StoreItem("Save A Lot", 68.08, 78.26557),
            new StoreItem("H-E-Be", 57.6384, 63.684268),
            new StoreItem("Albertsons", 2.7128, 124.39486),
            new StoreItem("WinCo Foods", -27.7128, 136.68278),
            new StoreItem("Piggly Wiggly", -1.9829, 111.13563),
            new StoreItem("Raley's", 5.9829, 95.13563),
            new StoreItem("Wendy's", 13.6851, 52.43515),
            new StoreItem("Five Guys", -6.9829, 23.6842),
            new StoreItem("Culver's", 41.4682, 20.6842),
            new StoreItem("White Castle", 9.1651, 120.28213),
            new StoreItem("Whataburger", -34.7128, -142.3110107)
        };

        public RetailChainsData()
        {
            if (_dbService != null)
            {
                var dbs = new OracleDbService(_connectString);
                dbs.Connect();
            }
        }

        /// You simply ask the system to provide you with a DatabaseService.

        public RetailChainsData(IDatabaseService databaseService)
        {
            _dbService = databaseService;
        }

        public void GetUsers()
        {
            _dbService.ExecuteQuery("SELECT * FROM Users");
        }
    }
}
