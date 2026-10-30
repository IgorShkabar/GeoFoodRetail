/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * High-performance debugging and demonstration platform (test bench).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal partial class DemoPlatform : Form
    {
        private RTreePage _page;
        // Mouse-driven query (Query AABB)
        private RectangleF queryAABB = new RectangleF(100, 100, 150, 100);
        // Emulate screen/form dimensions using hard-coded values
        private float maxHeigh = 1080f;
        private float maxWidh = 1920f;

        public DemoPlatform()
        {
            InitializeComponent();

            this.Text = "RTreePage SIMD IntersectsBatch4 Demo (VS 2017)";
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true; // Eliminating flickering during redrawing

            InitSampleData();

            // Moving the Query AABB along with the cursor
            this.MouseMove += (s, e) =>
            {
                queryAABB.X = e.X - queryAABB.Width / 2;
                queryAABB.Y = e.Y - queryAABB.Height / 2;
                this.Invalidate(); // Redraw request
            };
        }

        // Add an internal getter to the DemoPlatform property 
        // that returns a reference (ref) to the struct, rather than a copy:
        internal ref RTreePage CurrentPageRef => ref _page;
        
        public unsafe void InitSampleData()
        {
            _page = new RTreePage();
            // We populate 4 test boxes in SoA format directly into a fixed buffer.
            fixed (float* ptr = _page.BoxesData)
            {
                // min_X [0..3]
                ptr[0] = 50; ptr[1] = 300; ptr[2] = 500; ptr[3] = 200;
                // min_Y [4..7]
                ptr[4] = 50; ptr[5] = 100; ptr[6] = 300; ptr[7] = 350;
                // max_X [8..11]
                ptr[8] = 200; ptr[9] = 450; ptr[10] = 650; ptr[11] = 350;
                // max_Y [12..15]
                ptr[12] = 180; ptr[13] = 250; ptr[14] = 450; ptr[15] = 500;
            }
        }

        // warn about unacceptable dimensions of the search area frame.
        private void CheckQueryAABBB()
        {
            var queryRect = new RectangleF(0, 0, 800f, 600f);

            if (queryRect.Height <= 0 || queryRect.Width <= 0 ||
                queryRect.Height > maxHeigh || queryRect.Width > maxWidh)
            {
                MessageBox.Show("The search area size is outside of the permissible boundaries.");
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            float qMinX = queryAABB.Left;
            float qMinY = queryAABB.Top;
            float qMaxX = queryAABB.Right;
            float qMaxY = queryAABB.Bottom;

            // INVOKING SIMD BATCH VALIDATION
            int mask = _page.IntersectsBatch4(qMinX, qMinY, qMaxX, qMaxY, startIndex: 0);

            DrawBoxes(mask, g);

            // Draw a green Query AABB (controlled by the mouse)
            DrawQueryAABB(g);
            CheckQueryAABBB();

            // Display the bitmask status at the top of the screen
            LayoutBitMaskState(mask, g);
        }

        // draw boxes from RTreePage
        private void DrawBoxes(int mask, Graphics g)
        {
            // Drawing boxes from RTreePage
            using (Pen bluePen = new Pen(Color.DodgerBlue, 2))
            using (Pen redPen = new Pen(Color.Red, 4))
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                for (int i = 0; i < 4; i++)
                {
                    _page.GetBox(i, out float minX, out float minY, out float maxX, out float maxY);
                    RectangleF boxRect = RectangleF.FromLTRB(minX, minY, maxX, maxY);

                    // Check the bit in the received mask (1, 2, 4, or 8)
                    bool isIntersected = (mask & (1 << i)) != 0;

                    // Filling the inner background
                    using (Brush fillBrush = new SolidBrush(isIntersected ? Color.FromArgb(100, Color.Red) : Color.FromArgb(50, Color.Blue)))
                    {
                        g.FillRectangle(fillBrush, boxRect);
                    }

                    // Outline: Red if it intersects, blue if it does not
                    g.DrawRectangle(isIntersected ? redPen : bluePen, boxRect.X, boxRect.Y, boxRect.Width, boxRect.Height);

                    // Text box
                    g.DrawString($"Box #{i}\nBit {(1 << i)}: {(isIntersected ? "HIT" : "MISS")}",
                        font, textBrush, boxRect.X + 5, boxRect.Y + 5);
                }
            }            
        }
        
        private void DrawQueryAABB(Graphics g)
        {
            // Draw a green Query AABB (controlled by the mouse)
            using (Pen greenPen = new Pen(Color.LimeGreen, 3))
            using (Brush greenFill = new SolidBrush(Color.FromArgb(40, Color.LimeGreen)))
            {
                g.FillRectangle(greenFill, queryAABB);
                g.DrawRectangle(greenPen, queryAABB.X, queryAABB.Y, queryAABB.Width, queryAABB.Height);
            }
        }

        private void LayoutBitMaskState(int mask, Graphics g)
        {
            // Display the bitmask status at the top of the screen
            using (Font titleFont = new Font("Consolas", 14, FontStyle.Bold))
            {
                string maskBits = Convert.ToString(mask, 2).PadLeft(4, '0');

                g.DrawString($"Query AABB (Mouse) | SIMD Result Mask: {mask} (Binary: 0b{maskBits})",
                    titleFont, Brushes.DarkSlateGray, 10, 10);
            }
        }
    }
}


