/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GeoFoodRetail.GIS.Domain.Models;

namespace GeoFoodRetail.GIS.Domain.Models
{
    class RetailStoreMarker : MapMarker
    {
        // Domain Model(Retail & Food Stores)
        internal enum StoreType { Retail, Grocery, Supermarket }

        public RetailStoreMarker(string name, 
                                string chain, 
                                GeoCoordinate3D location, 
                                StoreType type, 
                                Color color)
        : base(name, location, color, chain)
        {
            Name = name;
            RetailChain = chain;
            Location = location;
            Type = type;
            BrandColor = color;
        }

        public StoreType Type { get; set; }
        public Color BrandColor { get; set; }
                
        public override void Render(Graphics g, PointF screenPos, List<Rectangle> placedRects)
        {
            // 1. First, we draw the dot itself (it is not affected by collisions)
            // Diameter of the point marker on the map, in pixels
            const int MarkerDiameter = 16;
            const int TextPaddingX = 2;
            const int TextPaddingY = 1;
            const float BorderWidth = 1f;
            const int MinBoxSize = 1;

            var rect = 
                new RectangleF( screenPos.X - MarkerDiameter / 2f, screenPos.Y - MarkerDiameter / 2f, MarkerDiameter, MarkerDiameter );

            using (var brush = new SolidBrush(BrandColor))
            using (var pen = new Pen(Color.White, 2))
            {
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
            }

            // Anti-collision logic for label text
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var bgBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            using (var borderPen = new Pen(Color.LightGray, BorderWidth))
            {
                SizeF textSize = g.MeasureString(Name, font);

                // Text width + double indentation (left + right)
                int w = Math.Max(MinBoxSize, (int)Math.Ceiling(textSize.Width + (TextPaddingX * 2)));

                // Text height + double indentation (top + bottom)
                int h = Math.Max(MinBoxSize, (int)Math.Ceiling(textSize.Height + (TextPaddingY * 2)));

                Point[] candidateOffsets = new Point[]
                {
                    new Point(10, -6),       // On the right
                    new Point(10, 10),       // Bottom-right
                    new Point(-w - 10, -6),  // On the left
                    new Point(-w - 10, 10),  // Bottom left
                    new Point(0, -h - 10),   // On the top
                    new Point(0, 12)         // on the bottom
                };
                ForRender(g, screenPos, placedRects, font, textBrush, bgBrush, borderPen, w, h, candidateOffsets);
            }
        }

        private void ForRender(Graphics g, 
                            PointF screenPos, 
                            List<Rectangle> placedRects, 
                            Font font, 
                            SolidBrush textBrush, 
                            SolidBrush bgBrush, 
                            Pen borderPen, 
                            int w, 
                            int h, 
                            Point[] candidateOffsets)
        {
            // Constants for internal text margins from the rectangle boundaries
            const int TextPaddingX = 2;
            const int TextPaddingY = 1;
            var bestRect = Rectangle.Empty;
            bool placed = false;

            foreach (var offset in candidateOffsets)
            {
                var testRect = new Rectangle((int)screenPos.X + offset.X, (int)screenPos.Y + offset.Y, w, h);

                bool collision = false;
                foreach (var existing in placedRects)
                {
                    if (testRect.IntersectsWith(existing))
                    {
                        collision = true;
                        break;
                    }
                }
                if (!collision)
                {
                    bestRect = testRect;
                    placed = true;
                    break;
                }
            }

            if (!placed && candidateOffsets.Length > 0)
            {
                var defaultOffset = candidateOffsets[0];
                bestRect = new Rectangle((int)screenPos.X + defaultOffset.X, (int)screenPos.Y + defaultOffset.Y, w, h);
            }
            placedRects.Add(bestRect);
            // Drawing the background and text
            g.FillRectangle(bgBrush, bestRect);
            g.DrawRectangle(borderPen, bestRect);
            g.DrawString(Name, font, textBrush, bestRect.X + TextPaddingX, bestRect.Y + TextPaddingY);
        }
    }
}
