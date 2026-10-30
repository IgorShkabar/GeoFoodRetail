/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    /// A stack-based iterator (`ref struct`) for zero-allocation search operations.

    internal unsafe ref struct SpatialStreamCursor
    {
        private readonly MemoryMappedViewAccessor _viewAccessor;
        private readonly SimdAABB                 _query_box;
        private StacksFrame*                      _stackPtr;        
        private int                               _stackTop;
      
        public  struct StacksFrame
        {
            public long PageAddress;
            public int  CurrentChildIndex;
        }

        private const int MaxDepth = 64;        

        public SpatialStreamCursor(MemoryMappedViewAccessor viewAccessor, in SimdAABB query_Box, StacksFrame* stackBuffer)
        {
            _viewAccessor = viewAccessor;
            _query_box = query_Box;
            _stackPtr = stackBuffer;
            _stackPtr = stackBuffer;
            _stackTop = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Init(long rootPageAddress)
        {
            _stackPtr[0] = new StacksFrame { PageAddress = rootPageAddress, CurrentChildIndex = 0 };
            _stackTop = 1;
        }

        /* The TryGetNext method is split into atomic private functions 
         * (TraverseTree, TryEvaluateChild, PushChildPageToStack) */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetNext(out long objectId)
        {
            objectId = 0;
            byte* pointerToMemory = null;
            _viewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointerToMemory);

            try
            {
                return TraverseTree(pointerToMemory, out objectId);
            }
            finally
            {
                _viewAccessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TraverseTree(byte* pointerToMemory, out long objectId)
        {
            objectId = 0;

            while (_stackTop > 0)
            {
                ref var frame = ref _stackPtr[_stackTop - 1];
                RTreePage* page = (RTreePage*)(pointerToMemory + frame.PageAddress);

                if (frame.CurrentChildIndex >= page->Count)
                {
                    _stackTop--;
                    continue;
                }

                int childIndex = frame.CurrentChildIndex++;
                if (TryEvaluateChild(page, childIndex, pointerToMemory, out objectId))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryEvaluateChild(RTreePage* page, int childIndex, byte* pointerToMemory, out long objectId)
        {
            objectId = 0;
            SimdAABB childBox = page->GetBox(childIndex);

            if (!childBox.Intersects(_query_box))
            {
                return false;
            }

            if (page->IsLeaf == 1)
            {
                objectId = page->GetChildId(childIndex);
                return true;
            }

            PushChildPageToStack(page, childIndex, pointerToMemory);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushChildPageToStack(RTreePage* page, int childIndex, byte* pointerToMemory)
        {
            if (_stackTop < MaxDepth)
            {
                long nextAddress = page->GetChildId(childIndex);
                _stackTop++;
                _stackPtr[_stackTop - 1] = new StacksFrame
                {
                    PageAddress = nextAddress,
                    CurrentChildIndex = 0
                };
            }
        }
    }
}