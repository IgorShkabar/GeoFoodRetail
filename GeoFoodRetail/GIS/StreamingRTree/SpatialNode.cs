/** 
Copyright (c) 2024 Igor Shkabar. All rights reserved.
Spatial Node: The class representing internal tree nodes or grid cells that hold references to 
geometric shapes and child nodes.
 */
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Projections;
using GeoFoodRetail.GIS.UI;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    /* to implement Spatial Node: 
         * internal tree nodes or grid cells that hold references to geometric shapes & 
         * child nodes in the bounds of the SpatialIndexManager class   */

    internal class SpatialNode
    {
        public SimdAABB Bounds { get; }
        public bool IsLeaf { get; private set; }

        private readonly List<SpatialNode> children;
        private readonly List<MapMarker> markers;
        private const int MaxMarkersPerNode = 16;
        private const float HalfSplitFactor = 0.5f;
        /* The value 0.5f represents the quadtree subdivision scale factor used to precisely 
         divide the current node's bounding box width and height exactly in half when creating 
         its four child quadrants. */
         
        private int MarkerCount => markers.Count;
        private int ChildCount => children.Count;

        public SpatialNode(SimdAABB bounds, bool isLeaf)
        {
            Bounds = bounds;
            IsLeaf = isLeaf;
            children = new List<SpatialNode>();
            markers = new List<MapMarker>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(MapMarker marker, float x, float y)
        {
            if (!Bounds.Contains(x, y)) { return; }

            if (IsLeaf)
            {
                InsertIntoLeaf(marker);
            }
            else
            {
                InsertIntoChild(marker, x, y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertIntoLeaf(MapMarker marker)
        {
            markers.Add(marker);
            if (MarkerCount > MaxMarkersPerNode)
            {
                Subdivide();
            }
        }

        public void Query(in SimdAABB query_box, int zoom, List<MapMarker> results)
        {
            bool is_intersecting = Bounds.Intersects(query_box);
            if (!is_intersecting) { return; }

            if (IsLeaf)
            {
                EvaluateLeafMarkers(query_box, zoom, results);
            }
            else
            {
                EvaluateChildNodes(query_box, zoom, results);
            }
        }

        /* These lines of code create new nodes for a spatial tree (most likely a quadtree or 
         * octree) that bounds a specific region of space.  */
        private void CreateNodes_forSpatialTree()
        {
            float half_width = Bounds.Width * HalfSplitFactor;
            float half_height = Bounds.Height * HalfSplitFactor;
            float mid_X = Bounds.MinX + half_width;
            float mid_Y = Bounds.MinY + half_height;

            /* From the perspective of high-performance, 
             * zero-allocation development, there is a nuance: the expression `children.
             * AddRange(new[] { ... })` creates a temporary array on the heap (a heap allocation) 
             * every time `Subdivide` is called.
             * 
            children.AddRange(new[]
            {
                new SpatialNode(new SimdAABB(Bounds.min_X, Bounds.min_Y, mid_X, mid_Y), true),
                new SpatialNode(new SimdAABB(mid_X, Bounds.min_Y, Bounds.max_X, mid_Y), true),
                new SpatialNode(new SimdAABB(Bounds.min_X, mid_Y, mid_X, Bounds.max_Y), true),
                new SpatialNode(new SimdAABB(mid_X, mid_Y, Bounds.max_X, Bounds.max_Y), true)
            });   
            It is better to eliminate this micro-allocation entirely by calling `children.Add` 
            4 times in a row. The compiler and JIT will effectively optimize this into an inlined 
            sequence with no memory allocation. */

            children.Add(new SpatialNode(new SimdAABB(Bounds.MinX, Bounds.MinY, mid_X, mid_Y), true));
            children.Add(new SpatialNode(new SimdAABB(mid_X, Bounds.MinY, Bounds.MaxX, mid_Y), true));
            children.Add(new SpatialNode(new SimdAABB(Bounds.MinX, mid_Y, mid_X, Bounds.MaxY), true));
            children.Add(new SpatialNode(new SimdAABB(mid_X, mid_Y, Bounds.MaxX, Bounds.MaxY), true));
        }

        private void Subdivide()
        {
            IsLeaf = false;
            CreateNodes_forSpatialTree();   // DRY(Don't Repeat Yourself)
            RedistributeExistingMarkers();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateChildQuadrants()
        {
            CreateNodes_forSpatialTree();   // DRY(Don't Repeat Yourself)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RedistributeExistingMarkers()
        {
            int total_markers = MarkerCount;
            for (int i = 0; i < total_markers; i++)
            {
                MapMarker marker = markers[i];
                PointF pixel_pos = MercatorProjection.LatLonToPixel(marker.Location, 0);
                InsertIntoChild(marker, pixel_pos.X, pixel_pos.Y);
            }
            markers.Clear();
        }

        private void InsertIntoChild(MapMarker marker, float x, float y)
        {
            int total_children = ChildCount;
            for (int i = 0; i < total_children; i++)
            {
                SpatialNode child = children[i];
                if (child.Bounds.Contains(x, y))
                {
                    child.Insert(marker, x, y);
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EvaluateLeafMarkers(in SimdAABB query_box, int zoom, List<MapMarker> results)
        {
            int total_markers = MarkerCount;
            for (int i = 0; i < total_markers; i++)
            {
                MapMarker marker = markers[i];
                PointF pixel_pos = MercatorProjection.LatLonToPixel(marker.Location, zoom);

                if (query_box.Contains(pixel_pos.X, pixel_pos.Y))
                {
                    results.Add(marker);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EvaluateChildNodes(in SimdAABB query_box, int zoom, List<MapMarker> results)
        {
            int total_children = ChildCount;
            for (int i = 0; i < total_children; i++)
            {
                SpatialNode child = children[i];
                child.Query(query_box, zoom, results);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(MapMarker marker, float x, float y)
        {
            if (!Bounds.Contains(x, y)) { return false; }
            if (IsLeaf)
            {
                return RemoveFromLeaf(marker);
            }
            return RemoveFromChild(marker, x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemoveFromLeaf(MapMarker marker)
        {
            return markers.Remove(marker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemoveFromChild(MapMarker marker, float x, float y)
        {
            int total_children = ChildCount;
            for (int i = 0; i < total_children; i++)
            {
                SpatialNode child = children[i];
                if (child.Bounds.Contains(x, y))
                {
                    return child.Remove(marker, x, y);
                }
            }
            return false;
        }

        public void QueryRange(in SimdAABB query_box, int zoom, List<MapMarker> results)
        {
            bool isIntersecting = Bounds.Intersects(query_box);
            if (!isIntersecting) { return; }
            if (IsLeaf)
            {
                EvaluateLeafRangeMarkers(query_box, zoom, results);
            }
            else
            {
                EvaluateChildRanges(query_box, zoom, results);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EvaluateLeafRangeMarkers(in SimdAABB query_box, int zoom, List<MapMarker> results)
        {
            int total_markers = MarkerCount;
            for (int i = 0; i < total_markers; i++)
            {
                MapMarker marker = markers[i];
                PointF pixelPos = MercatorProjection.LatLonToPixel(marker.Location, zoom);
                SimdAABB markerBox = new SimdAABB(pixelPos.X, pixelPos.Y, pixelPos.X, pixelPos.Y);

                if (query_box.Intersects(markerBox))
                {
                    results.Add(marker);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EvaluateChildRanges(in SimdAABB query_box, int zoom, List<MapMarker> results)
        {
            int total_children = ChildCount;
            for (int i = 0; i < total_children; i++)
            {
                SpatialNode child = children[i];
                child.QueryRange(query_box, zoom, results);
            }
        }

        /* Support methods inside the nested SpatialNode class to allow secure access &
         * State queries required by Bulk Load, Rebalance, and Optimize:  */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMarkerCount()
        {
            return MarkerCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<MapMarker> GetAllMarkers()
        {
            return new List<MapMarker>(markers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetChildCount()
        {
            return ChildCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpatialNode GetChild(int index)
        {
            return children[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveChildAt(int index)
        {
            children.RemoveAt(index);
        }

        /* Operational EssentialsThread Safety: 
         * Implements locks (ReaderWriterLockSlim) to allow safe concurrent reads &
         * synchronized updates in multi-threaded environments.Custom Metrics: Properties to expose the total item count, tree depth, and node distribution for performance tuning.
         * metrics calculation methods*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetTotalItemCount()
        {
            if (IsLeaf)
            {
                return MarkerCount;
            }

            int count = 0;
            int total_children = ChildCount;
            for (int i = 0; i < total_children; i++)
            {
                count += children[i].GetTotalItemCount();
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDepth()
        {
            if (IsLeaf || ChildCount == 0) { return 1; }

            int max_child_depth = 0;
            int total_children = ChildCount;
            for (int i = 0; i < total_children; i++)
            {
                int childDepth = children[i].GetDepth();
                if (childDepth > max_child_depth)
                {
                    max_child_depth = childDepth;
                }
            }
            return 1 + max_child_depth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CalculateNodeDistribution(ref int leaf_count, ref int internal_count)
        {
            if (IsLeaf)
            {
                leaf_count++;
            }
            else
            {
                internal_count++;
                int total_children = ChildCount;
                for (int i = 0; i < total_children; i++)
                {
                    children[i].CalculateNodeDistribution(ref leaf_count, ref internal_count);
                }
            }
        }
    }    
}
