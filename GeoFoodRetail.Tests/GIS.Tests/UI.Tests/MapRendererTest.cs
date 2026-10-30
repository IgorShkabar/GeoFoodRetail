/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 Add the System.Drawing.Common NuGet package to the Buying.Tests project 
 (or add the corresponding assembly via the package manager), since the 
 MapRenderer class accepts or uses types from this library.
 Install the NuGet package System.Buffers (matching version 4.0.5 or higher) 
 in your test project (Buying.Tests), as the rendering or clustering engine 
 requires assembly binding resolution for buffer pooling dependencies.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using NUnit.Framework;
using GeoFoodRetail.GIS.UI;
using GeoFoodRetail.GIS.Projections;

namespace GeoFoodRetail.Tests.GIS.Tests.UI.Tests
{
    public class MapRendererTest
    {
        [Test]
        public void Render_WithValidItemsAndViewport_ExecutesWithoutException()
        {
            // Arrange
            var cacheManager = new CacheManager(/* pass the parameters if required, or mock them */);
            var renderer = new MapRenderer(cacheManager);
            var viewport = new MapViewport();
            var items = new List<StoreItem>(); // Use the actual StoreItem type instead of MapItem

            // Act & Assert
            using (var bitmap = new Bitmap(800, 600))
            using (var g = Graphics.FromImage(bitmap))
            {
                Assert.DoesNotThrow(() => renderer.Render(g, items, viewport));
            }
        }
    }
}
