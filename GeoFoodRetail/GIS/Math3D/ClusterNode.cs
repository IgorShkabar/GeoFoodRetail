/** 
 Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 ClusterNode is a class that helps group nearby points into clusters.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GeoFoodRetail.GIS.Math3D
{
    internal class ClusterNode
    {
        public PointF Center { get; set; }
        public int Count { get; set; }
    }
}
