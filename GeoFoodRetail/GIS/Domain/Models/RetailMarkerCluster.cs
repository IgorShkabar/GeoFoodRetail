/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * I`ll create domain entities for retail location clusters and a grouping algorithm layer.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Domain.Models
{
    internal class RetailMarkerCluster
    {
        public PointF CenterPixelForRetail { get; set; }
        public GeoCoordinate3D RetailCenterCoordinate { get; set; }
        public List<RetailStoreMarker> RetailMarkers { get; } = new List<RetailStoreMarker>();
        public float SumX { get; set; }
        public float SumY { get; set; }
        public int RetailCount => RetailMarkers.Count;
        public bool IsRetailSingleCluster => RetailCount == 1;
    }
}
