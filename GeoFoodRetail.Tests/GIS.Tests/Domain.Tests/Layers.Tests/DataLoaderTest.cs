/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 */
using NUnit.Framework;
using GeoFoodRetail.GIS;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using GeoFoodRetail.GIS.Domain.Layers;
using GeoFoodRetail.GIS.UI;

namespace GeoFoodRetail.Tests.GIS.Tests.Domain.Tests.Layer.Tests
{
    public class DataLoaderTest
    {
        [SetUp]
        public void Setup()
        {
            /* If there is no need to initialize shared variables, contexts, 
             * or objects before running each test, this method and the [SetUp] 
             * attribute can be safely removed to avoid cluttering the code with
             * empty stubs.*/
        }

        [Test]
        public void LoadParallel_WithValidData_ReturnsAllItems()
        {
            // Preparing test data
            var sourceData = Enumerable.Range(1, 1000).Select(i => new StoreItem
            {
                Name = $"Walmart Inc. {i}",
                Latitude = i * 0.01,
                Longitude = i * 0.01
            }).ToList();
           
            var result = DataLoader.LoadParallel(sourceData);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(sourceData.Count));
            Assert.That(result.First().Name, Is.EqualTo("Walmart Inc. 1"));
            Assert.That(result.Last().Name, Is.EqualTo("Walmart Inc. 1000"));
        }

        [Test]
        public void LoadParallel_WithEmptyCollection_ReturnsEmptyList()
        {
            // Preparing empty collection
            var sourceData = new List<StoreItem>();
            
            var result = DataLoader.LoadParallel(sourceData);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}
