/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved. 
 * Add a helper cluster class. 
 Screen-Space Grid Clustering is the fastest method for GDI+; 
 it groups nearby points—those within a 30-pixel radius on the screen—into a single cluster 
 displaying the number of markers contained within it.
*/

using System.Collections.Generic;
using System.Drawing;

namespace GeoFoodRetail.GIS.Domain.Models
{
    /// <summary>
    /// For 100,000 points:
    /// Implement a spatial index(e.g., QuadTree): 
    /// To locate points within the current screen view in milliseconds.

    /// Clustering algorithm: Group nearby points into a single cluster displaying a count
    /// (e.g., "54 stores") and render only the clusters and individual points that fall within
    /// the aggregation radius.

    /// Zoom-based rendering: When the user views the globe from a distance, display only
    /// large clusters organized by region or country; as the user zooms in (reducing the sphere radius),
    /// reveal details down to individual stores.
    /// </summary>

    internal class StoreCluster
    {
        private const int MaxSingleMarkerCount = 1;

        public PointF ScreenPosition { get; set; }
        public List<RetailStoreMarker> StoreClusterMarkers { get; } = new List<RetailStoreMarker>();

        public bool IsCluster => StoreClusterMarkers.Count > MaxSingleMarkerCount;
    }    
}
