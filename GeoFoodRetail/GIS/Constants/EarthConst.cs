/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Taking the Earth's radius into a common constant that is accessible to 
 * the entire project is the correct approach.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Constants
{
    /// <summary>
    /// Public geographic constants.
    /// </summary>
    
    internal static class EarthConst
    {
        /// <summary>
        /// Average radius of the Earth in kilometers.
        /// </summary>

        internal const double RadiusInKilometers = 6371.0088;

        /// <summary>
        /// Average radius of the Earth in meters.
        /// </summary>

        internal const double RadiusInMeters = 6371009.0;
    }
}
