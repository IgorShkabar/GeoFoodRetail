/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Contract for obtaining tiles.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Abstractions
{
    /// Tile Service & Disk/Memory Caching

    internal interface ITileProvider
    {
        Task<Image> GetTileAsync(int x, int y, int zoom);
    }
}
