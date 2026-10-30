/**
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * I`ll create domain entities for clusters and a grouping algorithm layer.
 */
using System.Collections.Generic;
using System.Drawing;

namespace GeoFoodRetail.GIS.Domain.Models
{
    /// <summary>
    /// To implement dynamic clustering of 100,000 points—while accounting for the current zoom level
    /// & the performance requirements of Clean Architecture—we will create a spatial grouping
    /// component.
    /// It employs grid-based clustering for rapid $O(N)$ calculation without placing a load on the
    /// Garbage Collector, while also adhering to the methodology's strict limit of 30–40 lines per 
    /// method.
    /// </summary>
    
    internal class MarkerCluster
    {
        public PointF CenterPixel { get; set; }
        public GeoCoordinate3D CenterCoordinate { get; set; }
        public List<MapMarker> Markers { get; } = new List<MapMarker>();
        public float SumX { get; set; }
        public float SumY { get; set; }
        public int Count => Markers.Count;
        public bool IsSingleCluster => Count == 1;
    }
}

