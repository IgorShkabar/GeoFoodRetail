/** 
 Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace GeoFoodRetail.GIS.Projections
{
    internal class MapViewport
    {
        public int Zoom { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public Envelope VisibleEnvelope { get; set; }
    }
}
