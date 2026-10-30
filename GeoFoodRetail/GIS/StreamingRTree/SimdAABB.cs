/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * A high-performance streaming R-Tree spatial index framework featuring zero-allocation 
 * support and SIMD acceleration, designed to operate via memory-mapped files (MMF).* 
 */
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
///using System.Runtime.Intrinsics;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    /// <summary>
    /// SIMD-accelerated packed Axis-Aligned Bounding Box (AABB).
    /// Structure size: 16 bytes (4 float values).
    /// </summary>

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct SimdAABB
    { 
        private const float HalfScale = 0.5f;

        // Constructor:
        /* The `[MethodImpl(MethodImplOptions.AggressiveInlining)]` 
         * line is a standard built-in attribute in the C# (.NET) programming language. 
         * It instructs the JIT compiler to inline the method at the call site to optimize 
         * performance. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimdAABB(float mX, float mY, float mxX, float mxY)
        {
            MinX = mX;
            MinY = mY;
            MaxX = mxX;
            MaxY = mxY;
        }

        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }

        /* Check for intersection with another AABB
        * An implementation of an intersection test for two Axis-Aligned Bounding Boxes (AABBs) 
        * in 2D space (X and Y axes) */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in SimdAABB other)
        {            
            return !(MinX > other.MaxX || MaxX < other.MinX ||
                        MinY > other.MaxY || MaxY < other.MinY);                           
        }
        /* For a single AABB (Axis-Aligned Bounding Box), this compact layout is more than 
         * sufficient. 
           In high-performance code (High-Performance C# / GameDev), the SimbAABB structure is 
           simply a pure data container. The smaller and simpler it is, the better the CPU cache 
           (L1/L2) performs, and the easier it is for the JIT compiler to pack it into registers.
           However, depending on the project's requirements, a few useful utility methods are 
           often added; these do not increase the structure's memory footprint (keeping it at 
           exactly 16 bytes) but make working with it more convenient.         */

        /* Does the AABB contain the point (x, y) ?
        Method essence: It checks whether a point with coordinates(x, y) lies inside 
        an axis-aligned bounding box(AABB). Check logic: The point is considered to be 
        inside if its x-coordinate falls between the rectangle's minimum and maximum x-values, 
        and its y-coordinate falls between the minimum and maximum y-values. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  bool Contains(float x, float y)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }

        // Merging two AABBs (Required when building an R-tree)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  SimdAABB Merge(in SimdAABB other)
        {
            return new SimdAABB(
                MinX < other.MinX ? MinX : other.MinX,
                MinY < other.MinY ? MinY : other.MinY,
                MaxX > other.MaxX ? MaxX : other.MaxX,
                MaxY > other.MaxY ? MaxY : other.MaxY );
        }

        // Area calculation (Required for R-Tree node-splitting heuristics)
        public float Area
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (MaxX - MinX) * (MaxY - MinY);
        }

        // Auxiliary properties
        public float Width => MaxX - MinX;
        public  float Height => MaxY - MinY;
        public  float CenterX => (MinX + MaxX) * HalfScale;
        public  float CenterY => (MinY + MaxY) * HalfScale;

        // If one of the .NET versions is not supported on the PC
#if NET48
    // Код только для классического .NET Framework (4.8 и ниже)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.Numerics.Vector<float> AsVector4()
    {            
        fixed (float* ptr = &min_X)
        {
            // Using System.Runtime.CompilerServices
             return
                System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Numerics.Vector<float>>(ptr);
        }
    }
#elif NET8_0_OR_GREATER
    // Код для современного .NET (5.0, 6.0, 7.0, 8.0, 9.0+)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.Runtime.Intrinsics.Vector128<float> AsVector128()
    {
        return System.Runtime.Intrinsics.Vector128.Create(min_X, min_Y, max_X, max_Y);
    }    
#else
        // Fallback code (for .NET Standard or .NET Core 3.1)
        // Direct conversion to SIMD Vector for hardware-accelerated batch operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector<float> AsVector128()
    {
        float[] values = new float[] { MinX, MinY, MaxX, MaxY };
            return new Vector<float>(values, 0);
    }        
#endif
        // Inflates or deflates the bounding box by a given delta (useful for buffer zones)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimdAABB Inflate(float delta)
        {
            float expandedMinX = MinX - delta;
            float expandedMinY = MinY - delta;
            float expandedMaxX = MaxX + delta;
            float expandedMaxY = MaxY + delta;

            return new SimdAABB(expandedMinX, expandedMinY, expandedMaxX, expandedMaxY);
        }

        // Computes the intersection bounding box between two AABBs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimdAABB Intersection(in SimdAABB other)
        {
            float intersectMinX = MinX > other.MinX ? MinX : other.MinX;
            float intersectMinY = MinY > other.MinY ? MinY : other.MinY;
            float intersectMaxX = MaxX < other.MaxX ? MaxX : other.MaxX;
            float intersectMaxY = MaxY < other.MaxY ? MaxY : other.MaxY;

            return new SimdAABB(intersectMinX, intersectMinY, intersectMaxX, intersectMaxY);
        }

        /* Perimeter Property (R-Tree Heuristic Optimization). 
         * 
         Essential for advanced R-Tree node splitting algorithms (like R*-Tree choose-split metrics) 
         that minimize perimeter sum alongside area overlap. 
         the fundamental heuristic principle of the R*-tree's operation.
         The goal of this heuristic is to form nodes that are as square (quadratic) as possible, 
         which drastically reduces the likelihood of their intersection during spatial queries.
         Computes the total perimeter of the AABB     */
        public float Perimeter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Width + Height) * 2.0f;
        }

        /* Squared Distance to Point (Nearest-Neighbor Pruning)
          Enables lightning-fast spatial sorting and priority queue pruning for nearest-neighbor 
          queries without calculating expensive square roots (Math.Sqrt). 
          Computes the squared Euclidean distance from a point to the nearest edge of the AABB  */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceSquaredTo(float x, float y)
        {
            float deltaX = 0.0f;
            float deltaY = 0.0f;

            if (x < MinX)
            {
                deltaX = MinX - x;
            }
            else if (x > MaxX)
            {
                deltaX = x - MaxX;
            }

            if (y < MinY)
            {
                deltaY = MinY - y;
            }
            else if (y > MaxY)
            {
                deltaY = y - MaxY;
            }

            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        /* Union Area Calculation (Insertion Cost Evaluation)
        Allows the R-Tree insertion algorithm to calculate the exact area enlargement penalty 
        when deciding which branch node to descend into, keeping allocations at zero.   */
        // Calculates the resulting area if this AABB were merged with another
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetUnionArea(in SimdAABB other)
        {
            float unionMinX = MinX < other.MinX ? MinX : other.MinX;
            float unionMinY = MinY < other.MinY ? MinY : other.MinY;
            float unionMaxX = MaxX > other.MaxX ? MaxX : other.MaxX;
            float unionMaxY = MaxY > other.MaxY ? MaxY : other.MaxY;

            return (unionMaxX - unionMinX) * (unionMaxY - unionMinY);
        }

        /* Tuple Deconstruction Pattern
        Enables modern C# tuple destruction syntax (var (minX, minY, maxX, maxY) = box;), 
        streamlining coordinate extractions in spatial algorithms without allocations. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = MinX;
            minY = MinY;
            maxX = MaxX;
            maxY = MaxY;
        }

        /* High-Performance Value Equality (IEquatable<SimbAABB>)
        Implements strict structural comparison avoiding boxing overhead, essential for caching 
        keys and deduplicating spatial node states.   */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SimdAABB other)
        {
            return MinX.Equals(other.MinX) &&
                   MinY.Equals(other.MinY) &&
                   MaxX.Equals(other.MaxX) &&
                   MaxY.Equals(other.MaxY);
        }

        /* Structural Validity Guard (IsValid)
        Protects R-Tree indices against corrupt spatial bounds, ensuring coordinate inversion 
        errors are caught instantly during serialization or file streaming. */
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MaxX >= MinX && MaxY >= MinY;
        }
    }
}

