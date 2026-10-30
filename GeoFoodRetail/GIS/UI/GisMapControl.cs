/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoFoodRetail.GIS.Abstractions;
using GeoFoodRetail.GIS.Domain.Layers;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.TileProviders;
using GeoFoodRetail.GIS.Projections;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.UI
{
    // 4. Custom GIS Map Control(WinForms UI)
    internal partial class GisMapControl : Control
    {
        private readonly ITileProvider _tileProvider;        
        private readonly VectorLayer _defaultLayer = new VectorLayer("Retail Stores");
        private Point _lastMousePosition;
        private bool _isDragging;                

        public event EventHandler<MapMarker> MarkerClicked;

        public GisMapControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            _tileProvider = new OsmTileProvider();
        }

        public GisMapControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public List<VectorLayer> Layers { get; } = new List<VectorLayer>();
        public int Zoom { get; set; } = 4; // USA Level
        public GeoCoordinate3D Center { get; set; } =
            new GeoCoordinate3D(39.8283, -98.5795); // USA Center

        public List<MapMarker> StoreMarkers
        {
            get
            {
                if (Layers.Count == 0) Layers.Add(_defaultLayer);
                return _defaultLayer.Markers;
            }
        }        

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _lastMousePosition = e.Location;

                CheckMarkerClick(e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                float dx = e.X - _lastMousePosition.X;
                float dy = e.Y - _lastMousePosition.Y;

                PointF centerPixel = MercatorProjection.LatLonToPixel(Center, Zoom);
                PointF newCenterPixel = new PointF(centerPixel.X - dx, centerPixel.Y - dy);

                Center = MercatorProjection.PixelToLatLon(newCenterPixel, Zoom);
                _lastMousePosition = e.Location;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left) _isDragging = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0 && Zoom < 18) Zoom++;
            else if (e.Delta < 0 && Zoom > 2) Zoom--;

            Invalidate();
        }        

        protected override async void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Draw Map Tiles
            PointF centerPixel = MercatorProjection.LatLonToPixel(Center, Zoom);
            PointF screenOffset = new PointF(Width / 2f - centerPixel.X, Height / 2f - centerPixel.Y);

            int tileSize = 256;
            int numTiles = 1 << Zoom;

            int minTileX = (int)Math.Max(0, (-screenOffset.X) / tileSize);
            int maxTileX = (int)Math.Min(numTiles - 1, (Width - screenOffset.X) / tileSize);
            int minTileY = (int)Math.Max(0, (-screenOffset.Y) / tileSize);
            int maxTileY = (int)Math.Min(numTiles - 1, (Height - screenOffset.Y) / tileSize);

            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    var tileImage = await _tileProvider.GetTileAsync(x, y, Zoom);
                    float tileScreenX = x * tileSize + screenOffset.X;
                    float tileScreenY = y * tileSize + screenOffset.Y;

                    if (tileImage != null)
                        g.DrawImage(tileImage, tileScreenX, tileScreenY, tileSize, tileSize);
                    else
                        g.DrawRectangle(Pens.LightGray, tileScreenX, tileScreenY, tileSize, tileSize);
                }
            }            
        }

        private void CheckMarkerClick(Point clickLocation)
        {
            PointF centerPixel = MercatorProjection.LatLonToPixel(Center, Zoom);
            PointF screenOffset = new PointF(Width / 2f - centerPixel.X, Height / 2f - centerPixel.Y);

            foreach (var layer in Layers)
            {
                if (!layer.Visible) continue;
                foreach (var marker in layer.Markers)
                {
                    PointF markerPixel = MercatorProjection.LatLonToPixel(marker.Location, Zoom);
                    PointF screenPos = new PointF(markerPixel.X + screenOffset.X, markerPixel.Y + screenOffset.Y);

                    double dist = Math.Sqrt(Math.Pow(clickLocation.X - screenPos.X, 2) + Math.Pow(clickLocation.Y - screenPos.Y, 2));
                    if (dist <= 12) // Radius hit-test
                    {
                        MarkerClicked?.Invoke(this, marker);
                        return;
                    }
                }
            }
        }
    }
}
