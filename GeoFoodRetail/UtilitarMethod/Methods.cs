/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.UtilitarMethod
{
    /** Fixing the Math.Clamp error
     * If the project does not support Math.Clamp, write your own micro-function */
    public static class Methods
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        /* The AggressiveInlining attribute will force the compiler to inline this code directly, 
         * ensuring there is no performance loss (which is important for SIMD). */
    }
}
