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
using GeoFoodRetail.GIS.Math3D;
using GeoFoodRetail.GIS.Domain.Models;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm
    {
        private void RegisterGisEvents()
        {
            this.MouseWheel += OnMapMouseWheel;
            this.MouseDown += OnMapMouseDown;
            this.MouseMove += OnMapMouseMove;
            this.MouseUp += OnMapMouseUp;
            this.MouseClick += HandleMapMouseClick;
            this.MouseDoubleClick += OnMapLeftMouseClick;
        }

        private void ResetSearchValidation()
        {
            // Restore the default color and hide the error message
            _txtSearch.BackColor = SystemColors.Window;
            if (_lblSearchError != null)
            {
                _lblSearchError.Visible = false;
            }
        }

        private async void OnSearchClick(object sender, EventArgs e)
        {
            string addr = _txtSearch.Text.Trim();
            // Run validation. If the field is empty, stop execution.
            if (!ValidateSearchInput(addr)) return;

            _btnSearch.Enabled = false;
            _txtSearch.Enabled = false;
            _searchProgressBar.Visible = true;
            try
            {
                await SearchAddressAsync(addr);
            }
            finally
            {
                _searchProgressBar.Visible = false;
                _btnSearch.Enabled = true;
                _txtSearch.Enabled = true;
            }
        }
        // Mouse Events
        private void OnMapMouseWheel(object sender, MouseEventArgs e)
        {
            float old_zoom = _zoomLevel;
            _zoomLevel = 
                (e.Delta > 0) ? _zoomLevel * 1.15f : _zoomLevel / 1.15f;
            _zoomLevel =
                Math.Max(0.5f, Math.Min(_zoomLevel, 5.0f));

            _mapOffset.X -= (e.X - _mapOffset.X) * (_zoomLevel / old_zoom - 1);
            _mapOffset.Y -= (e.Y - _mapOffset.Y) * (_zoomLevel / old_zoom - 1);
            this.Invalidate();
        }
        // Dynamic check of map clicks (via .Left)
        private async void OnMapLeftMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_storeState.HoveredRegionIndex != -1)
                {
                    ClearPinState();
                    _storeState.SelectedRegionIndex =
                        _storeState.HoveredRegionIndex;
                    _selectedStoreIndex =
                        _storeState.SelectedRegionIndex;
                    UpdateUI();
                    this.Invalidate();
                }
                else
                {
                    PointF world_pt = ScreenToWorld(e.Location);
                    PointF geo_pt = WebMercatorToLatLon(world_pt);
                    await ReverseGeocodeAsync(geo_pt);
                }
            }
        }
        
        private void OnMapMouseDown(object sender, MouseEventArgs e)
        {
            // Если кликнули в области правой панели — игнорируем карту!
            if (_rightPanel != null && e.X >= _rightPanel.Left)
                return;

            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                _isDragging = true;
                _lastMousePos = e.Location;
            }
        }
        // Isolating mouse clicks from the map 
        private void OnMapMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {                
                return;
            }
            if (e.X >= 20 && e.X <= (this.ClientSize.Width - 370) && e.Y >= 60 && e.Y <= 460)
            {
                ExecuteSpatialHitTest(e.Location);
            }
        }

        private void OnMapMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
                _isDragging = false;
        }
        // Integrating reverse geocoding into a mouse click
        
        private async void HandleMapMouseClick(object sender, MouseEventArgs e)
        {
            // Ignore map events if the click is on the right panel
            if (_rightPanel != null && e.X >= _rightPanel.Left)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (_storeState.HoveredRegionIndex != -1)
                {
                    ClearPinState();
                    _storeState.SelectedRegionIndex = 
                        _storeState.HoveredRegionIndex;
                    _selectedStoreIndex = 
                        _storeState.SelectedRegionIndex;
                    UpdateUI();
                    this.Invalidate();
                }
                else
                {
                    PointF world_pt = ScreenToWorld(e.Location);
                    PointF geo_pt = WebMercatorToLatLon(world_pt);
                    await ReverseGeocodeAsync(geo_pt);
                }
            }
        }
        
        private void OnClearPinClick(object sender, EventArgs e)
        {
            ClearPinState();
            UpdateUI();
            this.Invalidate();
        }

        private void UpdateDistanceMatrixInfo(int activeStoreIdx)
        {
            string activeStoreName = StoreData.Store[activeStoreIdx].Name;            
            string matrix = $"Distance from {activeStoreName}\n";
            PointF originGeo = _storeCoordArray[activeStoreIdx];

            for (int i = 0; i < StoreData.Store.Count; i++)
            {
                if (i == activeStoreIdx) continue;

                double dist = GeometryCalc.CalculateHaversineDistance(originGeo, _storeCoordArray[i]);                
                var targetStoreName = StoreData.Store[i].Name; 
                matrix += $"-> {targetStoreName}: ~{dist:N0} km\n";                
            }
            _lblDistanceInfo.Text = matrix;
        }
    }
}
