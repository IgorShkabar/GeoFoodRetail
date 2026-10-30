/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Math3D;
using GeoFoodRetail.GIS.Projections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class Gis3DGlobeControl : Control
    {
        private void MarkForOnPaint(PointF screenOffset, out System.Diagnostics.Stopwatch sw, out List<MarkerCluster> clusters)
        {
            var allMarkers = new List<MapMarker>();
            if (Layers != null)
            {
                foreach (var layer in Layers.Where(l => l.Visible))
                {
                    if (layer.MarkersofRetailStores != null)
                    {
                        // Now polymorphism works: RetailStoreMarker is implicitly cast to MapMarker
                        allMarkers.AddRange(layer.MarkersofRetailStores);
                    }
                }
            }
            if (StoreMarkers != null)
            {
                foreach (var store in StoreMarkers)
                {
                    if (store is MapMarker marker)
                    {
                        allMarkers.Add(marker);
                    }
                }
            }
            // to output the actual execution time, use a measurement with Stopwatch:            
            sw = System.Diagnostics.Stopwatch.StartNew();
            clusters = _clusterEngine.BuildClusters(allMarkers, Zoom, Width, Height, screenOffset);
            sw.Stop();
        }

        private void RenderingsForOnPaint(PaintEventArgs e)
        {
            // Get current screen and zoom settings
            float viewWidth = this.Width;
            float viewHeight = this.Height;
            // Group markers into clusters using the engine
            // At the end of the OnPaint / drawing method:
            if (_isSelecting)
            {
                float dx = _selectCurrent.X - _selectStart.X;
                float dy = _selectCurrent.Y - _selectStart.Y;
                float radiusPx = (float)Math.Sqrt(dx * dx + dy * dy);

                using (var pen = new Pen(Color.Cyan, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                using (var brush = new SolidBrush(Color.FromArgb(40, 0, 200, 255)))
                {
                    e.Graphics.FillEllipse(brush, _selectStart.X - radiusPx, _selectStart.Y - radiusPx, radiusPx * 2, radiusPx * 2);
                    e.Graphics.DrawEllipse(pen, _selectStart.X - radiusPx, _selectStart.Y - radiusPx, radiusPx * 2, radiusPx * 2);
                }
            }
        }

        private void RenderToGraphicContextOfForm(PaintEventArgs e, Graphics g, PointF screenOffset)
        {
            MarkForOnPaint(screenOffset, out System.Diagnostics.Stopwatch sw, out List<MarkerCluster> clusters);

            if (clusters.Count != _lastLoggedCount)
            {
                // In the OnPaint rendering loop: To log the State of the clusters directly during 
                // redrawing so that they supplement the general log stream:
                _lastLoggedCount = clusters.Count;
                GlobeLogger.Log($"API request... Received in {sw.ElapsedMilliseconds}ms");
                GlobeLogger.Log($"Rendered {clusters.Count} clusters");
            }
            /// it`s clones msg-s:  AccessorLogClusters(clusters.Count);
            
            using (var font = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.White, 2.0f))
            {
                DrawClusters(g, clusters, font, brush, pen);
            }
            RenderingsForOnPaint(e);
            DrawDebugConsole(e.Graphics);
        }
                
        protected override void OnPaint(PaintEventArgs e)
        {
            int centerX = Width / 2;
            int centerY = Height / 2;

            base.OnPaint(e);
            Graphics g = e.Graphics;
            // Enable maximum smoothing for GDI+ graphics
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Draw the outer cosmic glow
            DrawAtmosphereGlow(g, centerX, centerY, (float)_globeRadius);
           
            RenderTexturedGlobeMesh(g, centerX, centerY);            
            DrawStoreMarkers(g, centerX, centerY);            
            /// RenderGlobe(g, _currentRadius);

            // Draw Clustered Vector Layers & Retail Markers
            PointF centerPixel = MercatorProjection.LatLonToPixel(Center, Zoom);
            PointF screenOffset = new PointF(Width / 2f - centerPixel.X, Height / 2f - centerPixel.Y);

            RenderToGraphicContextOfForm(e, g, screenOffset);
        }

        private void DrawAtmosphereGlow(Graphics g, int cx, int cy, float radius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(cx - radius - 20, cy - radius - 20, (radius + 20) * 2, (radius + 20) * 2);
                using (var pathBrush = new PathGradientBrush(path))
                {
                    pathBrush.CenterColor = Color.FromArgb(80, 0, 180, 255);
                    pathBrush.SurroundColors = new[] { Color.Transparent };
                    g.FillEllipse(pathBrush, cx - radius - 25, cy - radius - 25, (radius + 25) * 2, (radius + 25) * 2);
                }
            }
        }

        private void MakeTexture(Graphics g, int cx, int cy)
        {
            // take the ocean color from the top-left corner of the texture as the "transparent" color
            Color oceanColor = WorldTexture.GetPixel(0, 0);
            
            int scanStep = 2;   // шаг сканирования УСТАНОВИ в 2

            using (var landPen = new Pen(Color.FromArgb(200, 0, 180, 255), 1.0f))
            {
                for (int y = 0; y < WorldTexture.Height; y += scanStep)
                {
                    for (int x = 0; x < WorldTexture.Width; x += scanStep)
                    {
                        Color texColor = WorldTexture.GetPixel(x, y);

                        // If it’s not the ocean, then it’s land 
                        if (ColorDistance(texColor, oceanColor) > 50) 
                        {                            
                            double lon = (x / (double)WorldTexture.Width) * 360.0 - 180.0;
                            double lat = 90.0 - (y / (double)WorldTexture.Height) * 180.0;
                                                        
                            Point3D p3D = Transform(new GeoCoordinate3D(lat, lon));
                            if (p3D.Z > 0)
                            {
                                float sx = (float)(cx + p3D.X);
                                float sy = (float)(cy - p3D.Y);
                                
                                g.DrawLine(landPen, sx, sy, sx + 1, sy);
                            }
                        }
                    }
                }
            }
        }

        private void RenderTexturedGlobeMesh(Graphics g, int cx, int cy)
        {
            if (WorldTexture == null) return;

            float r = (float)_globeRadius;
            
            using (GraphicsPath clipPath = new GraphicsPath())
            {
                clipPath.AddEllipse(cx - r, cy - r, r * 2, r * 2);
                g.SetClip(clipPath);
            }
            MakeTexture(g, cx, cy);
            g.ResetClip();
            
            using (var borderPen = new Pen(Color.FromArgb(100, 0, 180, 255), 1.5f))
            {
                g.DrawEllipse(borderPen, cx - r, cy - r, r * 2, r * 2);
            }
        }

        // Helper function for comparing colors
        private double ColorDistance(Color c1, Color c2)
        {
            return Math.Sqrt(Math.Pow(c1.R - c2.R, 2) + Math.Pow(c1.G - c2.G, 2) + Math.Pow(c1.B - c2.B, 2));
        }


        private void DrawStoreMarkers(Graphics g, int cx, int cy)
        {
            if (StoreMarkers == null || StoreMarkers.Count == 0)    return;

            List<Rectangle> placedRects = new List<Rectangle>();

            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            using (var bgBrush = new SolidBrush(Color.FromArgb(200, 15, 23, 42)))
            {
                foreach (var marker in StoreMarkers)
                {
                    var pt3D = Transform(marker.Location);
                    if (pt3D.Z <= 0) continue; // Culling

                    float screenX = (float)(cx + pt3D.X);
                    float screenY = (float)(cy - pt3D.Y);

                    // Drawing a point marker
                    int size = 14;
                    var rect = new RectangleF(screenX - size / 2f, screenY - size / 2f, size, size);

                    using (var glowPen = new Pen(Color.FromArgb(150, marker.BrandColor), 6))
                    using (var fillBrush = new SolidBrush(marker.BrandColor))
                    using (var borderPen = new Pen(Color.White, 2))
                    {
                        g.DrawEllipse(glowPen, rect);
                        g.FillEllipse(fillBrush, rect);
                        g.DrawEllipse(borderPen, rect);
                    }
                    // Preparing text and dimensions with protection against null values
                    string txt = $"{marker.RetailChain} | {marker.Name}";
                    var textSize = g.MeasureString(txt, font);

                    ForDrawStoreMarkers(g, placedRects, font, textBrush, bgBrush, screenX, screenY, txt, textSize);
                }
            }
        }

        private static void SafetyRendering(Graphics g, List<Rectangle> placedRects, Font font, SolidBrush textBrush, SolidBrush bgBrush, string txt, Rectangle bestRect)
        {
            // Record the occupied zone
            placedRects.Add(bestRect);
            
            g.FillRectangle(bgBrush, bestRect);
            g.DrawString(txt, font, textBrush, bestRect.X + 4, bestRect.Y + 1);
        }

        private static void ForDrawStoreMarkers(Graphics g, List<Rectangle> placedRects, Font font, SolidBrush textBrush, SolidBrush bgBrush, float screenX, float screenY, string txt, SizeF textSize)
        {
            // Safeguard: Text width and height must always be greater than 0
            int w = Math.Max(1, (int)Math.Ceiling(textSize.Width + 8));
            int h = Math.Max(1, (int)Math.Ceiling(textSize.Height + 1));

            Rectangle bestRect;
            bool placed;
            FanPatternSearch(placedRects, screenX, screenY, w, h, out bestRect, out placed);
            // Fallback: if everything is occupied, use the default position
            if (!placed)
            {
                bestRect = new Rectangle((int)screenX + 10, (int)screenY - 10, w, h);
            }
            SafetyRendering(g, placedRects, font, textBrush, bgBrush, txt, bestRect);
        }

        private static void FanPatternSearch(List<Rectangle> placedRects, float screenX, float screenY, int w, int h, out Rectangle bestRect, out bool placed)
        {
            // Offset options (fan-pattern search for an available zone)
            Point[] candidateOffsets = new Point[]  {
                new Point(10, -10),      //  Starting position (top right)
                new Point(10, 8),        //  Bottom right
                new Point(-w - 10, -10), //  Top left
                new Point(-w - 10, 8),   //  Bottom left
                new Point(10, -h - 15),  //  Higher
                new Point(10, h + 12),   //  Below
                new Point(0, -h - 18),   //  Strictly from above
                new Point(0, h + 15)     //  Strictly from below
            };
            bestRect = Rectangle.Empty;
            placed = false;

            // Searching for an unoccupied rectangle using IntersectsWith
            foreach (var offset in candidateOffsets)
            {
                Rectangle testRect = new Rectangle((int)screenX + offset.X, (int)screenY + offset.Y, w, h);

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
        }

        private Point3D Transform(GeoCoordinate3D coord)
        {
            Point3D p = SphericalProjection.GeoToCartesian(coord, _globeRadius);
            p = SphericalProjection.RotateY(p, _rotY);
            p = SphericalProjection.RotateX(p, _rotX);
            return p;
        }

        private void HitTest(Point mousePt)
        {
            int cx = Width / 2;
            int cy = Height / 2;

            foreach (var marker in StoreMarkers)
            {
                var pt3D = Transform(marker.Location);
                if (pt3D.Z <= 0) continue;

                float sx = (float)(cx + pt3D.X);
                float sy = (float)(cy - pt3D.Y);

                double dist = Math.Sqrt(Math.Pow(mousePt.X - sx, 2) + Math.Pow(mousePt.Y - sy, 2));
                if (dist <= 15)
                {
                    StoreSelected?.Invoke(this, marker);
                    break;
                }
            }
        }
    }
}
