/** 
 The SpatialNode<T> must be обязательно struct but not class for 100_000 objects. 
 A massive structure is allocated in memory as a single contiguous block, which is ideal for the processor cache and does not place a burden on the garbage collector (GC). 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.UI;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal  struct SpatialNode<T>
    {
        public SpatialNode(float x, float y, T data) : this()
        {
            X = x;
            Y = y;
            Data = data;
        } 

        public float X { get; }
        public float Y { get; }
        public  T Data { get; } // A link to my object (e.g., a class of points on a map)
        public StoreItem Item { get; set; }  // for [Test] ClearnIndex_ShouldClearInternalSpatialIndex()
    }
}
