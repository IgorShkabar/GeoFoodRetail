/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Clustering mode for high zoom levels; individual rendering via R-Tree when zoomed in.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Domain.Models
{
    public enum DetailLevel
    {
        Clustered, // Clustering mode for long-range zooms
        Detailed   // Individual-item rendering via R-Tree during zooming
    }
}
