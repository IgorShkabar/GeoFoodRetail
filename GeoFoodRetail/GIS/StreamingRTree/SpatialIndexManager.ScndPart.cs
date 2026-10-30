/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * SpatialIndexManager.ScndPart.cs is the second partial class for (high-performance nearest-mrk search)   
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using GeoFoodRetail.GIS.Abstractions;
using GeoFoodRetail.GIS.Domain.Models;
using System.Drawing;
using GeoFoodRetail.GIS.Projections;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal partial class SpatialIndexManager<T> : ISpatialIndex<T>
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimdAABB CreateRayBoundingBox(float start_x, float start_y, float end_x, float end_y)
        {
            float min_x = start_x < end_x ? start_x : end_x;
            float min_y = start_y < end_y ? start_y : end_y;
            float max_x = start_x > end_x ? start_x : end_x;
            float max_y = start_y > end_y ? start_y : end_y;

            return new SimdAABB(min_x, min_y, max_x, max_y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FilterMarkersIntersectingRay(List<MapMarker> candidates, float start_x, float start_y, float end_x, float end_y, int zoom, List<MapMarker> res)
        {
            int tot_candidates = candidates.Count;
            for (int i = 0; i < tot_candidates; i++)
            {
                MapMarker mrk = candidates[i];
                PointF px_pos = MercatorProjection.LatLonToPixel(mrk.Location, zoom);

                if (IsPointNearLineSegment(px_pos.X, px_pos.Y, start_x, start_y, end_x, end_y))
                {
                    res.Add(mrk);
                }
            }
        }

        /* lin_sgm_threshold is abbreviation of line segment threshold; sgmt_len_squared - of segment lenght squared; 
         interpolation_prm - of interpolation parameter; clamped_prm - of clamped Parameter;
         separat_x - of separation X and separat_y - similarly; */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPointNearLineSegment(float px, float py, float x1, float y1, float x2, float y2)
        {
            float lin_sgm_threshold = 2.0f;
            float sgmt_len_squared = ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));

            if (sgmt_len_squared == 0.0f)
            {
                float delta_x = px - x1;
                float delta_y = py - y1;
                return ((delta_x * delta_x) + (delta_y * delta_y)) <= (lin_sgm_threshold * lin_sgm_threshold);
            }

            float interpolation_prm = ((px - x1) * (x2 - x1) + (py - y1) * (y2 - y1)) / sgmt_len_squared;
            float clamped_prm = interpolation_prm < 0.0f ? 0.0f : (interpolation_prm > 1.0f ? 1.0f : interpolation_prm);

            float nearest_x = x1 + clamped_prm * (x2 - x1);
            float nearest_y = y1 + clamped_prm * (y2 - y1);
            float separat_x = px - nearest_x;
            float separat_y = py - nearest_y;

            return ((separat_x * separat_x) + (separat_y * separat_y)) <= (lin_sgm_threshold * lin_sgm_threshold);
        }

        public void BulkLoad(IEnumerable<MapMarker> markers, int crr_zoom)
        {
            var collectedMarkers = new List<MapMarker>(markers);
            SimdAABB computed_bounds = ComputeMarkersBoundingBox(collectedMarkers, crr_zoom);

            _rootNode = new SpatialNode(computed_bounds, isLeaf: true);
            InsertMarkersInBatch(collectedMarkers, crr_zoom);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimdAABB ComputeMarkersBoundingBox(List<MapMarker> markers, int zoom)
        {
            int tot_mrks = markers.Count; // tot_mrks is abbrv. of total markers
            if (tot_mrks == 0)
            {
                return new SimdAABB(0.0f, 0.0f, 1000.0f, 1000.0f);
            }
            MapMarker firstMarker = markers[0];
            PointF frst_pos = MercatorProjection.LatLonToPixel(firstMarker.Location, zoom);
            float min_x = frst_pos.X;
            float min_y = frst_pos.Y;
            float max_x = frst_pos.X;
            float max_y = frst_pos.Y;

            for (int i = 1; i < tot_mrks; i++)
            {
                PointF pos = MercatorProjection.LatLonToPixel(markers[i].Location, zoom);
                min_x = min_x < pos.X ? min_x : pos.X;
                min_y = min_y < pos.Y ? min_y : pos.Y;
                max_x = max_x > pos.X ? max_x : pos.X;
                max_y = max_y > pos.Y ? max_y : pos.Y;
            }
            return new SimdAABB(min_x, min_y, max_x, max_y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertMarkersInBatch(List<MapMarker> markers, int zoom)
        {
            int tot_mrks = markers.Count;
            for (int i = 0; i < tot_mrks; i++)
            {
                MapMarker mrk = markers[i];
                PointF px_pos = MercatorProjection.LatLonToPixel(mrk.Location, zoom);
                _rootNode.Insert(mrk, px_pos.X, px_pos.Y);
            }
        }

        /* extracted_mrks is abbrv. of extracted markers; */

        public void Rebalance(int crr_zoom)
        {
            var extracted_mrks = new List<MapMarker>();
            ExtractAllMarkersFromNode(_rootNode, crr_zoom, extracted_mrks);

            SimdAABB global_bounds = _rootNode.Bounds;
            _rootNode = new SpatialNode(global_bounds, isLeaf: true);
            InsertMarkersInBatch(extracted_mrks, crr_zoom);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExtractAllMarkersFromNode(SpatialNode node, int zoom, List<MapMarker> collection)
        {
            if (node.IsLeaf)
            {
                collection.AddRange(node.GetAllMarkers());
            }
            else
            {
                int tot_children = node.GetChildCount();
                for (int i = 0; i < tot_children; i++)
                {
                    ExtractAllMarkersFromNode(node.GetChild(i), zoom, collection);
                }
            }
        }

        public void Optimize(int crr_zoom)
        {
            PurgeEmptyTreeBranches(_rootNode, crr_zoom);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool PurgeEmptyTreeBranches(SpatialNode node, int zoom)
        {
            if (node.IsLeaf)
            {
                return node.GetMarkerCount() == 0;
            }
            int tot_children = node.GetChildCount();

            for (int i = tot_children - 1; i >= 0; i--)
            {
                SpatialNode child_node = node.GetChild(i);
                bool should_remove_child = PurgeEmptyTreeBranches(child_node, zoom);
                if (should_remove_child)
                {
                    node.RemoveChildAt(i);
                }
            }
            return node.GetChildCount() == 0;
        }

        /** Thread Safety: Implements locks (ReaderWriterLockSlim) to allow safe concurrent reads 
         * & synchronized updates in multi-threaded environments.Custom Metrics: Properties to 
         * expose the total item count, tree depth, and node distribution for performance tuning.
         */
        private readonly System.Threading.ReaderWriterLockSlim lock_ = new System.Threading.ReaderWriterLockSlim();

        public void Dispose()
        {
            lock_.Dispose();
        }

        public int TotalItemCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                lock_.EnterReadLock();
                try
                {
                    return _rootNode != null ? _rootNode.GetTotalItemCount() : 0;
                }
                finally { lock_.ExitReadLock(); }
            }
        }

        public int TreeDepth
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                lock_.EnterReadLock();
                try
                {
                    return _rootNode != null ? _rootNode.GetDepth() : 0;
                }
                finally { lock_.ExitReadLock(); }
            }
        }

        public string GetNodeDistributionMetrics()
        {
            lock_.EnterReadLock();
            try
            {
                int leaf_count = 0;
                int internal_count = 0;
                if (_rootNode != null)
                {
                    _rootNode.CalculateNodeDistribution(ref leaf_count, ref internal_count);
                }
                return $"Leaf nodes: {leaf_count}, Internal nodes: {internal_count}";
            }
            finally { lock_.ExitReadLock(); }
        }

        /* State Reset (Clear)
          Provides clean teardown capabilities when clearing the map view or reloading streaming 
          spatial data inside DeliveryZoneForm.  */
        public void Clear()
        {
            _indexedMarkers.Clear();
        }
    }
}
