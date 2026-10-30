/**
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using GeoFoodRetail.GIS.Math3D;


namespace GeoFoodRetail.Tests.GIS.Tests.Math3D.Tests
{
    public class ClusterNodeTest
    {
        [Test]
        public void ClusterNode_Properties_SetAndGetCorrectly()
        {
            const float testLat = 35.5f;
            const float testLon = 45.2f;
            const int expectedCount = 15;

            var centerPoint = new PointF(testLat, testLon);

            var clusterNode = new ClusterNode
            {
                Center = centerPoint,
                Count = expectedCount
            };

            Assert.That(clusterNode.Center, Is.EqualTo(centerPoint));
            Assert.That(clusterNode.Count, Is.EqualTo(expectedCount));
        }
    }
}
