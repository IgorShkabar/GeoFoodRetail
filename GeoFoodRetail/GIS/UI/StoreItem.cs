/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.Domain.Models;

namespace GeoFoodRetail.GIS.UI
{
    public class StoreItem
    { 
        public StoreItem() { }
        public StoreItem( string nm, double lat, double lon)
        {
            Name = nm;
            Latitude = lat;
            Longitude = lon;            
        }

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }        
        /// public GeoCoordinate3D Location { get; set; } <= for Fallback calculation formula based on StoreItem entities

        public int Id { get; set; } // for [Test] ClearnIndex_ShouldClearInternalSpatialIndex()
    }
}
