/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoFoodRetail.GIS.Projections;

namespace GeoFoodRetail.GIS.UI
{
    class MapRenderer
    {
        private readonly CacheManager _cacheManager;

        public MapRenderer(CacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void Render(Graphics g, IList<StoreItem> items, MapViewport viewport)
        {
            if (viewport == null) throw new ArgumentNullException(nameof(viewport));
            if (g == null) throw new ArgumentNullException(nameof(g));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int count = items.Count;
            StoreItem[] poolArray = ArrayPool<StoreItem>.Shared.Rent(count);

            try
            {
                for (int i = 0; i < count; i++)
                {
                    poolArray[i] = items[i];
                }

                var redBrush = _cacheManager.GetBrush(Color.Red);

                for (int i = 0; i < count; i++)
                {
                    var item = poolArray[i];
                    float screenX = (float)((item.Longitude * viewport.Zoom) + viewport.OffsetX);
                    float screenY = (float)((item.Latitude * viewport.Zoom) + viewport.OffsetY);

                    g.FillEllipse(redBrush, screenX - 4, screenY - 4, 8, 8);
                }
            }
            finally
            {
                ArrayPool<StoreItem>.Shared.Return(poolArray);
            }
        }
    }
}

