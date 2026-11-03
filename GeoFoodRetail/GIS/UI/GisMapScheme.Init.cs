/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using GeoFoodRetail.GIS.Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class GisMapScheme
    {
        // Spatial Data Initialization 

        private void InitStoresSpatialData()
        {
            _storePixelPositions.Clear();
            _storeNames.Clear();
            for (int i = 0; i < _usaRegionGeoCenters.Length; i++)
            {
                PointF screenPt = GeometryCalc.LatLonToWebMercator(_usaRegionGeoCenters[i].X, _usaRegionGeoCenters[i].Y);
                _storePixelPositions.Add(screenPt);
                _storeNames.Add("Store #" + (i + 1));
            }
        }

        private unsafe void InitRTreeData()
        {
            _usaRegionsRTree = new GIS.StreamingRTree.RTreePage();
            fixed (float* ptr = _usaRegionsRTree.BoxesData)
            {
                for (int i = 0; i < 4; i++)
                {
                    RectangleF bounds = _usaRegionPaths[i].GetBounds();
                    ptr[i] = bounds.Left;
                    ptr[i + 4] = bounds.Top;
                    ptr[i + 8] = bounds.Right;
                    ptr[i + 12] = bounds.Bottom;
                }
            }
        }

        private void InitUsaGraphicsPaths()
        {
            _usaRegionPaths[0] = CreatePolygon(new[] {
                new PointF(5, 35), new PointF(150, 35), new PointF(160, 220),
                new PointF(185, 300), new PointF(145, 385), new PointF(105, 335), new PointF(10, 210) });

            _usaRegionPaths[1] = CreatePolygon(new[] {
                new PointF(150, 35), new PointF(350, 35), new PointF(335, 225), new PointF(160, 220) });

            _usaRegionPaths[2] = CreatePolygon(new[] {
                new PointF(160, 220), new PointF(335, 225), new PointF(360, 235), new PointF(580, 255),
                new PointF(435, 320), new PointF(420, 445), new PointF(365, 375), new PointF(265, 355),
                new PointF(230, 415), new PointF(185, 300) });

            _usaRegionPaths[3] = CreatePolygon(new[] {
                new PointF(350, 35), new PointF(630, 70), new PointF(580, 255), new PointF(335, 225) });
        }

        private void InitUI()
        {
            _rightPanel = new Panel();
            ConfigureRightPanel();

            BuildSearchControls();
            BuildInformationLabels();

            this.Controls.Add(_rightPanel);
            _rightPanel.BringToFront();

            this.Resize += (s, e) => UpdatePanelLayout();
            UpdatePanelLayout();
            UpdateUI();
        }
    }
}
