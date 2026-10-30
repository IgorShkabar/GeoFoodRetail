/** 
 Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using GeoFoodRetail.EnterpriseForm;
using GeoFoodRetail.GIS.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GeoFoodRetail.GIS.StreamingRTree.DemoPlatform;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal partial class VisualisationForm : Form
    {
        private RectangleF _queryAABB = new RectangleF(5, 5, 10, 10); 
        private List<StoreItem> _retailChains = new List<StoreItem>();

        // Constants and rendering parameters
        private const float VisualStoreRadius = 6f;
        private const float SpatialIndexBoxSize = 15f;
        private const int GridStepPixels = 100;
        private const int MinQuerySize = 12;
        private const float ScrollDelta = 10f;
        private const int MaxDisplayedHits = 34;

        public VisualisationForm(List<StoreItem> retailList = null)
        {
            InitializeComponent();

            // Beware of Window Resizing: WinForms controls can stutter or flicker when 
            // redrawing large background images during a resize. To fix this, enable DoubleBuffered
            this.DoubleBuffered = true;
            ///this.Opacity = 0.9;            

            this.Text = "GIS Spatial Index Inspector";
            //this.Size = new Size(1024, 768);
            this.DoubleBuffered = true;

            this.MouseMove += VisualisationForm_MouseMove;
            
            _retailChains = RetailChainsData.RetailList ?? new List<StoreItem>();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            ///g.Clear(Color.WhiteSmoke);
            
            DrawCoordinateGrid(g);

            List<StoreItem> matchedStores = new List<StoreItem>();
            int totalHitsCount = 0;

            totalHitsCount = 
                DrawAndLinearTraversalElementsCollection(g, matchedStores, totalHitsCount);

            DrawRectangleSearchAreaUnderCursor(g);

            // Debug and Results Panel
            PanelShowResults(matchedStores, totalHitsCount, g);
        }

        // Draw a search-area rectangle under the cursor
        private void DrawRectangleSearchAreaUnderCursor(Graphics g)
        {
            using (Pen greenPen = new Pen(Color.LimeGreen, 2))
            using (Brush greenFill = new SolidBrush(Color.FromArgb(35, Color.LimeGreen)))
            {
                g.FillRectangle(greenFill, _queryAABB);
                g.DrawRectangle(greenPen, _queryAABB.X, _queryAABB.Y, _queryAABB.Width, _queryAABB.Height);
            }
        }

        // The clean Linear Traversal 
        // all elements in Collection without restrictions; to draw elements
        private int DrawAndLinearTraversalElementsCollection(Graphics g, List<StoreItem> matchedStores, int totalHitsCount)
        {
            using (Pen bluePen = new Pen(Color.DeepSkyBlue, 1.5f))   
            using (Pen redPen = new Pen(Color.Red, 2f))
            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            {
                // A clean, linear traversal of ALL elements in a collection, without restrictions
                for (int i = 0; i < _retailChains.Count; i++)
                {
                    var store = _retailChains[i];

                    float screenX = (float)((store.Longitude + 180.0) * (Width / 360.0));
                    float screenY = (float)((90.0 - store.Latitude) * (Height / 180.0));

                    RectangleF boxRect = new RectangleF( screenX - SpatialIndexBoxSize, screenY - SpatialIndexBoxSize, 
                                                        SpatialIndexBoxSize * 2f, SpatialIndexBoxSize * 2f );

                    RectangleF drawRect = new RectangleF( screenX - VisualStoreRadius, screenY - VisualStoreRadius,
                                                        VisualStoreRadius * 2f, VisualStoreRadius * 2f );

                    bool isHit = _queryAABB.IntersectsWith(boxRect);

                    if (isHit)
                    {
                        matchedStores.Add(store);
                        totalHitsCount++;
                    }
                    using (Brush fillBrush = new SolidBrush(isHit ? Color.FromArgb(140, Color.Red) : Color.FromArgb(80, Color.DodgerBlue)))
                    {
                        g.FillEllipse(fillBrush, drawRect);
                    }

                    g.DrawEllipse(isHit ? redPen : bluePen, drawRect);
                    g.DrawString(store.Name, font, Brushes.Black, drawRect.X + (VisualStoreRadius * 2f), drawRect.Y);
                }
            }
            return totalHitsCount;
        }

        // Drawing a coordinate grid
        private void DrawCoordinateGrid(Graphics g)
        {
            
            using (Pen gridPen = new Pen(Color.FromArgb(220, 220, 220), 1))
            {
                for (int x = 0; x < Width; x += GridStepPixels) g.DrawLine(gridPen, x, 0, x, Height);
                for (int y = 0; y < Height; y += GridStepPixels) g.DrawLine(gridPen, 0, y, Width, y);
            }
        }

        // Panel of debugging and results
        private void PanelShowResults(List<StoreItem> matchedStores, int totalHitsCount, Graphics g)
        {
            MakePanelBackgroundForResultOutput(matchedStores, totalHitsCount, g);

            using (Font titleFont = new Font("Consolas", 12, FontStyle.Bold))
            using (Font textFont = new Font("Consolas", 10, FontStyle.Regular))
            {
                g.DrawString("Spatial Index Inspector", titleFont, Brushes.DarkSlateGray, 20, 20);
                g.DrawString($"Found stores in zone (Total Hits: {totalHitsCount}):", textFont, Brushes.Black, 20, 50);

                int offset = 75;
                foreach (var store in matchedStores.Take(MaxDisplayedHits))
                {
                    g.DrawString($"> {store.Name}", textFont, Brushes.DarkRed, 30, offset);
                    offset += 20;
                }
            }                       
        }

        private void MakePanelBackgroundForResultOutput(List<StoreItem> matchedStores, int totalHitsCount, Graphics g)
        {
            int maxDisplayed = Math.Min(matchedStores.Count, MaxDisplayedHits);

            // Dynamic calculation of substrate dimensions
            int panelWidth = 365;
            int titleHeight = 75; // Top margin before the first line of a list
            int itemHeight = 20;  // Height of a single line
            int paddingBottom = 20; // Bottom margin for aesthetic appeal

            int panelHeight = titleHeight + (maxDisplayed * itemHeight) + paddingBottom;

            // draw the background layer itself (the layer between the map and the text)
            using (Brush panelBackground = new SolidBrush(Color.FromArgb(252, SystemColors.Window))) // 240 - slight transparency
            using (Pen panelBorder = new Pen(Color.LightGray, 1))
            {
                // Draw the background for the underlay in the top-left corner (coordinates 10, 10)
                g.FillRectangle(panelBackground, 10, 10, panelWidth, panelHeight);
                g.DrawRectangle(panelBorder, 10, 10, panelWidth, panelHeight);
            }
        }

        private void VisualisationForm_MouseMove(object sender, MouseEventArgs e)
        {
            _queryAABB.X = e.X - _queryAABB.Width / 2f;
            _queryAABB.Y = e.Y - _queryAABB.Height / 2f;
            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            float delta = e.Delta > 0 ? ScrollDelta : -ScrollDelta;

            _queryAABB.Width = Math.Max(MinQuerySize, _queryAABB.Width + delta);
            _queryAABB.Height = Math.Max(MinQuerySize, _queryAABB.Height + delta);
            this.Invalidate();
        }
    }
}

