/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved. 
 * Architectural Role of the ISpatialIndex Interface.
The ISpatialIndex interface encapsulates spatial search mechanisms, 
isolating application controllers and forms from the low-level details of traversing 
R-tree nodes.
Class Implementation: Implement this interface using an adapter class, RTreeSpatialIndex, or 
refactor the existing SpatialIndexManager class so that it formally inherits from ISpatialIndex.
Core logic: The `AddMarker` method calculates a bounding box (SimbAABB) for a `MapMarker`
object and inserts it into your custom R-tree structure. The `QueryRegion` method delegates 
the task to a high-performance `SpatialStreamCursor` class and employs batched intersection 
checks—utilizing SIMD instructions (`IntersectsBatch4`)—to rapidly filter for markers that 
intersect with the active viewing area. 
An advantage: Should the need arise in the future to replace your custom R-tree implementation 
(which uses memory mapping) with a quadtree (stored in RAM) or another type of spatial index, 
this can be done seamlessly—you simply need to create a new class that implements 
`ISpatialIndex`, without requiring any changes to the `DeliveryZoneForm` code or the user 
interface rendering logic. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.StreamingRTree;

namespace GeoFoodRetail.GIS.Abstractions
{
    interface ISpatialIndex
    {
        /// Registers a map marker into the spatial indexing structure.

        void AddMarker(MapMarker marker);

        /// Performs a spatial range query using an Axis-Aligned Bounding Box (AABB) 
        /// optimized for the current zoom level.

        List<MapMarker> QueryRegion(SimdAABB queryBox, int currentZoom);
    }
}