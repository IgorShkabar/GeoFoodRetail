/**
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Cluster Models & Logic: Cluster Engine Implementation
   See below after the source code. 
   I am implementing a clustering service that aggregates markers based on the size of the screen 
   pixel window (grid size), which depends on the zoom level.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using GeoFoodRetail.GIS.Domain.Models;
using System.Drawing;
using GeoFoodRetail.GIS.Projections;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal class SpatialClusterEngine
    {
        private const float BaseClusterGridSize = 64.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<MarkerCluster> BuildClusters(IEnumerable<MapMarker> markers, int currentZoom, float viewWidth, float viewHeight, PointF screenOffset)
        {
            var clusterMap = new Dictionary<long, MarkerCluster>();
            float adaptiveGridSize = CalculateAdaptiveGridSize(currentZoom);

            foreach (var marker in markers)
            {
                PointF worldPixel = MercatorProjection.LatLonToPixel(marker.Location, currentZoom);
                PointF screenPos = new PointF(worldPixel.X + screenOffset.X, worldPixel.Y + screenOffset.Y);

                if (screenPos.X < -50 || screenPos.Y < -50 || screenPos.X > viewWidth + 50 || screenPos.Y > viewHeight + 50)
                {
                    continue;
                }

                long cellKey = ComputeGridCellKey(screenPos.X, screenPos.Y, adaptiveGridSize);
                AppendMarkerToClusterMap(clusterMap, cellKey, marker, screenPos);
            }

            return ResolveClusterCenters(clusterMap);
        }
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalculateAdaptiveGridSize(int zoom)
        {
            int zoomDelta = zoom > 3 ? zoom - 3 : 1;
            return BaseClusterGridSize / zoomDelta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long ComputeGridCellKey(float pixelX, float pixelY, float gridSize)
        {
            long cellX = (long)(pixelX / gridSize);
            long cellY = (long)(pixelY / gridSize);
            return (cellX << 32) ^ cellY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendMarkerToClusterMap(Dictionary<long, MarkerCluster> map, long key, MapMarker marker, PointF screenPos)
        {
            if (!map.TryGetValue(key, out var cluster))
            {
                cluster = new MarkerCluster();
                map[key] = cluster;
            }
            cluster.Markers.Add(marker);
            cluster.SumX += screenPos.X;
            cluster.SumY += screenPos.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<MarkerCluster> ResolveClusterCenters(Dictionary<long, MarkerCluster> map)
        {
            var resultClusters = new List<MarkerCluster>(map.Count);
            foreach (var kvp in map)
            {
                var cluster = kvp.Value;
                int count = cluster.Markers.Count;
                cluster.CenterPixel = new PointF(cluster.SumX / count, cluster.SumY / count);
                cluster.CenterCoordinate = cluster.Markers[0].Location;
                resultClusters.Add(cluster);
            }
            return resultClusters;
        }
    }
}

