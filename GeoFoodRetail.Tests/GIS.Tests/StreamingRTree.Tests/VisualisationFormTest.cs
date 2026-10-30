/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using GeoFoodRetail.EnterpriseForm;
using GeoFoodRetail.GIS.StreamingRTree;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.Tests.GIS.Tests.StreamingRTree.Tests
{
    [TestFixture]
    public class VisualisationFormTest
    {
        private RectangleF _queryAABB = new RectangleF(100, 100, 100, 100);

        [Test]
        public void SearchAreaSize_ShouldBeValid()
        {    
            // Size of the search/selection area of the points-retails
            var sz = new RectangleF(0f, 0f, 800f, 600f);

            // Expected optimal dimensions of search/selected area
            var expectedSize = new Size(800, 600);

            // Maximal Screen/Form sizes: maxH - height, maxW - Width 
            float maxH = 1080f;
            float maxW = 1920f;

            Assert.Multiple(() =>
            {
                Assert.That(sz.Height, Is.LessThanOrEqualTo(maxH), "Height exceeds permissible boundaries of the search box.");
                Assert.That(sz.Width, Is.LessThanOrEqualTo(maxW), "Width exceeds permissible of the search box.");
            });            
        }

        /** Create a virtual canvas (Bitmap) and obtain a Graphics object from it.
        Initialize a Pen and a Brush with actual values ​​(for example, Pens.Black &
        Brushes.White).     */
        public void DrawRectangleSearchAreaUnderCursor_ShouldBeDrawn()
        {
            // create a temporary image and a drawing context locally.
            using (Bitmap bmp = new Bitmap(100, 100))
            using (Graphics g = Graphics.FromImage(bmp)) 
            using (Pen pn = new Pen(Color.Black))
            using (Brush br = new SolidBrush(Color.Red))
            {
                // verify that the code executes without exceptions
                Assert.DoesNotThrow(() =>
                {
                    g.FillRectangle(br, _queryAABB);
                    g.DrawRectangle(pn, _queryAABB.X, _queryAABB.Y, _queryAABB.Width, _queryAABB.Height);
                });
            }
        }
    }
}
