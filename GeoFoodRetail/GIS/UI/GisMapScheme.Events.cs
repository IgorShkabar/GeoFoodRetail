/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.GIS.Math3D;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class GisMapScheme 
    {
        private async void OnSearchClick(object sender, EventArgs e)
        {
            string address = _txtSearch.Text.Trim();

            // Initiate validation. If the field is empty, stop execution.
            if (!ValidateSearchInput(address)) return;

            _btnSearch.Enabled = false;
            _txtSearch.Enabled = false;
            _searchProgressBar.Visible = true;

            try
            {
                await SearchAddressAsync(address);
            }
            finally
            {
                _searchProgressBar.Visible = false;
                _btnSearch.Enabled = true;
                _txtSearch.Enabled = true;
            }
        }

        private async Task SearchAddressAsync(string address)
        {
            try
            {
                string encoded = Uri.EscapeDataString(address + ", USA");
                string url = "https://nominatim.openstreetmap.org/search?format=json&q=" + encoded + "&limit=1";

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Add("User-Agent", "DeliveryZoneGIS/1.0");
                    using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        ProcessGeocodingResponse(json, address);
                    }
                }
            }
            catch (HttpRequestException)
            {
                ShowUserFriendlyError("Network error. Check your internet connection.");
            }
            catch (TaskCanceledException)
            {
                ShowUserFriendlyError("Timeout for response from geocoding exceeded.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Geocoding Error] {ex}");
                ShowUserFriendlyError("Unable to restore address search. Please try again later.");
            }
        }

        private void ShowUserFriendlyError(string userMessage)
        {
            _lblDistanceInfo.Text = "⚠️ " + userMessage;
            _lblDistanceInfo.ForeColor = Color.Red;
        }

        private void ProcessGeocodingResponse(string json, string address)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                if (root.GetArrayLength() > 0)
                {
                    float lat = float.Parse(root[0].GetProperty("lat").GetString());
                    float lon = float.Parse(root[0].GetProperty("lon").GetString());

                    _searchedPinLocation = GeometryCalc.LatLonToWebMercator(lat, lon);
                    _searchedAddressLabel = address;

                    SelectNearestStoreToPin(new PointF(lat, lon));
                    CenterMapOnPoint(_searchedPinLocation.Value);
                    if (_btnClearPin != null) _btnClearPin.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Address not found.", "Geocoding", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void HandleGeocodingError(Exception ex)
        {
            string msg = "Error requesting geocoding service:\n" + ex.Message;
            if (ex.InnerException != null) msg += "\nDetails: " + ex.InnerException.Message;
            MessageBox.Show(msg, "Network / API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SelectNearestStoreToPin(PointF pinGeo)
        {
            int nearestIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < _usaRegionGeoCenters.Length; i++)
            {
                double dist = GeometryCalc.CalculateHaversineDistance(pinGeo, _usaRegionGeoCenters[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestIndex = i;
                }
            }
            if (nearestIndex != -1)
            {
                _storeState.SelectedRegionIndex = nearestIndex;
                _selectedStoreIndex = nearestIndex;
                UpdateUIWithPinDistance(nearestIndex, minDistance);
            }
        }

        // Osm Tiles 

        private async void RenderOsmTileLayer(Graphics g)
        {
            string tileKey = "3_2_3";
            if (!_tileCache.ContainsKey(tileKey))
            {
                await LoadOsmTileAsync(3, 2, 3, tileKey);
            }

            if (_tileCache.TryGetValue(tileKey, out Image tileImg))
            {
                g.DrawImage(tileImg, new Rectangle(10, 40, 500, 400));
            }
        }

        private async Task LoadOsmTileAsync(int z, int x, int y, string key)
        {
            try
            {
                string url = "https://tile.openstreetmap.org/" + z + "/" + x + "/" + y + ".png";
                byte[] bytes = await _httpClient.GetByteArrayAsync(url);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    _tileCache[key] = Image.FromStream(ms);
                    this.Invalidate();
                }
            }
            catch { }
        }

        // Mouse Events 

        private void OnMapMouseWheel(object sender, MouseEventArgs e)
        {
            float oldZoom = _zoomLevel;
            _zoomLevel = (e.Delta > 0) ? _zoomLevel * 1.15f : _zoomLevel / 1.15f;
            _zoomLevel = Math.Max(0.5f, Math.Min(_zoomLevel, 5.0f));

            _mapOffset.X -= (e.X - _mapOffset.X) * (_zoomLevel / oldZoom - 1);
            _mapOffset.Y -= (e.Y - _mapOffset.Y) * (_zoomLevel / oldZoom - 1);
            this.Invalidate();
        }

        // Dynamic check of map clicks (via .Left)
        // Instead of using a hardcoded value of 370, we now check the actual left boundary 
        // of the panel via `_rightPanel.Left`. As a result, even when the window is resized, 
        // map clicks will never land on the panel containing the buttons.
        private void OnMapMouseDown(object sender, MouseEventArgs e)
        {
            // If a click occurs within the area of ​​the right-hand panel, ignore the map!
            if (_rightPanel != null && e.X >= _rightPanel.Left) return;

            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                _isDragging = true;
                _lastMousePos = e.Location;
            }
        }
        // 3. Изоляция кликов мыши от карты 

        private void OnMapMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                ExecutePanDrag(e.Location);
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

        private void HandleMapMouseClick(object sender, MouseEventArgs e)
        {
            // Ignore map events if the click is on the right panel
            // Dynamic check: click inside the map, not on the panel
            if (_rightPanel != null && e.X >= _rightPanel.Left) return;

            if (e.Button == MouseButtons.Left && _storeState.HoveredRegionIndex != -1)
            {
                ClearPinState();
                _storeState.SelectedRegionIndex = _storeState.HoveredRegionIndex;
                _selectedStoreIndex = _storeState.SelectedRegionIndex;
                UpdateUI();
                this.Invalidate();
            }
        }

        private void ExecutePanDrag(Point currentPos)
        {
            _mapOffset.X += (currentPos.X - _lastMousePos.X);
            _mapOffset.Y += (currentPos.Y - _lastMousePos.Y);
            _lastMousePos = currentPos;
            this.Invalidate();
        }

        private void ExecuteSpatialHitTest(Point mousePt)
        {
            PointF worldPt = ScreenToWorld(mousePt);
            int mask = _usaRegionsRTree.IntersectsBatch4(worldPt.X, worldPt.Y, worldPt.X, worldPt.Y, 0);

            int lastHover = _storeState.HoveredRegionIndex;
            _storeState.HoveredRegionIndex = PerformNarrowPhaseTest(mask, worldPt);

            if (lastHover != _storeState.HoveredRegionIndex)
                this.Invalidate();
        }

        private int PerformNarrowPhaseTest(int mask, PointF worldPt)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((mask & (1 << i)) != 0 && _usaRegionPaths[i].IsVisible(worldPt))
                    return i;
            }
            return -1;
        }

        private PointF ScreenToWorld(Point screenPt)
        {
            return new PointF(
                (screenPt.X - _mapOffset.X) / _zoomLevel,
                (screenPt.Y - _mapOffset.Y) / _zoomLevel
            );
        }
    }
}
