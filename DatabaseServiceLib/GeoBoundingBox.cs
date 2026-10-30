/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Since my data will be loaded into various grids across different forms and folders, 
 * I need to make `GeoBoundingBox` and `GetVisibleStores(GeoBoundingBox bbox)` public 
 * & include them in the library. The logic should be moved to a shared library.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseServiceLib
{
    /// The very structure of boundaries

    public struct GeoBoundingBox
    {
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }

        public GeoBoundingBox(double minLat, double maxLat, double minLong, double maxLong)
        {
            MinLatitude = minLat;
            MaxLatitude = maxLat;
            MinLongitude = minLong;
            MaxLongitude = maxLong;
        }
    }
}
