/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm
    {
        // UI Initialization & Clear Pin

        private void ClearPinState()
        {
            _searchedPinLocation = null;
            _searchedAddressLabel = string.Empty;
            if (_btnClearPin != null) _btnClearPin.Enabled = false;
        }

        private void ConfigureRightPanel()
        {
            _rightPanel.Dock = DockStyle.Right;
            _rightPanel.BackColor = Color.White; 
            _rightPanel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void DrawTitleOverlay(Graphics g)
        {
            using (var titleFont = new Font("Arial", 11, FontStyle.Bold))
            {
                g.DrawString("GIS Map USA (EPSG:3857 + Nominatim Search)", titleFont, Brushes.Black, 20, 15);
            }
        }

        private void DrawDistanceConnections(Graphics g)
        {
            if (_selectedStoreIndex < 0) return;
            PointF startPt = _storePixelPositions[_selectedStoreIndex];

            using (var linePen = new Pen(Color.Blue, 1.5f / _zoomLevel) { DashStyle = DashStyle.Dash } )
            {
                for (int i = 0; i < _storePixelPositions.Count; i++)
                {
                    if (i != _selectedStoreIndex)
                        g.DrawLine(linePen, startPt, _storePixelPositions[i]);
                }
            }
            DrawPinToStoreLine(g, startPt);
        }

        private void DrawPinToStoreLine(Graphics g, PointF store_pt)
        {
            if (!_searchedPinLocation.HasValue) return;

            using (var pin_line_pen = new Pen(Color.Red, 2.0f / _zoomLevel) { DashStyle = DashStyle.Dot })
            {
                g.DrawLine(pin_line_pen, _searchedPinLocation.Value, store_pt);
            }
        }

        private void DrawCustomMapPin(Graphics g, PointF world_pt, string label)
        {
            float w = 24f / _zoomLevel;
            float h = 36f / _zoomLevel;
            float x = world_pt.X;
            float y = world_pt.Y;

            using (var shadow_brush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
            {
                g.FillEllipse(shadow_brush, x - (w * 0.4f), 
                              y - (h * 0.1f), w * 0.8f, h * 0.25f);
            }
            using (var pin_path = new GraphicsPath())
            {
                float radius = w / 2f;
                var top_center = new PointF(x, y - h + radius);

                pin_path.AddArc(top_center.X - radius, top_center.Y - radius, w, w, 200, 140);
                pin_path.AddLine(x + (w * 0.35f), y - h + (radius * 1.5f), x, y);
                pin_path.CloseFigure();

                using (var fill_brush = new SolidBrush(Color.Crimson))
                using (var stroke_pen = new Pen(Color.White, 2f / _zoomLevel))
                {
                    g.FillPath(fill_brush, pin_path);
                    g.DrawPath(stroke_pen, pin_path);
                }
                float dot_size = w * 0.35f;
                using (var dotBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(dotBrush, 
                                top_center.X - (dot_size / 2f), 
                                top_center.Y - (dot_size / 2f), dot_size, dot_size);
                }
            }
        }

        private void DrawControlsOverlay(Graphics g)
        {
            using (var font = new Font("Arial", 8.5f, FontStyle.Regular))
            {
                g.DrawString("Zoom: " + 
                    _zoomLevel.ToString("F2") + 
                    "x | Pan: X=" + 
                    _mapOffset.X.ToString("F0") + 
                    ", Y=" + 
                    _mapOffset.Y.ToString("F0"), 
                    font, 
                    Brushes.Black, 
                    20, 
                    this.ClientSize.Height - 25);

                g.DrawString("RMB/MMB + Drag = Pan | Mouse Wheel = Zoom", 
                    font, 
                    Brushes.DarkBlue, 
                    20, 
                    this.ClientSize.Height - 42);
            }
        }

        // Rendering
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;            
            DrawTitleOverlay(g);
            RenderTransformedGisScene(g);
            DrawControlsOverlay(g);
        }

        private void RenderTransformedGisScene(Graphics g)
        {
            GraphicsState state = g.Save();
            g.TranslateTransform(_mapOffset.X, _mapOffset.Y);
            g.ScaleTransform(_zoomLevel, _zoomLevel);
           
            RenderVectorRegions(g);
            RenderSpatialStoreMarkers(g);  
            DrawDistanceConnections(g);   

            if (_searchedPinLocation.HasValue)
            {
                DrawCustomMapPin(g, _searchedPinLocation.Value, _searchedAddressLabel);
            }
            g.Restore(state);
        }

        private void RenderVectorRegions(Graphics g)
        {
            for (int i = 0; i < 4; i++)
            {
                bool is_hovered = (i == _storeState.HoveredRegionIndex);
                bool is_selected = (i == _storeState.SelectedRegionIndex);

                Color fill_color = is_selected ? Color.FromArgb(160, 144, 238, 144) :
                                 (is_hovered ? Color.FromArgb(160, 255, 165, 0) : Color.FromArgb(120, 211, 211, 211));

                using (Brush b = new SolidBrush(fill_color))
                using (var p = new Pen(is_selected ? Color.DarkGreen : Color.DimGray, 1.2f / _zoomLevel))
                {
                    g.FillPath(b, _usaRegionPaths[i]);
                    g.DrawPath(p, _usaRegionPaths[i]);
                }
            }   
        }

        private void RenderSpatialStoreMarkers(Graphics g)
        {   
            if (_storePixelPositions == null || _storePixelPositions.Count == 0)
                return;

            List<Rectangle> placedRects = new List<Rectangle>();

            // We set minimum and maximum font size limits for zooming, so the text doesn't end up reaching astronomical proportions.
            float safeFontSize = Math.Max(7f, Math.Min(12f, 8f / Math.Max(0.1f, _zoomLevel)));

            using (var b = new SolidBrush(Color.Red))
            using (var font = new Font("Arial", safeFontSize, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var bgBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255))) // Light-colored backing
            using (var borderPen = new Pen(Color.LightGray, 1))
            {
                for (int i = 0; i < _storePixelPositions.Count; i++)
                {
                    PointF pos = _storePixelPositions[i];

                    // Draw tthe marker point 
                    g.FillEllipse(b, pos.X - 4, pos.Y - 4, 8, 8);

                    string txt = _storeNames[i];
                    SizeF textSize = g.MeasureString(txt, font);

                    int w = Math.Max(1, (int)Math.Ceiling(textSize.Width + 4));
                    int h = Math.Max(1, (int)Math.Ceiling(textSize.Height + 2));

                    // Fan-shaped offsets for spacing out store names
                    Point[] candidateOffsets = new Point[]
                    {
                        new Point(6, -4),        // Top right (standard)
                        new Point(6, 8),         
                        new Point(-w - 6, -4),   
                        new Point(-w - 6, 8),    
                        new Point(0, -h - 8),    
                        new Point(0, h + 6)      
                    };

                    Rectangle bestRect = Rectangle.Empty;
                    bool placed = false;

                    foreach (var offset in candidateOffsets)
                    {
                        Rectangle testRect = new Rectangle(
                            (int)pos.X + offset.X,
                            (int)pos.Y + offset.Y,
                            w, h
                        );

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
                        bestRect = new Rectangle((int)pos.X + 6, (int)pos.Y - 4, w, h);
                    }

                    placedRects.Add(bestRect);

                    // Rendering the background and text
                    g.FillRectangle(bgBrush, bestRect);
                    g.DrawRectangle(borderPen, bestRect);
                    g.DrawString(txt, font, textBrush, bestRect.X + 2, bestRect.Y + 1);
                }
            }
        }
    }
}

