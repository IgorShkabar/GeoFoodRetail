/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.Domain.Models;

namespace GeoFoodRetail.GIS.Domain.Layers
{
    internal class VectorLayer
    {
        internal string Name { get; set; }
        internal bool Visible { get; set; } = true;
        internal List<MapMarker> Markers { get; } = new List<MapMarker>();
        internal List<RetailStoreMarker>  MarkersofRetailStores { get; } = new List<RetailStoreMarker>();

        public VectorLayer(string name) { Name = name;  }
    }
}
