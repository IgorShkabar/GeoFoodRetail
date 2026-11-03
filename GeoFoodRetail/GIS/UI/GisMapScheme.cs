/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * The `DeliveryZoneForm` code implements basic GIS functionality for working with four macro-regions of the United States.
Implemented GIS functions:
Spatial Query & Hit Testing:
Two-level Hit Test (Broad Phase + Narrow Phase):
Broad Phase (Fast filter): The call to `_usaRegionsRTree.IntersectsBatch4(e.X, e.Y, e.X, e.Y, 0)` checks for the intersection of the mouse coordinates with the bounding boxes (AABB) of all four regions simultaneously, utilizing SIMD instructions.
Narrow Phase (Precise check): The call to `_usaRegionPaths[i].IsVisible(e.Location)` performs a precise check to determine whether the point lies within a vector polygon (`GraphicsPath`).
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.GIS.Constants;
using System.Text.Json;
using GeoFoodRetail.GIS;
using GeoFoodRetail.GIS.Math3D;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class GisMapScheme : MapState
    {        
        public GisMapScheme()
        {
            InitializeComponent();
            GisMapSchemeConfigureSettings();
            InitUsaGraphicsPaths();
            InitRTreeData();
            InitStoresSpatialData();
            InitUI();
            RegisterGisEvents();

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeliveryZoneGIS/1.0");
            _storeState.InitStoreData();
        }

        public decimal ExternalCashGiven { get; set; } = 1000.00m;

        private void GisMapSchemeConfigureSettings()
        {
            this.Text = "Scheme: GIS Map USA - Responsive Layout & Geocoding API";
            this.Size = new Size(1000, 550);
            this.MinimumSize = new Size(818, 497);
            this.DoubleBuffered = true;
        }

        private void RegisterGisEvents()
        {
            this.MouseWheel += OnMapMouseWheel;
            this.MouseDown += OnMapMouseDown;
            this.MouseMove += OnMapMouseMove;
            this.MouseUp += OnMapMouseUp;
            this.MouseClick += HandleMapMouseClick;
        }

        // Geocoding API & Search

        private void UpdateUIWithPinDistance(int storeIdx, double distanceKm)
        {
            UpdateUI();
            _lblDistanceInfo.Text = "=== SEARCH RESULT ===\n" +
                                  "Address: " + _searchedAddressLabel + "\n" +
                                  "Nearest Store: Store #" + (storeIdx + 1) + " (" + _regionNames[storeIdx] + ")\n" +
                                  "Distance: ~" + distanceKm.ToString("N1") + " km\n\n" +
                                  _lblDistanceInfo.Text;
            _lblNoteInfo.Text = _lblNoteInfo.Text;
        }

        private void CenterMapOnPoint(PointF targetWorldPt)
        {
            _zoomLevel = 2.0f;
            int mapCenterX = (this.ClientSize.Width - 370) / 2;
            int mapCenterY = this.ClientSize.Height / 2;

            _mapOffset.X = mapCenterX - (targetWorldPt.X * _zoomLevel);
            _mapOffset.Y = mapCenterY - (targetWorldPt.Y * _zoomLevel);
            this.Invalidate();
        }

        // Math & GIS Transformations        

        private GraphicsPath CreatePolygon(PointF[] pts)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(pts);
            return path;
        }

        // UI Initialization & Clear Pin 

        private void ClearPinState()
        {
            _searchedPinLocation = null;
            _searchedAddressLabel = string.Empty;
            if (_btnClearPin != null) _btnClearPin.Enabled = false;
        }

        private void OnClearPinClick(object sender, EventArgs e)
        {
            ClearPinState();
            UpdateUI();
            this.Invalidate();
        }

        private void UpdateDistanceMatrixInfo(int activeStoreIdx)
        {
            string matrix = "--- Distance from Store #" + (activeStoreIdx + 1) + " ---\n";
            PointF originGeo = _usaRegionGeoCenters[activeStoreIdx];

            for (int i = 0; i < _usaRegionGeoCenters.Length; i++)
            {
                if (i == activeStoreIdx) continue;
                double dist = GeometryCalc.CalculateHaversineDistance(originGeo, _usaRegionGeoCenters[i]);
                matrix += "-> Store #" + (i + 1) + ": ~" + dist.ToString("N0") + " km\n";
            }
            _lblDistanceInfo.Text = matrix;

            _lblNoteInfo.Text = $"GIS Map USA (in the Web Mercator projection + search via Nominatim) /n EPSG:3857 - is a digital code, standard in cartography & GIS systems, that designates the Wb Mercator.";
        }        
    }
}
