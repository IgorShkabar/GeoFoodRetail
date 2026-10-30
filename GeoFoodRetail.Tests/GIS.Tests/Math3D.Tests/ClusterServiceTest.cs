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
    public class ClusterServiceTest
    {
        [Test]
        public void ClusterPoints_WithPointsInSameAndDifferentCells_GroupsThemCorrectly()
        {
            var points = new List<PointF>
            {
                new PointF(10f, 10f),
                new PointF(12f, 11f), // Two points in one cell
                new PointF(50f, 50f)  // 1 point in the sepatate cell
            };

            const double gridStep = 20.0;
            const int expectedTotalClusters = 2;
            const int expectedItemsInDenseCluster = 2;
            const int expectedItemsInSparseCluster = 1;
            const int expectedClusterMatchCount = 1;

            var clusterService = new ClusterService();

            var clusters = clusterService.ClusterPoints(points, gridStep);

            Assert.That(clusters, Is.Not.Null);
            Assert.That(clusters.Count, Is.EqualTo(expectedTotalClusters));

            int denseClusterMatches = 0;
            int sparseClusterMatches = 0;

            foreach (var cluster in clusters)
            {
                if (cluster.Count == expectedItemsInDenseCluster) denseClusterMatches++;
                if (cluster.Count == expectedItemsInSparseCluster) sparseClusterMatches++;
            }

            Assert.That(denseClusterMatches, Is.EqualTo(expectedClusterMatchCount));
            Assert.That(sparseClusterMatches, Is.EqualTo(expectedClusterMatchCount));
        }

        [Test]
        public void ClusterPoints_WithEmptyList_ReturnsEmptyResult()
        {
            var points = new List<PointF>();
            const double gridStep = 20.0;
            var clusterService = new ClusterService();

            var clusters = clusterService.ClusterPoints(points, gridStep);

            Assert.That(clusters, Is.Not.Null);
            Assert.That(clusters, Is.Empty);
        }
    }
}
