
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
    internal partial class GisMapScheme
    {
        // Rendering

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            RenderOsmTileLayer(g);
            DrawTitleOverlay(g);
            RenderTransformedGisScene(g);
            DrawControlsOverlay(g);
        }

        private void DrawTitleOverlay(Graphics g)
        {
            using (Font titleFont = new Font("Arial", 11, FontStyle.Bold))
            {
                g.DrawString("GIS Map USA (EPSG:3857 + Nominatim Search)", titleFont, Brushes.Black, 20, 10);
            }
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
                bool isHovered = (i == _storeState.HoveredRegionIndex);
                bool isSelected = (i == _storeState.SelectedRegionIndex);

                Color fillColor = isSelected ? Color.FromArgb(160, 144, 238, 144) :
                                 (isHovered ? Color.FromArgb(160, 255, 165, 0) : Color.FromArgb(120, 211, 211, 211));

                using (Brush b = new SolidBrush(fillColor))
                using (Pen p = new Pen(isSelected ? Color.DarkGreen : Color.DimGray, 1.2f / _zoomLevel))
                {
                    g.FillPath(b, _usaRegionPaths[i]);
                    g.DrawPath(p, _usaRegionPaths[i]);
                }
            }
        }

        private void RenderSpatialStoreMarkers(Graphics g)
        {
            using (Brush b = new SolidBrush(Color.Red))
            using (Font font = new Font("Arial", 8 / _zoomLevel, FontStyle.Bold))
            {
                for (int i = 0; i < _storePixelPositions.Count; i++)
                {
                    PointF pos = _storePixelPositions[i];
                    g.FillEllipse(b, pos.X - 4, pos.Y - 4, 8, 8);
                    g.DrawString(_storeNames[i], font, Brushes.Black, pos.X + 6, pos.Y - 4);
                }
            }
        }

        private void DrawDistanceConnections(Graphics g)
        {
            if (_selectedStoreIndex < 0) return;
            PointF startPt = _storePixelPositions[_selectedStoreIndex];

            using (Pen linePen = new Pen(Color.Blue, 1.5f / _zoomLevel) { DashStyle = DashStyle.Dash })
            {
                for (int i = 0; i < _storePixelPositions.Count; i++)
                {
                    if (i != _selectedStoreIndex) g.DrawLine(linePen, startPt, _storePixelPositions[i]);
                }
            }

            DrawPinToStoreLine(g, startPt);
        }

        private void DrawPinToStoreLine(Graphics g, PointF storePt)
        {
            if (!_searchedPinLocation.HasValue) return;

            using (Pen pinLinePen = new Pen(Color.Red, 2.0f / _zoomLevel) { DashStyle = DashStyle.Dot })
            {
                g.DrawLine(pinLinePen, _searchedPinLocation.Value, storePt);
            }
        }

        private void DrawCustomMapPin(Graphics g, PointF worldPt, string label)
        {
            float w = 24f / _zoomLevel;
            float h = 36f / _zoomLevel;
            float x = worldPt.X;
            float y = worldPt.Y;

            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, x - (w * 0.4f), y - (h * 0.1f), w * 0.8f, h * 0.25f);
            }

            using (GraphicsPath pinPath = new GraphicsPath())
            {
                float radius = w / 2f;
                PointF topCenter = new PointF(x, y - h + radius);

                pinPath.AddArc(topCenter.X - radius, topCenter.Y - radius, w, w, 200, 140);
                pinPath.AddLine(x + (w * 0.35f), y - h + (radius * 1.5f), x, y);
                pinPath.CloseFigure();

                using (SolidBrush fillBrush = new SolidBrush(Color.Crimson))
                using (Pen strokePen = new Pen(Color.White, 2f / _zoomLevel))
                {
                    g.FillPath(fillBrush, pinPath);
                    g.DrawPath(strokePen, pinPath);
                }
                float dotSize = w * 0.35f;
                using (SolidBrush dotBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(dotBrush, topCenter.X - (dotSize / 2f), topCenter.Y - (dotSize / 2f), dotSize, dotSize);
                }
            }
        }

        private void DrawControlsOverlay(Graphics g)
        {
            using (Font font = new Font("Arial", 8.5f, FontStyle.Regular))
            {
                g.DrawString("Zoom: " + _zoomLevel.ToString("F2") + "x | Pan: X=" + _mapOffset.X.ToString("F0") + ", Y=" + _mapOffset.Y.ToString("F0"), font, Brushes.Black, 20, this.ClientSize.Height - 25);
                g.DrawString("RMB/MMB + Drag = Pan | Mouse Wheel = Zoom", font, Brushes.DarkBlue, 20, this.ClientSize.Height - 42);
            }
        }
    }
}
