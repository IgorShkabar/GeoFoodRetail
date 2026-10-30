/**
 * Copyright (c) 2024 Igor Shkabar. All rights reserved. 
 * You need to use your mouse to create a circular selection area—specifically, 
 * one that includes at least one retailer or store.
 */
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Math3D;
using GeoFoodRetail.GIS.Projections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.Domain
{
    /// <summary>
    /// Spatial Queries: Add a "Selection by Area" tool (using a circle or polygon). 
    /// The user draws an area on the map of the U.S. with their mouse, and the application 
    /// returns the top 3 retailers in that region, the total number of locations, and the 
    /// coverage density.
    /// </summary>

    internal class AreaSpatialAnalyzer
    {
        private const float DensityScaleFactor = 1000f;
        private const float MinSelectionRadiusPx = 5f;

        public AreaSpatialAnalyzer(PointF center, PointF edge)
        {
            Center = center;
            Edge = edge;
        }

        public PointF Center { get; }
        public PointF Edge { get; }
        public int Width { get; set; }  
        public int Height { get; set; }
        public List<RetailStoreMarker> StoreMarkers { get; } = new List<RetailStoreMarker>();

        private static string ForAnalyze(float radiusPx, List<RetailStoreMarker> selectedMarkers)
        {
            int totalCount = selectedMarkers.Count;
            if (totalCount == 0)
            {
                return "There are no stores in the selected area.";
            }

            var topRetailers = selectedMarkers
                .GroupBy(m => m.RetailChain)
                .Select(g => new { Chain = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(3)
                .ToList();

            float areaPixels = (float)(Math.PI * radiusPx * radiusPx);
            float density = (totalCount / areaPixels) * DensityScaleFactor;

            string report = $"Spatial analysis results:\n\n" +
                            $"Total points: {totalCount}\n" +
                            $"Density: {density:F2} dots/1,000 px²\n\n" +
                            $"Top 3 retailers:\n";

            int rank = 1;
            foreach (var item in topRetailers)
            {
                report += $"{rank}. {item.Chain} — {item.Count} item.\n";
                rank++;
            }
            return report;
        }

        public string Analyze(List<RetailStoreMarker> storeMarkers, int cx, int cy, Func<GeoCoordinate3D, dynamic> transformFunc)
        {
            if (storeMarkers == null || storeMarkers.Count == 0)
                return null;

            float dx = Edge.X - Center.X;
            float dy = Edge.Y - Center.Y;
            float radiusPx = (float)Math.Sqrt(dx * dx + dy * dy);

            if (radiusPx < MinSelectionRadiusPx)
                return null; // To small selection

            var selectedMarkers = new List<RetailStoreMarker>();

            foreach (var marker in storeMarkers)
            {
                var pt3D = transformFunc(marker.Location);
                if (pt3D.Z <= 0) continue;

                float sx = cx + (float)pt3D.X;
                float sy = cy - (float)pt3D.Y;

                float mdx = sx - Center.X;
                float mdy = sy - Center.Y;
                if (mdx * mdx + mdy * mdy <= radiusPx * radiusPx)
                {
                    selectedMarkers.Add(marker);
                }
            }
            return ForAnalyze(radiusPx, selectedMarkers);
        }        
    }
}

