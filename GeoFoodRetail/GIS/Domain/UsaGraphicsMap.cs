/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoFoodRetail.GIS.Domain
{
    internal class UsaGraphicsMap
    {
        internal static GraphicsPath BuildUsaMapPath()
        {
            var path = new GraphicsPath();

            BuildMainContiguousTerritory(path);
            BuildAlaskaInSet(path);
            BuildHawaiiInSet(path);
            BuildTerritoriesInSet(path);

            return path;
        }
        private static void BuildMainContiguousTerritory(GraphicsPath path)
        {
            // A single closed loop: West -> Center -> Northeast -> Southeast -> South
            PointF[] mainContour = new PointF[]
            {
                // Северо-Запад (WA, OR, CA)
                new PointF(-124.7f, 48.4f),
                new PointF(-124.2f, 42.0f),
                new PointF(-120.0f, 42.0f),
                new PointF(-124.4f, 40.4f),
                new PointF(-117.1f, 32.5f),

                // Southern border (AZ, NM, TX)
                new PointF(-114.8f, 32.5f),
                new PointF(-108.2f, 31.8f),
                new PointF(-106.5f, 31.8f),
                new PointF(-104.9f, 29.8f),
                new PointF(-99.5f, 26.0f),
                new PointF(-97.1f, 26.0f),

                // The Gulf Coast and Florida (LA, MS, AL, FL)
                new PointF(-93.8f, 29.7f),
                new PointF(-89.1f, 29.1f),
                new PointF(-87.5f, 30.3f),
                new PointF(-82.8f, 27.8f),
                new PointF(-80.5f, 25.1f), // South of Florida
                new PointF(-80.1f, 26.9f),

                // The East Coast and the Northeast (NC, VA, NJ, NY, ME)
                new PointF(-75.5f, 35.2f),
                new PointF(-74.0f, 40.6f),
                new PointF(-71.0f, 42.3f),
                new PointF(-67.0f, 44.8f), // Man
                new PointF(-71.5f, 45.0f),

                // The Northern Border and the Great Lakes (NY, MI, MN, ND, MT, WA)
                new PointF(-73.7f, 45.0f),
                new PointF(-82.4f, 43.0f),
                new PointF(-84.6f, 46.5f),
                new PointF(-89.6f, 48.0f),
                new PointF(-95.2f, 49.0f),
                new PointF(-104.0f, 49.0f),
                new PointF(-116.0f, 49.0f),
                new PointF(-123.0f, 49.0f)
            };

            path.StartFigure();
            path.AddPolygon(mainContour);
        }

        private static void BuildAlaskaInSet(GraphicsPath path)
        {
            // Simplified outline of Alaska (offset inset in the bottom-left corner)
            PointF[] alaskaContour = new PointF[]
            {
                new PointF(-130.0f, 26.0f),
                new PointF(-126.0f, 31.0f),
                new PointF(-122.0f, 31.0f),
                new PointF(-118.0f, 28.0f),
                new PointF(-120.0f, 25.0f),
                new PointF(-125.0f, 24.0f)
            };

            path.StartFigure();
            path.AddPolygon(alaskaContour);
        }

        private static void BuildHawaiiInSet(GraphicsPath path)
        {
            // Islands of the Hawaiian chain (inset at the bottom)
            PointF[] islandOahu = new PointF[]
            {
                new PointF(-110.0f, 23.0f),
                new PointF(-109.5f, 23.2f),
                new PointF(-109.2f, 22.8f)
            };

            PointF[] islandMaui = new PointF[]
            {
                new PointF(-108.5f, 22.5f),
                new PointF(-108.0f, 22.7f),
                new PointF(-107.8f, 22.3f)
            };

            PointF[] islandBig = new PointF[]
            {
                new PointF(-107.0f, 21.8f),
                new PointF(-106.2f, 22.0f),
                new PointF(-106.5f, 21.2f)
            };

            path.StartFigure();
            path.AddPolygon(islandOahu);
            path.StartFigure();
            path.AddPolygon(islandMaui);
            path.StartFigure();
            path.AddPolygon(islandBig);
        }

        private static void BuildTerritoriesInSet(GraphicsPath path)
        {
            // Puerto Rico (PR)
            PointF[] puertoRico = new PointF[]
            {
                new PointF(-75.0f, 23.5f),
                new PointF(-73.5f, 23.5f),
                new PointF(-73.5f, 22.8f),
                new PointF(-75.0f, 22.8f)
            };

            path.StartFigure();
            path.AddPolygon(puertoRico);
        }
    }
}
