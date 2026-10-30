/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Mathematical core: Conversion of geographic coordinates (Lat/Lon) to 
 * 3D Cartesian coordinates on a sphere, & Euler angle/quaternion matrix rotations.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Math3D;
using static System.Math; 

namespace GeoFoodRetail.GIS.Projections
{    
    internal static class SphericalProjection
    {
        /** If GIS-folk are familiar with "magical numbers" like 90.0, 180.0, 360.0, and etc. 
         * & are tired of reading incredibly long constant names, I use these "magic numbers": 
         * 180.0 is Half Revolution Degrees (half-turn degrees); 360.0 - is Degrees of full rotation (Full Revolution Degrees;
         * PI / 180.0 is Degrees to Radians Factor; */

        public static Point3D GeoToCartesian(GeoCoordinate3D coord, double radius)
        {
            // latitude (in radians), longtitude (in radians)

            double latInRadians = coord.Latitude * PI / 180.0;
            double lonInRadians = coord.Longitude * PI / 180.0;

            double x = radius * Cos(latInRadians) * Cos(lonInRadians);
            double y = radius * Sin(latInRadians);
            double z = radius * Cos(latInRadians) * Sin(lonInRadians);

            return new Point3D(x, y, z);
        }

        public static Point3D RotateY(Point3D p, double angleRad)
        {
            double cos = Cos(angleRad);
            double sin = Sin(angleRad);
            return new Point3D(p.X * cos + p.Z * sin, p.Y, -p.X * sin + p.Z * cos);
        }

        public static Point3D RotateX(Point3D p, double angleRad)
        {
            double cos = Cos(angleRad);
            double sin = Sin(angleRad);
            return new Point3D(p.X, p.Y * cos - p.Z * sin, p.Y * sin + p.Z * cos);
        }

        /// <summary>
        /// Calculating the sampling point (UV coordinates) on an equirectangular world map. 
        /// U: Longitude [-180..180] -> [0..Texture.Width] 
        /// V: Latitude [-90..90] -> [0..Texture.Height]
        /// </summary>
       
        public static PointF GeoToUV(double lat, double lon, int textureWidth, int textureHeight)
        {
            float u = (float)((lon + 180.0) / 360.0 * textureWidth);
            float v = (float)((90.0 - lat) / 180.0 * textureHeight);
            return new PointF(u, v);
        }
    }
}
