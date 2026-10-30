/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * The R-Tree (and SIMD IntersectsBatch4) is a spatial index (GIS) 
 * created specifically for the instant lookup of objects on a 2D map in O(log N) time.
 * How it works interactively:
When the application starts, a window appears displaying four blue rectangles (Box 0–3) and a green frame (Query) controlled by the mouse.
When the green frame hovers over any rectangle, the IntersectsBatch4 method determines intersections with all four AABBs on the fly using a single SIMD operation.
The current bitmask is displayed in the title bar (for example, 0b0101 indicates that Box #0 and Box #2 are being intersected simultaneously).
 */
using System;
using System.IO.MemoryMappedFiles;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    /// <summary>
    /// Unmanaged fixed-size R-Tree node (optimized for a 4 KB page).
    /// </summary>

    internal unsafe struct RTreePage
    {
        private const int MaxChildren = 60; // Adjusted to fit a 4 KB page size, including the header
        
        public int Count { get; set; }
        public int IsLeaf { get; set; } // 1 = leaf, 0 = internal node

        /* Where and why 'unsafe' is used:
         Fixed-size structures: placing arrays directly inside the structure (fixed buffers).
         Array of bounding rectangles for child elements.  */

        // Buffer SoA: [0..3] min_X, [4..7] min_Y, [8..11] max_X, [12..15] max_Y
        public fixed float BoxesData[MaxChildren * 4];

        // Array of pointers/IDs of child pages or objects
        fixed long ChildrenIds[MaxChildren];
        
        /* Additional optimization (Low-Level / Zero-Allocation): I am designing a high-load
         * system utilizing SIMD and zero-allocation techniques. Pinning buffers via the 
         * `fixed (float* ptr = BoxesData)` statement on every call introduces slight 
         * overhead.
         * If I am already inside an `unsafe struct`, I can access the fixed buffer directly
         * by index without the explicit `fixed (...)` construct; this results in cleaner 
         * code and spares the JIT compiler from unnecessary pointer dereferencing during 
         * inlining.  */ 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimdAABB GetBox(int index)
        {
            
            int baseIndex = index * 4;
            return new SimdAABB
            {
                MinX = BoxesData[baseIndex],
                MinY = BoxesData[baseIndex + 1],
                MaxX = BoxesData[baseIndex + 2],
                MaxY = BoxesData[baseIndex + 3]
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetChildId(int index)
        {
            fixed (long* ptr = ChildrenIds)
            {
                return ptr[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int IntersectsBatch4(float qMinX, float qMinY, float qMaxX, float qMaxY, int startIndex = 0)
        {
            Vector<float> qMinXVec = new Vector<float>(qMinX);
            Vector<float> qMinYVec = new Vector<float>(qMinY);
            Vector<float> qMaxXVec = new Vector<float>(qMaxX);
            Vector<float> qMaxYVec = new Vector<float>(qMaxY);

            fixed (float* ptr = &BoxesData[startIndex])
            {
                Vector<float> bMinX = LoadVector4(ptr);
                Vector<float> bMinY = LoadVector4(ptr + 4);
                Vector<float> bMaxX = LoadVector4(ptr + 8);
                Vector<float> bMaxY = LoadVector4(ptr + 12);

                // SIMD comparisons (return Vector<int>)
                Vector<int> c1 = Vector.LessThanOrEqual(qMinXVec, bMaxX);
                Vector<int> c2 = Vector.GreaterThanOrEqual(qMaxXVec, bMinX);
                Vector<int> c3 = Vector.LessThanOrEqual(qMinYVec, bMaxY);
                Vector<int> c4 = Vector.GreaterThanOrEqual(qMaxYVec, bMinY);

                // combine the results using BitwiseAnd
                Vector<int> maskVec = Vector.BitwiseAnd( Vector.BitwiseAnd(c1, c2),
                                                        Vector.BitwiseAnd(c3, c4));

                // Create a 4-bit mask
                int mask = 0;
                if (maskVec[0] != 0) mask |= 1;
                if (maskVec[1] != 0) mask |= 2;
                if (maskVec[2] != 0) mask |= 4;
                if (maskVec[3] != 0) mask |= 8;
                return mask;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe Vector<float> LoadVector4(float* ptr)
        {
            Vector4 v4 = *(Vector4*)ptr;
            return new Vector<float>(new float[] { v4.X, v4.Y, v4.Z, v4.W });
        }

        // Helper method to extract a standard Rectangle from SoA
        public void GetBox(int index, out float minX, out float minY, out float maxX, out float maxY)
        {
            fixed (float* ptr = BoxesData)
            {
                minX = ptr[index];
                minY = ptr[index + 4];
                maxX = ptr[index + 8];
                maxY = ptr[index + 12];
            }
        }

        /* Adding a method to verify the boundary integrity of all boxes on the page using the 
         * .IsValid property. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidatePageBounds()
        {
            int limit = Count;
            bool isPageValid = true;

            for (int i = 0; i < limit; i++)
            {
                SimdAABB box = GetBox(i);
                if (!box.IsValid)
                {
                    isPageValid = false;
                    break;
                }
            }
            return isPageValid;
        }
    } 
}


