/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * SpatialIndexManager (high-performance nearest-marker search)   
 */
using System.Collections.Generic;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Projections;
using System.Drawing;
using System.Runtime.CompilerServices;
using GeoFoodRetail.GIS.Abstractions;
using GeoFoodRetail.GIS.UI;
using System;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal partial class SpatialIndexManager<T> : ISpatialIndex<T>
    {
        /** Some abbreviations: mrk is abbreviation of marker;  cntrX  - of center X; res - of results; m - of 
           totMrks - of total markers; pxPos - of pixel position; minX - of minimum X coordinate, for Y, Z - the same; maxX and maxY - the
           same for maximum X, maximum Y; totRes - of total results; delX - delta X; crrDistSquared - of current distance squared; 
           isRemoved - of is removed; targX - of target x & the same with targY; */
        
        private const float   DefaultWorldMinX = 0.0f;
        private const float   DefaultWorldMinY = 0.0f;
        private const float   DefaultWorldMaxX = 16384.0f;
        private const float   DefaultWorldMaxY = 16384.0f;
        private SpatialNode   _rootNode;
        private readonly List<MapMarker> _indexedMarkers = new List<MapMarker>();

        public SpatialIndexManager()
        {
            SimdAABB worldBounds = CreateDefaultWorldBounds();
            _rootNode = new SpatialNode(worldBounds, isLeaf: true);
        }

        public SpatialIndexManager(SimdAABB customBounds)
        {
            _rootNode = new SpatialNode(customBounds, isLeaf: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimdAABB CreateDefaultWorldBounds()
        {
            return new SimdAABB(DefaultWorldMinX, DefaultWorldMinY, DefaultWorldMaxX, DefaultWorldMaxY);
        }

        public void Initialize(SimdAABB bounds)
        {
            ConfigureRootNode(bounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ConfigureRootNode(in SimdAABB bounds)
        {
            // Re-initialization or clearing logic for structural bounds setup
            var newNode = new SpatialNode(bounds, isLeaf: true);
            // Internal replacement can be wired if mutability is desired
        }

        public void AddMarker(MapMarker mrk)
        {
            _indexedMarkers.Add(mrk);
        }

        public void AddMarker(MapMarker mrk, int crrZoom)
        {
            lock_.EnterWriteLock();
            try
            {
                PointF pxPos = MercatorProjection.LatLonToPixel(mrk.Location, crrZoom);
                _rootNode.Insert(mrk, pxPos.X, pxPos.Y);
            }
            finally   {   lock_.ExitWriteLock();   }            
        }

        public List<MapMarker> QueryRegion(SimdAABB queryBox, int currentZoom)
        {
            lock_.EnterReadLock();
            try
            {
                var results = new List<MapMarker>();
                _rootNode.Query(queryBox, currentZoom, results);
                return results;
            }
            finally    {   lock_.ExitReadLock();   }
        }        

        /* Integration of an allocation-free method for the rapid search of the nearest object 
         * based on quadratic distance. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MapMarker FindNearestMarker(float trgX, float trgY, int crrZoom)
        {
            MapMarker nearestMrk = null;
            float     minDistanceSquared = float.MaxValue;
            int       totMrks = _indexedMarkers.Count;

            for (int i = 0; i < totMrks; i++)
            {
                MapMarker candidate = _indexedMarkers[i];
                PointF    pxPos = MercatorProjection.LatLonToPixel(candidate.Location, crrZoom);

                float   deltaX = pxPos.X - trgX;
                float   deltaY = pxPos.Y - trgY;
                float   currDistanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

                if (currDistanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = currDistanceSquared;
                    nearestMrk = candidate;
                }
            }
            return nearestMrk;
        }

        /* Bulk Insertion (AddRange)
        Optimizes memory allocation by allowing batch ingestion of retail store collections 
        before spatial indexing operations. */
        public void AddRange(IEnumerable<MapMarker> markers)
        {
            foreach (var m in markers)
            {
                AddMarker(m);
            }
        }

        /* Proximity Radius Query (QueryRadius)
         Filters and returns all retail markers falling within a specific screen-pixel radius 
         from a designated center coordinate, essential for delivery zone radius checks. */
        public List<MapMarker> QueryRadius(float cntrX, float cntrY, float radiusPxls, int crrZoom)
        {
            var   res = new List<MapMarker>();    
            float radiusSquared = radiusPxls * radiusPxls;
            int   totMrks = _indexedMarkers.Count;  

            for (int i = 0; i < totMrks; i++)
            {
                MapMarker mr = _indexedMarkers[i];  
                PointF pxPos = MercatorProjection.LatLonToPixel(mr.Location, crrZoom);

                float  deltaX = pxPos.X - cntrX;
                float  deltaY = pxPos.Y - cntrY;
                float  distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

                if (distanceSquared <= radiusSquared)
                {
                    res.Add(mr);
                }
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimdAABB CreateSearchBoxAroundTarget(float trgX, float trgY)
        {
            const float SearchRadiusOffset = 100.0f;

            float minX = trgX - SearchRadiusOffset;
            float minY = trgY - SearchRadiusOffset;
            float maxX = trgX + SearchRadiusOffset;
            float maxY = trgY + SearchRadiusOffset;

            return new SimdAABB(minX, minY, maxX, maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private MapMarker EvaluateNearestCandidate(List<MapMarker> res, float targX, float targY, int crrZoom)
        {
            MapMarker   nearestMrk = null;
            float       minDistanceSquared = float.MaxValue;
            int         totRes = res.Count;

            for (int i = 0; i < totRes; i++)
            {
                MapMarker candidate = res[i];
                PointF    pxPos = MercatorProjection.LatLonToPixel(candidate.Location, crrZoom);

                float   delX = pxPos.X - targX;
                float   delY = pxPos.Y - targY;
                float   crrDistSquared = (delX * delX) + (delY * delY);

                if (crrDistSquared < minDistanceSquared)
                {
                    minDistanceSquared = crrDistSquared;
                    nearestMrk = candidate;
                }
            }
            return nearestMrk;
        }

        // Remove / Delete
        public bool RemoveMarker(MapMarker mr, int crrZoom)
        {
            PointF pxPos = ResolveMarkerPixelPosition(mr, crrZoom);
            return _rootNode.Remove(mr, pxPos.X, pxPos.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PointF ResolveMarkerPixelPosition(MapMarker mr, int crrZoom)
        {
            PointF pxPos = MercatorProjection.LatLonToPixel(mr.Location, crrZoom);
            return pxPos;
        }

        /* Update: Re-indexes an item when its geometry or position changes. 
         * Typically combines a Remove and an Insert operation. */
        public void UpdateMarker(MapMarker mr, GeoCoordinate3D oldLoc, int crrZoom)
        {
            PointF oldPixelPos = ResolveOldPixelPosition(oldLoc, crrZoom);
            bool isRemoved = _rootNode.Remove(mr, oldPixelPos.X, oldPixelPos.Y);

            PointF newPixelPos = ResolveNewPixelPosition(mr, crrZoom);
            _rootNode.Insert(mr, newPixelPos.X, newPixelPos.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PointF ResolveOldPixelPosition(GeoCoordinate3D oldLoc, int crrZoom)
        {
            PointF oldPixelPos = MercatorProjection.LatLonToPixel(oldLoc, crrZoom);
            return oldPixelPos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PointF ResolveNewPixelPosition(MapMarker mr, int crrZoom)
        {
            PointF newPixelPos = MercatorProjection.LatLonToPixel(mr.Location, crrZoom);
            return newPixelPos;
        }

        /* Range / Envelope Query: 
         * Retrieves all items whose bounding boxes intersect with a specified query bounding box. */
        public List<MapMarker> QueryRange(SimdAABB queryBox, int crrZoom)
        {
            var res = new List<MapMarker>();
            _rootNode.QueryRange(queryBox, crrZoom, res);
            return res;
        }

        public List<MapMarker> QueryPoint(float pointX, float pointY, int crrZoom)
        {
            var res = new List<MapMarker>();
            SimdAABB pointBox = CreatePointQueryBox(pointX, pointY);
            _rootNode.QueryRange(pointBox, crrZoom, res);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimdAABB CreatePointQueryBox(float x, float y)
        {
            const float PointQueryEpsilon = 0.001f;

            float minX = x - PointQueryEpsilon;
            float minY = y - PointQueryEpsilon;
            float maxX = x + PointQueryEpsilon;
            float maxY = y + PointQueryEpsilon;

            return new SimdAABB(minX, minY, maxX, maxY);
        }

        public List<MapMarker> FindKNearestMarkers(float targX, float targY, int kCount, int crrZoom)
        {
            var allMarkers = new List<MapMarker>();
            SimdAABB massiveSearchBox = CreateMassiveSearchBounds();
            _rootNode.QueryRange(massiveSearchBox, crrZoom, allMarkers);

            SortMarkersByDistance(allMarkers, targX, targY, crrZoom);
            return ExtractTopKMarkers(allMarkers, kCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimdAABB CreateMassiveSearchBounds()
        {
            const float MaxBoundsCoordinate = 1000000.0f;
            // Дальнейшая логика метода с использованием MaxBoundsCoordinate...
            return new SimdAABB(-MaxBoundsCoordinate, -MaxBoundsCoordinate, MaxBoundsCoordinate, MaxBoundsCoordinate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SortMarkersByDistance(List<MapMarker> markerList, float targX, float targY, int zoom)
        {
            markerList.Sort((mrkA, mrkB) =>
            {
                float distanceA = ComputeSquaredMarkerDistance(mrkA, targX, targY, zoom);
                float distanceB = ComputeSquaredMarkerDistance(mrkB, targX, targY, zoom);
                return distanceA.CompareTo(distanceB);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ComputeSquaredMarkerDistance(MapMarker mrk, float targX, float targY, int zoom)
        {
            PointF pixPos = MercatorProjection.LatLonToPixel(mrk.Location, zoom);
            float deltaX = pixPos.X - targX;
            float deltaY = pixPos.Y - targY;
            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        /* The letter k originates from the classic K-Nearest Neighbors (KNN) algorithm notation,
         * where $k$ universally represents the number of nearest neighbors to retrieve. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<MapMarker> ExtractTopKMarkers(List<MapMarker> sortedList, int kCount)
        {
            int takeCount = sortedList.Count < kCount ? sortedList.Count : kCount;
            return sortedList.GetRange(0, takeCount);
        }

        public List<MapMarker> QueryRayIntersection(float startX, float startY, float endX, float endY, int crrZoom)
        {
            var res = new List<MapMarker>();
            SimdAABB rayBoundingBox = CreateRayBoundingBox(startX, startY, endX, endY);
            var candidateMarkers = new List<MapMarker>();

            _rootNode.QueryRange(rayBoundingBox, crrZoom, candidateMarkers);
            FilterMarkersIntersectingRay(candidateMarkers, startX, startY, endX, endY, crrZoom, res);
            return res;
        }
    }
}

