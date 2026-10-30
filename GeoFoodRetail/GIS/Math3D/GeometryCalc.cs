/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.Math3D
{
    internal class GeometryCalc
    {
        internal static double CalculateHaversineDistance(PointF pt1, PointF pt2)
        {
            double dLat = (pt2.X - pt1.X) * Math.PI / 180.0;
            double dLon = (pt2.Y - pt1.Y) * Math.PI / 180.0;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(pt1.X * Math.PI / 180.0) * Math.Cos(pt2.X * Math.PI / 180.0) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return
                Constants.EarthConst.RadiusInKilometers * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        internal static PointF LatLonToWebMercator(float lat, float lon)
        {
            double x = lon * 20037508.34 / 180.0;
            double y = Math.Log(Math.Tan((90.0 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            y = y * 20037508.34 / 180.0;
            return NormalizeMercatorToScreen((float)x, (float)y);
        }

        internal static PointF NormalizeMercatorToScreen(float mx, float my)
        {
            float screenX = (mx + 14000000f) / 18000f;
            float screenY = (6000000f - my) / 18000f;
            return new PointF(screenX, screenY);
        }
    }
}
