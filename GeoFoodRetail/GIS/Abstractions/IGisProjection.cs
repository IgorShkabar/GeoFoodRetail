/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Lat/Lon <-> Pixel Conversion
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Math3D;

namespace GeoFoodRetail.GIS.Abstractions
{
    interface IGisProjection
    {
        /// Projects geographic coordinates (EPSG:4326) to 
        /// Web Mercator projected coordinates (EPSG:3857 in meters).
        
        Point3D Project(GeoCoordinate3D coordinate);

        /// Unprojects Web Mercator coordinates (EPSG:3857 in meters) 
        /// back to geographic coordinates (EPSG:4326).

        GeoCoordinate3D Unproject(Point3D projectedPoint);
    }
}