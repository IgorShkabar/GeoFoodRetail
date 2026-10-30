/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * How to implement a dynamic sphere radius
In the `Gis3DGlobeControl` class, the sphere's radius is defined by a variable (for example, `float _radius` 
or `float _globeRadius`). To ensure it automatically adjusts to the window size, you must 
recalculate it within the control's `OnResize` method.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.Domain.Layers;
using GeoFoodRetail.GIS.Math3D;
using GeoFoodRetail.GIS.Projections;
using System.Linq;
using GeoFoodRetail.GIS.StreamingRTree;
using GeoFoodRetail.GIS.Domain;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class Gis3DGlobeControl : Control
    {
        private static int _lastLoggedCount = -1;
        private double  _globeRadius = 200f;       
        private bool    _isSelecting = false;
        private PointF  _selectStart;
        private PointF  _selectCurrent;       
        private double  _rotX = 0.3;    
        private double  _rotY = -1.5;     
        private Point   _lastMouse;
        private bool    _isDragging;
        private readonly SpatialClusterEngine _clusterEngine = new SpatialClusterEngine();
        private readonly VectorLayer _defaultLayer = new VectorLayer("Retail Stores");
        
        public Gis3DGlobeControl()
        {
            // Enable double buffering for the 3D control itself
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Create a default procedural texture (if no external image is provided)
            CreateDefaultWorldTexture();
            Layers.Add(_defaultLayer);

            // Mandatory: allow the control to receive focus upon clicking; otherwise, it won't capture keystrokes         
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
        }

        public bool IsSelectionMode { get; set; } = true; // Flag to enable selection mode     
        // Property for manually changing the radius from code
        public float GlobeRadius
        {
            get => (float)_globeRadius;
            set
            {
                _globeRadius = value;
                Invalidate(); // Redraw the globe   
            }
        }
        public List<VectorLayer> Layers { get; } = new List<VectorLayer>();
        public List<RetailStoreMarker> StoreMarkers { get; } = new List<RetailStoreMarker>();
        public Bitmap WorldTexture { get; set; } // World map image (Equirectangular)
        public int Zoom { get; set; } = 4; // USA Level
        public GeoCoordinate3D Center { get; set; } =
            new GeoCoordinate3D(39.8283, -98.5795); // USA Center       
  

        // Define an event for store selection
        public event EventHandler<RetailStoreMarker> StoreSelected; // The selected retail

        /* In the map rendering component or GisMapControl, add a cluster rendering method that 
         * displays individual icons or circular markers showing the count of nested elements. */

        public void DrawClusters(Graphics g, List<MarkerCluster> clusters, Font font, Brush textBrush, Pen borderPen)
        {
            int totalClusters = clusters.Count;
            for (int i = 0; i < totalClusters; i++)
            {
                var cluster = clusters[i];
                DrawSingleClusterNode(g, cluster, font, textBrush, borderPen);
            }
        }

        // Inside your mouse-click or selection handler where a store is hit:
        protected void OnStoreClicked(RetailStoreMarker store)
        {
            // Fire the event up to whoever is listening (the Form)
            StoreSelected?.Invoke(this, store);
        }

        private static void HelpDrawSingleClusterNode(Graphics g, MarkerCluster cluster, Font font, Brush textBrush, Pen borderPen, float sx, float sy)
        {
            if (!cluster.IsSingleCluster)
            {
                using (var clusterBrush = new SolidBrush(Color.FromArgb(200, 220, 50, 50)))
                {
                    int size = 26;
                    var rect = new RectangleF(sx - size / 2f, sy - size / 2f, size, size);
                    g.FillEllipse(clusterBrush, rect);
                    g.DrawEllipse(borderPen, rect);

                    string countTxt = cluster.Count.ToString();
                    var sz = g.MeasureString(countTxt, font);
                    g.DrawString(countTxt, font, textBrush, sx - sz.Width / 2f, sy - sz.Height / 2f);
                }
            }
            else
            {
                var marker = cluster.Markers[0];
                int size = 14;
                var rect = new RectangleF(sx - size / 2f, sy - size / 2f, size, size);

                using (var fillBrush = new SolidBrush(Color.LimeGreen))
                {
                    g.FillEllipse(fillBrush, rect);
                    g.DrawEllipse(borderPen, rect);
                }
                string txt = marker.Name;
                var txtSize = g.MeasureString(txt, font);
                int w = Math.Max(1, (int)Math.Ceiling(txtSize.Width + 8));
                int h = Math.Max(1, (int)Math.Ceiling(txtSize.Height + 1));
                var bestRect = new Rectangle((int)sx + 10, (int)sy - 10, w, h);

                using (var bgBrush = new SolidBrush(Color.FromArgb(220, 15, 23, 42)))
                {
                    g.FillRectangle(bgBrush, bestRect);
                }
                g.DrawString(txt, font, textBrush, bestRect.X + 4, bestRect.Y + 1);
            }
        }
        private void DrawSingleClusterNode(Graphics g, MarkerCluster cluster, Font font, Brush textBrush, Pen borderPen)
        {
            float sx = cluster.CenterPixel.X;
            float sy = cluster.CenterPixel.Y;
            HelpDrawSingleClusterNode(g, cluster, font, textBrush, borderPen, sx, sy);
        }                

        private void DrawDebugConsole(Graphics g)
        {        
            var lines = GlobeLogger.GetLines().ToList();
            if (lines.Count == 0) return;

            int padding = 6, lineHeight = 15, width = 390;
            int height = (lines.Count * lineHeight) + (padding * 2);

            // Move this block up, above the bottom API console (the bottom console is approximately 80px high)           
            int x = 10, y = Height - height - 10; 

            using (var bgBrush = new SolidBrush(Color.FromArgb(210, 15, 23, 42)))
            using (var borderPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
            using (var textBrush = new SolidBrush(Color.LightYellow))
            using (var font = new Font("Consolas", 9.5f, FontStyle.Regular))
            {
                g.FillRectangle(bgBrush, x, y, width, height);
                g.DrawRectangle(borderPen, x, y, width, height);

                int currentY = y + padding;
                foreach (var line in lines)
                {           
                    g.DrawString(line, font, textBrush, x + padding, currentY);
                    currentY += lineHeight;
                }
            }
        }
        // Automatic radius recalculation when window dimensions change
        // When the user drags the corners of the form, the OnResize method will automatically recalculate _globeRadius:
        protected override void OnResize(EventArgs e)
        {            
            base.OnResize(e);
            if (Width <= 0 || Height <= 0) return;
            
            int minDimension = Math.Min(Width, Height);
            _globeRadius = (minDimension / 2.0) * 0.99;  
            /// Invalidate();

            base.OnResize(e);
            GlobeLogger.ResetLastMessage(); 
            Invalidate();
        }               

        private void CreateDefaultWorldTexture()
        {
            WorldTexture = new Bitmap(720, 360);
            using (Graphics g = Graphics.FromImage(WorldTexture))
            {
                g.Clear(Color.FromArgb(15, 45, 90)); // Океаны
                using (Font f = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    // In a real application, load WorldTexture = new Bitmap("earth_map.jpg");                    
                    g.DrawString("Equirectangular Earth Map Layer", f, Brushes.DarkGreen, 50, 50);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            // IsSelectionMode is set in true to get a selection area.
            if (IsSelectionMode == true && e.Button == MouseButtons.Left)
            {
                _isSelecting = true;
                _selectStart = e.Location;
                _selectCurrent = e.Location;
            }
            else
            {
                _isDragging = true;
                _lastMouse = e.Location;
            }            
            HitTest(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                double dx = (e.X - _lastMouse.X) * 0.008;
                double dy = (e.Y - _lastMouse.Y) * 0.008;
                _rotY += dx;
                _rotX -= dy;

                _lastMouse = e.Location;
                Invalidate();
            }
            else if (_isSelecting) 
            {
                _selectCurrent = e.Location;
                Invalidate(); 
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;

            if (_isSelecting && e.Button == MouseButtons.Left)
            {
                _isSelecting = false;

                var analyzer = new AreaSpatialAnalyzer(_selectStart, _selectCurrent);
                string report = 
                    analyzer.Analyze(StoreMarkers, Width / 2, Height / 2, loc => Transform(loc));

                if (!string.IsNullOrEmpty(report))
                {
                    MessageBox.Show(report, "Analysis of Region", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Invalidate();
            }
            else
            {                
                _isSelecting = false;                
            }
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0 && _globeRadius < 500)
            {
                _globeRadius += 20;
            }
            else if (e.Delta < 0 && _globeRadius > 100)
            {
                _globeRadius -= 20;
            }
            Zoom += e.Delta;
            GlobeLogger.ResetLastMessage(); 
            Invalidate();            
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {            
            if (keyData == Keys.Escape)
            {
                if (IsSelectionMode || _isSelecting)
                {
                    _isSelecting = false;
                    IsSelectionMode = false;
                    Invalidate(); 
                    return true;  
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // A helper logging procedure:
        private void AccessorLogClusters(int count)
        {
            GlobeLogger.Log($"Rendered {count} clusters");
        }        
    }
}

