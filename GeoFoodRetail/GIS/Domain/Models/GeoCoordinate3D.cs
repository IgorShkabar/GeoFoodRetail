/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Structure.
Architecture Layer: Domain (Model)
A core data entity. Stores latitude/longitude ($\text{Lat/Lon}$).

If the issue is strictly that the signs are being flipped (a $+$ becomes a $-$, or vice versa), 
then the inversion is occurring during the process of saving to or reading from `GeoCoordinate3D`.
This can be fixed locally and safely either within the constructor or inside the `GeoCoordinate3D` 
class itself by implementing sign normalization at the property level:
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Domain.Models
{
    /// Core Geo & Math (Web Mercator EPSG:3857)

    internal struct  GeoCoordinate3D
    {
        public GeoCoordinate3D(double lat, double lon, double altitude = 0)
        {            
            Latitude = lat;
            Longitude = lon;
            Altitude = altitude;            
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }        
        public double Altitude { get; set; }        
    }
}

