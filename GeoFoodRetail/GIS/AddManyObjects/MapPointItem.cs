/** 
 Copyright (c) 2026 Igor Shkabar. All Rights reserved.
 Auxiliary model for a point with coordinates X (longitude) and Y (latitude).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.AddManyObjects
{
    internal class MapPointItem
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
    }
}
