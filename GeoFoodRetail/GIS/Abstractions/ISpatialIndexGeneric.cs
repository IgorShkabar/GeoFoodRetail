/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * Add the NetTopologySuite NuGet package to your project 
 * before optimizing more than 100,000 points on the map.
 * Interface for working with an 
   Envelope-based spatial index (NetTopologySuite) without unnecessary allocations.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using GeoFoodRetail.GIS.StreamingRTree;

namespace GeoFoodRetail.GIS.Abstractions
{
    public interface ISpatialIndex<T>
    {
        void Insert(Envelope envelope, T item);
        void Insert(in SimdAABB bounds, T item);        
        void Build();
        void ClearIndex();
        IList<T> Query(Envelope searchEnv);
    }
}
