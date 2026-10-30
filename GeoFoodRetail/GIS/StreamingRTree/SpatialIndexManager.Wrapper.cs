/** 
 Copyright (c) 2026 Igor Shkabar. All right reserved.
 The C# Spatial Index Manager class handles the creation, maintenance, and querying of 
 * geometric data to enable fast spatial searches.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using GeoFoodRetail.GIS.Abstractions;
using GeoFoodRetail.GIS.Math3D;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal partial class SpatialIndexManager<T> : ISpatialIndex<T>
    {
        /// For another partial class
        
        private readonly STRtree<T>     _rtree;
        private bool                    _isBuilt;
        private readonly int            _capacity; // Maximum number of points in one tree cell (usually 32-64
        private readonly SimdAABB       _boundary; // Границы текущего узла
        private List<SpatialNode<T>>     _points;    
        private SpatialIndexManager<T>[]  _children; // Subtrees
        private readonly ISpatialIndex<T> spa_i;     // Spatial index
        private bool                      _is_split;
                

        public SpatialIndexManager(int capacity = 32)
        {
            _rtree = new STRtree<T>(capacity);
            _isBuilt = false;
            _children = null;
        }

        // 32 (or Capacity) is the capacity threshold for a single node (leaf) in the tree.
        // This number determines how many max points can be stored in 1 node before this node
        // "bursts" and splits into 4 new quadrants (Split method.
        public SpatialIndexManager(SimdAABB boundary, int capacity = 32)
        {
            _boundary = boundary;
            _capacity = capacity;
            _points = new List<SpatialNode<T>>();
            _is_split = false;
        }

        /** Creates a constructor that stores a pre-built spatial index structure 
         * (ISpatialIndex<T>) to enable fast point lookups. Purpose of this code: 
         * Initialization: Stores the provided index in a class field for future use.
         * Error protection: Ensures the index is not null (throws an ArgumentNullException); 
         * otherwise, the program execution is halted immediately.
         * Integration with rendering: Prepares the manager for filtering. Before rendering, the engine 
         * will query this index to retrieve only those points that fall within the visible screen area.
         * How it all works together: The method receives the visible screen boundaries (a rectangle or viewport). 
         * It accesses the stored spatialIndex and performs a fast query (for example, GetPointsInBounds).
         * The graphics engine receives an array containing only the visible points (e.g., 500 points instead of 
         * 100,000). 
         * Only the necessary elements are rendered on the screen, which speeds up the program.        */

        public SpatialIndexManager(ISpatialIndex<T> spatialIndex)
        {
            _rtree = new STRtree<T>(); // initialize the tree
            spa_i = spatialIndex ?? throw new ArgumentNullException(nameof(spatialIndex));
        }

        // Clears the spatial index before a full rebuild
        public void ClearIndex()
        {
            spa_i.ClearIndex();
        }

        // Adds one object, along with its bounding box, to the index
        public void Insert(in SimdAABB bounds, T item)
        {
            spa_i.Insert(in bounds, item);
        }

        /** Adds a new object, along with its boundaries, to an R-Tree spatial index, preparing the data for rapid map-based searching prior to the rendering stage.
         _rtree.Insert(envelope, item); Adds an item and its bounding box (envelope) to the R-Tree structure.
          The R-Tree organizes objects based on the principle of nested regions. This enables the retrieval of points on a map in minimal time, without the need to iterate through the entire dataset.
         _isBuilt = false; Resets the index readiness flag.
         This signals to the program that new data has been added to the structure, indicating that—if necessary—the tree may need to be optimized or rebalanced before any search operations are performed.   */

        public void Insert(Envelope envelope, T item)
        {
            _rtree.Insert(envelope, item);
            _isBuilt = false;
        }

        // Adding a point/element to the index
        public void Insert(T item, float x, float y)
        {
            // We check whether the point falls within the boundaries of the current node
            if (!_boundary.Contains(x, y))
                return;

            var node = new SpatialNode<T>(x, y, item);

            // If the node is already split, we move the point further down the tree.
            if (_is_split)
            {
                foreach (var child in _children)
                {
                    child.Insert(item, x, y);
                    return;
                }
            }
            _points.Add(node);
            // If capacity is exceeded, split the node
            // If the limit of 32 points has been exceeded and we haven't split yet — "burst"
            if (_points.Count > _capacity && !_is_split) { Split(); }
        }

        // Split: Node splitting on 4 quadrant
        private void Split()
        {
            float midX = (_boundary.MinX + _boundary.MaxX) / 2f;
            float midY = (_boundary.MinY + _boundary.MaxY) / 2f;

            _children = new SpatialIndexManager<T>[4];
            // [0] Nort-hwest (Top-Left)
            _children[0] = new SpatialIndexManager<T>(new SimdAABB(_boundary.MinX, midY, midX, _boundary.MaxY));
            // [1] North-east (Top-Right)
            _children[1] = new SpatialIndexManager<T>(new SimdAABB(midX, midY, _boundary.MaxX, _boundary.MaxY));
            // [2] South-west (Bottom-Left)
            _children[2] = new SpatialIndexManager<T>(new SimdAABB(_boundary.MinX, _boundary.MinY, midX, midY));
            // [3] South-east (Bottom-Right)
            _children[3] = new SpatialIndexManager<T>(new SimdAABB(midX, _boundary.MinY, _boundary.MaxX, midY));
        }

        // This implementation removes a point from the current node or, if the node has already been split (`_is_split`), 
        // recursively searches for it within the subtrees.
        public void Remove(T item, SimdAABB itemBoundary)
        {
            // If the point being removed does not intersect the boundaries of the current node, I exit.
            if (!_boundary.Intersects(itemBoundary)) return;
            
            if (_is_split)
            {
                // If the node is split, I propagate the deletion to the child subtrees.
                if (_children != null)
                {
                    for (int i = 0; i < _children.Length; i++)
                    {
                        _children[i].Remove(item, itemBoundary);
                    }
                }
            }
            else
            {   
                // If this is a leaf node, find and remove the point from the list
                if (_points != null)
                {
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (EqualityComparer<T>.Default.Equals(_points[i].Data, item))
                        {
                            _points.RemoveAt(i);
                            break; // Удаляю только первое вхождение
                        }
                    }
                }
            }
        }

        // Clears all dynamic data (_points, subtrees _children), resets the tree's state 
        // flags, and returns the instance to its initial ("empty") state, ready for reuse.
        public void Clean()
        {            
            if (_points != null)
            {
                _points.Clear();
                // Optional: you can assign null if you want to completely free up memory
                // _points = null;
            }
            // Recursively clear and remove child subtrees
            if (_children != null)
            {
                for (int i = 0; i < _children.Length; i++)
                {
                    if (_children[i] != null)
                    {
                        _children[i].Clean();
                        _children[i] = null; // Free up the reference for GC
                    }
                }
                _children = null;
            }
            // Reset the logical state flags
            _is_split = false;
            _isBuilt = false;

            // _rtree (STRtree) is typically recreated from scratch whenever the tree is rebuilt,
            // since the standard STRtree in NetTopologySuite does not provide a Clear() method.
            // If necessary, the reference can be set to null:
            // _rtree = null; // (This would require removing the 'readonly' modifier from the _rtree field)
        }

        public void Build()
        {
            if (!_isBuilt)
            {
                _rtree.Build();
                _isBuilt = true;
            }
        }

        public IList<T> Query(Envelope searchEnv)
        {
            if (!_isBuilt)
                Build();
            return _rtree.Query(searchEnv);
        }
    }
}

