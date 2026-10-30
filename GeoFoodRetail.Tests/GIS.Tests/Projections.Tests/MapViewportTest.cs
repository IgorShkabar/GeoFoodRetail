/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GeoFoodRetail.GIS.Projections;

namespace GeoFoodRetail.Tests.GIS.Tests.Projections.Tests
{
    [TestFixture]
    public class MapViewportTest
    {
        [Test]
        public void DefaultConstructor_SetsInitialValues()
        {
            var viewport = new MapViewport();

            Assert.IsNotNull(viewport);            
        }

        [Test]
        public void Zoom_SetsAndGetsCorrectly()
        {
            var viewport = new MapViewport
            {
                Zoom = 2
            };

            Assert.AreEqual(2, viewport.Zoom);
        }
    }
}
