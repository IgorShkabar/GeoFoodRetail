/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * 
 * Architecture level: Core / Math
 * Role & area of responsibility: Calculation service. 
 * It transforms geo-coordinates to pixels ($EPSG:3857$). * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;   // Import static properties of Math

namespace GeoFoodRetail.GIS.Projections
{
    /// <summary>
    /// It was possible to use numerical values ​​and write more concise code, e. g.:

    /// double x = (coord.Longitude + 180.0) / 360.0;
    /// double sin_lat = Math.Sin(coord.Latitude * Math.PI / 180.0);
    /// double y = 0.5 - Math.Log((1 + sin_lat) / (1 - sin_lat)) / (4 * Math.PI);

    /// but then they might appear "magical" to some people.
    /// I hope the word "PI" is known worldwide and needs no explanation.
    /// </summary>

    internal class MercatorProjection
    {
        private const double DegreesToRadiansFactor = PI / 180.0;
        private const double RadiansToDegreesFactor = 180.0 / PI;

        private const double HalfRevolutionDegrees = 180.0;
        private const double FullRevolutionDegrees = 360.0;
        private const double MaxLatitudeDegrees = 90.0;

        /** the `MrcVerDenomScale` constant is Mercator Vertical Denominator Scale */

        private const double NormalizedEquatorOffset = 0.5;
        private const double MrcVerDenomScale = 4.0 * PI;
        private const double InverseMercatorScale = 2.0 * PI;

        /** In mathematical formulas, the number one (1 or 1.0) is usually 
         * not singled out as a constant, as it is a standard element of algebra.*/

        public static PointF LatLonToPixel(GIS.Domain.Models.GeoCoordinate3D coord, int zoom, int tile_size = 256)
        {
            double map_size = tile_size * (1 << zoom);

            // Longtitude normalization: [0..1]
            double x = (coord.Longitude + HalfRevolutionDegrees) / FullRevolutionDegrees;

            // sine of latitude  
            double sinLat = Sin(coord.Latitude * DegreesToRadiansFactor);

            // Mercator projection for the vertical Y-coordinate
            double y = 
                NormalizedEquatorOffset - Log((1 + sinLat) / (1 - sinLat)) / MrcVerDenomScale;

            return new PointF((float)(x * map_size), (float)(y * map_size));
        }

        public static GIS.Domain.Models.GeoCoordinate3D PixelToLatLon(PointF pixel, int zoom, int tile_size = 256)
        {
            double map_size = tile_size * (1 << zoom);
            double x = pixel.X / map_size - NormalizedEquatorOffset;
            double y = NormalizedEquatorOffset - pixel.Y / map_size;

            double lon = FullRevolutionDegrees * x;
            double lat =
                MaxLatitudeDegrees - 
                FullRevolutionDegrees * Atan( Exp(-y * InverseMercatorScale)) / PI;

            return new GIS.Domain.Models.GeoCoordinate3D(lat, lon);
        }
    }
}


