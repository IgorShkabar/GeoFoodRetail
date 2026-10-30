/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * Mock class for testing (MockStoreRepository)
 * The file from the Class Library (.NET Standard) project.
Instead of using mocking frameworks, I will manually write a lightweight fake class. 
It returns prepared test data directly from memory.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseServiceLib 
{
    public class MockStoreRepository : IStoreRepository
    {
        private readonly IEnumerable<(object StoreId, object StoreNmb, object Lon, object Lat)> _fakeData;

        // The constructor allows passing any test data directly from the test body
        public MockStoreRepository(IEnumerable<(object StoreId, object StoreNmb, object Lon, object Lat)> fakeData)
        {
            _fakeData = fakeData;
        }

        public IEnumerable<(object StoreId, object StoreNmb, object Lon, object Lat)> GetStoresData()
        {
            return _fakeData;
        }
    }
}
