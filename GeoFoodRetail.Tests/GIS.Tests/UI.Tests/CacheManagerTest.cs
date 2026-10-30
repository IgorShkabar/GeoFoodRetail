/** 
 Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Regarding visibility: Since StoreData has the `internal` modifier, add the following
 * line to the `AssemblyInfo.cs` file of the main project (Buying) so that tests from 
 * the `Buying.Tests` assembly can see it:
 * using System.Runtime.CompilerServices; 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NUnit.Framework;
using GeoFoodRetail.GIS.UI;
using GeoFoodRetail.GIS.Domain.Models;
using System.Runtime.CompilerServices;


namespace GeoFoodRetail.Tests.GIS.Tests.UI.Tests
{
    [TestFixture]
    public class CacheManagerTest
    {
        [Test]
        public void Instance_CanBeCreatedWithoutException()
        {
            var cacheManager = new CacheManager();

            Assert.IsNotNull(cacheManager);
        }

        // Example of a cache test (adapt it to the actual CacheManager
        // methods, such as Get/Set or Add)
        [Test]
        public void Cache_StoresAndRetrievesData()
        {
            var cacheManager = new CacheManager();
            string key = "test_tile";
            string expectedData = "tile_data_bytes";

            cacheManager.SetLine(key, expectedData);
            var result = cacheManager.GetLine();

            Assert.AreEqual(expectedData, result);
        }

        private readonly Dictionary<Color, Pen> penCache_ = 
            new Dictionary<Color, Pen>();

        [Test]
        public void GetPen_WithSameParameters_ReturnsCachedInstance()
        {
            var cacheManager = new CacheManager();

            var pen1 = cacheManager.GetPen(Color.Red, 2f);
            var pen2 = cacheManager.GetPen(Color.Red, 2f);

            Assert.AreSame(pen1, pen2);
        }

        [Test]
        public void GetBrush_WithSameColor_ReturnsCachedInstance()
        {
            var cacheManager = new CacheManager();

            var brush1 = cacheManager.GetBrush(Color.Blue);
            var brush2 = cacheManager.GetBrush(Color.Blue);

            Assert.AreSame(brush1, brush2);
        }

        [Test]
        public void GetFont_WithSameParameters_ReturnsCachedInstance()
        {
            var cacheManager = new CacheManager();

            var font1 = cacheManager.GetFont("Arial", 12f, FontStyle.Bold);
            var font2 = cacheManager.GetFont("Arial", 12f, FontStyle.Bold);

            Assert.AreSame(font1, font2);
        }

        [TestCase("Walmart Inc.", "Target")]
        [TestCase("The Kroger Co. (KR)", "Target Corp.")]
        public void SetLine_WithStoreName_ReturnsCachedInstance(string name, string entry)
        {
            var obj = new CacheManager();

            if (!string.IsNullOrEmpty(entry) && name != entry)
            {
                var line1 = obj.SetLine(name, entry);
                var line2 = obj.SetLine(entry, name);

                Assert.AreEqual(entry, line1);
                Assert.AreEqual(name, line2);
                Assert.IsNotNull(entry, line1);
                Assert.IsNotNull(name, line2);
            }
        }

        [Test]
        public void GetLine_Returns_StoreExsample()
        {
            var obj = new CacheManager();
            string sampleData = StoreData.GetString(string.Empty);

            // First, I set the value in the cache/field.
            obj.SetLine("Wallmart Inc.", sampleData);

            // Now I receive and verify.
            var str1 = obj.GetLine();

            Assert.IsNotNull(str1);
            Assert.AreEqual(sampleData, str1);
        }        
    }
}
