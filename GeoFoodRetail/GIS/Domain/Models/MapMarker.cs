/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * These numbers are parameters for rendering geometry and anti-collision (label collision 
 * avoidance) margins. 
 * int size = 16: The diameter of the circular marker, in pixels on the screen.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GeoFoodRetail.GIS.Domain.Models
{
    /// <summary>
    /// size / 2f centers the point relative to the provided screen coordinates (screenPos), 
    /// ensuring that the center of the circle aligns precisely with the geographic node.
    /// `size / 2f` centers the point relative to the provided screen coordinates (`screenPos`), 
    /// ensuring that the center of the circle aligns precisely with the geographic node.
    /// The `+ 4` and `+ 2` in the calculations for `w` and `h` represent internal padding for 
    /// the text label around the name string. This prevents the text from appearing flush against 
    /// the edges of the rectangular frame.
    /// `candidateOffsets` array: A set of offset vectors relative to the center of a marker pixel. 
    /// The algorithm iterates through them in a specific order(right, bottom, left, top), checking for 
    /// overlaps with areas already occupied by other elements(`placedRects`). 
    /// If a preferred position is already taken by another piece of text, the label is shifted to a clear area.

    /// 10 and -6: The base horizontal and vertical offsets for the floating text label relative to the
    /// center of the marker point, ensuring the text does not overlap the pin icon itself.
    /// </summary>
    internal class MapMarker
    {
        // Reusable graphics resources (created once for the entire class)
        private static readonly Font SharedFont = new Font("Segoe UI", 8, FontStyle.Bold);
        private static readonly SolidBrush SharedTextBrush = new SolidBrush(Color.Black);
        private static readonly SolidBrush SharedBgBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
        private static readonly Pen SharedBorderPen = new Pen(Color.LightGray, 1);

        public MapMarker(string name, GeoCoordinate3D location, Color color, string chain = "")
        {
            Name = name;
            Location = location;            
            Color = color;
            RetailChain = chain;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public GeoCoordinate3D Location { get; set; }        
        public System.Drawing.Color Color { get; set; }
        public string RetailChain { get; set; }

        /* To eliminate "magic numbers," they are moved into named constants(using the 
         * `private const` modifier) ​​or computed properties, thereby structuring the code 
         * through configuration parameters.       */    
        public virtual void Render(Graphics g, PointF screenPos, List<Rectangle> placedRects)
        {
            // Named constants
            const int MarkerDiameter = 16;
            const int TextPaddingX = 4;
            const int TextPaddingY = 2;
            const int LabelOffsetX = 10;
            const int LabelOffsetY = -6;

            DrawMarkerCircle(g, screenPos, MarkerDiameter);
            DrawAntiCollisionLabel(g, screenPos, placedRects, TextPaddingX, TextPaddingY, LabelOffsetX, LabelOffsetY);
        }

        private void DrawMarkerCircle(Graphics g, PointF screenPos, int diameter)
        {
            var rect = new RectangleF(
                screenPos.X - diameter / 2f,
                screenPos.Y - diameter / 2f,
                diameter,
                diameter
            );
            using (var brush = new SolidBrush(Color))
            using (var pen = new Pen(Color.White, 2))
            {
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
            }
        }

        private void ForDrawAntiCollisionLabel(Graphics g, PointF screenPos, List<Rectangle> placedRects, int paddingX, int paddingY, int offsetX, int offsetY, Font font, SolidBrush textBrush, SolidBrush bgBrush, Pen borderPen, int w, int h, Point[] candidateOffsets)
        {
            Rectangle bestRect = Rectangle.Empty;
            bool placed = false;

            foreach (var offset in candidateOffsets)
            {
                Rectangle testRect = new Rectangle((int)screenPos.X + offset.X,
                                                (int)screenPos.Y + offset.Y,
                                                w,
                                                h );
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
            if (!placed)
            {
                bestRect = new Rectangle(
                    (int)screenPos.X + offsetX,
                    (int)screenPos.Y + offsetY,
                    w,
                    h );
            }
            placedRects.Add(bestRect);

            g.FillRectangle(bgBrush, bestRect);
            g.DrawRectangle(borderPen, bestRect);
            g.DrawString(Name, font, textBrush, bestRect.X + (paddingX / 2), bestRect.Y + (paddingY / 2));
        }

        private void DrawAntiCollisionLabel(Graphics g, PointF screenPos, List<Rectangle> placedRects, int paddingX, int paddingY, int offsetX, int offsetY)
        {
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var bgBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            using (var borderPen = new Pen(Color.LightGray, 1))
            {
                SizeF textSize = g.MeasureString(Name, font);
                int w = Math.Max(1, (int)Math.Ceiling(textSize.Width + paddingX));
                int h = Math.Max(1, (int)Math.Ceiling(textSize.Height + paddingY));

                Point[] candidateOffsets = new Point[]
                {
                    new Point(offsetX, offsetY),                    // Top-right
                    new Point(offsetX, Math.Abs(offsetY) + 4),      // Bottom-right
                    new Point(-w - offsetX, offsetY),               // Top-left
                    new Point(-w - offsetX, Math.Abs(offsetY) + 4), // Bottom-left
                    new Point(0, -h - 4),                           // Strictly from above
                    new Point(0, Math.Abs(offsetY) + 8)             // Strictly from below
                };

                ForDrawAntiCollisionLabel(g, screenPos, placedRects, paddingX, paddingY, offsetX, offsetY, font, textBrush, bgBrush, borderPen, w, h, candidateOffsets);
            }
        }        
    }
}
