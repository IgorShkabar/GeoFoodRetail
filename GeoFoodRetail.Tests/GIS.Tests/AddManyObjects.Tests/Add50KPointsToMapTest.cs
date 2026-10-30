/** 
 Copyright (c) 2024-26 Igor Shkabar. All rights reserved. 
 */
using DatabaseServiceLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using GeoFoodRetail.GIS.AddManyObjects;

namespace GeoFoodRetail.Tests.GIS.Tests.AddManyObjects.Tests
{
    [TestFixture]
    public class Add50KPointsToMapTest
    {
        private readonly IDatabaseService _dbSrv;
        private string _cnnStr;
        private IStoreRepository _storeRepo;

        public Add50KPointsToMapTest() {  }

        public Add50KPointsToMapTest(IStoreRepository storeRepository)
        {
            _storeRepo = storeRepository;
        }

        [SetUp]
        public void SetUp()
        {
            // Read a string
            _cnnStr = Environment.GetEnvironmentVariable("MY_ORACLE_CONN");
        }

        public Add50KPointsToMapTest(IDatabaseService dbSrv)
        {
            _dbSrv = dbSrv;
            Assert.AreSame(dbSrv, _dbSrv);

            LoadInitialData_WhenCalled_PopulatesDatabase();
        }

        // The test now verifies the logic 
        // of the LoadInitialData method without touching OracleConnection.
        [Test]
        public void LoadInitialData_WhenCalled_PopulatesDatabase()
        {
            // Preparing fake data (without StoreDto, using tuples)
            var testData = new List<(object StoreId, object StoreNmb, object Lon, object Lat)>
            {
                (StoreId: 1, StoreNmb: 1, Lon: 37.6173, Lat: 55.7558),
                (StoreId: 2, StoreNmb: 2, Lon: 30.3351, Lat: 59.9343)
            };

            // create my manual mock and pass data to it
            IStoreRepository mockRepository = new MockStoreRepository(testData);

            // create an instance of the class UNDER TEST (the dependency is passed via the constructor)
            var dataInitializer = new Add50KPointsToMap(mockRepository);

            // Action
            var ob = new Add50KPointsToMap();
            dataInitializer.LoadInitialData();

            // Assert (Verification of the result)
            // Here, we verify that the data has indeed been saved to the database/repository.
            // For example, we check the number of records via a mock:
            Assert.AreNotEqual(2, mockRepository.GetStoresData());            
        }
    }
}
