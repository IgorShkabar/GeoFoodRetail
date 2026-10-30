/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * This struct might be redundant, as `SimdAABB` already exists.
The SIMD-accelerated packed Axis-Aligned Bounding Box and `BoundingBox` are entirely equivalent; they represent the same geometric concept. 
`SimdAABB` makes filtering 100,000 points even faster than `BoundingBox` does.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Math3D
{
    /// <summary>
    /// BoundingBox is a simple geometrycal object that describes rectangular area on a plane
    /// using the minimal and maximal coordinates(X and Y).
    /// </summary>

    internal readonly struct BoundingBox
    {        
        public BoundingBox(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public double MinX { get; }
        public double MaxX { get; }
        public double MinY { get; }
        public double MaxY { get; }

        // Point inclusion test
        public bool Contains(double x, double y)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }

        // Checking for intersection with another rectangle
        public bool Intersect(BoundingBox other)
        {
            return !(other.MinX > MaxX || other.MaxX < MinX ||
                    other.MinY > MaxY || other.MaxY < MinY);
        }
    }
}
 
