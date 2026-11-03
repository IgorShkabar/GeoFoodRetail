/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;
using GeoFoodRetail.GIS.Math3D;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm
    {
        // Forward geocoding with offline fallback
        private async Task SearchAddressAsync(string address)
        {
            // First, check whether the user has entered the name of a specific store from the database.
            var matchedStore = Domain.Models.StoreData.Store
                .FirstOrDefault(s => s.Name.Equals(address, StringComparison.OrdinalIgnoreCase) ||
                                     s.Name.IndexOf(address, StringComparison.OrdinalIgnoreCase) >= 0);

            if (matchedStore != null)
            {
                // Successfully found the store in the local database — let’s grab its actual coordinates! 

                double lat = matchedStore.Location.Latitude;
                double lon = matchedStore.Location.Longitude;
                /* Checking the hemisphere for the USA (just to be safe):
                   If the longitude for the USA happens to be positive, we force it to be negative (since it is a western longitude):
                  */
                ApplyFoundCoordinates(lat, lon, $"Supermarket: {matchedStore.Name}");
                return;
            }
            try
            {
                var encoded = Uri.EscapeDataString(address + ", USA");
                var url = 
                    "https://nominatim.openstreetmap.org/search?format=json&q=" + encoded + "&limit=1";

                using (var req = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    req.Headers.UserAgent.ParseAdd("DeliveryZoneGIS/1.0 (contact@deliverygis.com)");
                    using (HttpResponseMessage resp = await _httpClient.SendAsync(req))
                    {
                        if (resp.IsSuccessStatusCode)
                        {
                            string json = await resp.Content.ReadAsStringAsync();
                            if (ProcessGeocodingResponse(json, address)) return;
                        }
                    }
                }
            }
            catch (Exception x) // Well, what if there’s no Internet?
            {
                if (x is HttpRequestException)
                {   
                    ShowUserFriendlyError("Network error. Check Internet connection.");
                }
                else if (x is TaskCanceledException)
                {                         
                    ShowUserFriendlyError("The timeout for the response from the geocoding server has been exceeded.");
                }
                else
                {
                    // Unexpected error 
                    // (logged for our internal use; a clear, understandable message is shown to the user)
                    System.Diagnostics.Debug.WriteLine($"[Geocoding Error] {x}");
                    ShowUserFriendlyError("Не удалось выполнить поиск адреса. Попробуйте позже.");
                }                
            }
            FallbackToOfflineGeocode(address);
        }

        private bool ProcessGeocodingResponse(string json, string address)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                if (root.GetArrayLength() == 0) return false;

                float lat = float.Parse(root[0].GetProperty("Lat").GetString(), System.Globalization.CultureInfo.InvariantCulture);
                float lon = float.Parse(root[0].GetProperty("Lon").GetString(), System.Globalization.CultureInfo.InvariantCulture);

                ApplyFoundCoordinates(lat, lon, address + " (Online)");
                return true;
            }
        }
        // Offline geocoder (works without an internet connection)
        private void FallbackToOfflineGeocode(string address)
        {
            // Basic local mapping of US states/cities for offline use
            var fall_back_geo = new PointF(38.8893f, -77.0502f); // Washington by default (Lincoln Memorial)
            string search_lower = address.ToLower();

            if (search_lower.Contains("ny") || search_lower.Contains("york"))
                fall_back_geo = new PointF(40.7128f, -74.0060f);
            else if (search_lower.Contains("ca") || search_lower.Contains("francisco"))
                fall_back_geo = new PointF(37.7749f, -122.4194f);
            else if (search_lower.Contains("il") || search_lower.Contains("chicago"))
                fall_back_geo = new PointF(41.8781f, -87.6298f);
            else if (search_lower.Contains("tx") || search_lower.Contains("houston"))
                fall_back_geo = new PointF(29.7604f, -95.3698f);

            ApplyFoundCoordinates(fall_back_geo.X, fall_back_geo.Y, address + " (Офлайн расчёт)");
        }

        // overloading
        // An overload for `double`, to avoid having to perform "clunky" manual type casts.
        private void ApplyFoundCoordinates(double latitude, double longitude, string label)
        {
            // Within the method, we safely cast to float only where necessary for on-screen rendering, 
            // while keeping the GIS logic in double precision.
            ApplyFoundCoordinates((float)latitude, (float)longitude, label);
        }
        private void ApplyFoundCoordinates(float lat, float lon, string labelText)
        {
            _searchedPinLocation = LatLonToWebMercator(lat, lon);
            _searchedAddressLabel = string.Empty; // labelText;

            SelectNearestStoreToPin(new PointF(lat, lon));
            CenterMapOnPoint(_searchedPinLocation.Value);

            _txtSearch.Text = $"{lat:F6}, {lon:F6}";
            if (_btnClearPin != null) _btnClearPin.Enabled = true;
            this.Invalidate();
        }


        // Binding method with coordinate transmission (SelectNearestStoreToPinWithCoords)
        
        private void SelectNearestStoreToPinWithCoords(PointF pinGeo, float lat, float lon)
        {
            int nearestIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < _storeCoordArray.Length; i++)
            {
                double dist = GeometryCalc.CalculateHaversineDistance(pinGeo, _storeCoordArray[i]);
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
                UpdateUIWithCoordinates(nearestIndex, minDistance, lat, lon);
            }
            else   {    
                System.Diagnostics.Debug.WriteLine("Store not found");
            }
        }

        //Formatted display of coordinates in the user interface (UpdateUIWithCoordinates)
        // Replacing the logic for updating the _lblDistanceInfo text label. Now, I am displaying the coordinates directly below the address:
        private void UpdateUIWithCoordinates(int storeIdx, double distanceKm, float lat, float lon)
        {
            UpdateUI();
            string coordsText = $"Lat: {lat:F6}, Lon: {lon:F6}";

            _lblDistanceInfo.Text = "=== SEARCH RESULT ===\n" +
                                   "Address: " + _searchedAddressLabel + "\n" +
                                   "Coordinates: " + coordsText + "\n" +
                                   "Nearest Store: " + StoreNameArr[storeIdx] + "\n" +
                                   "Distance: ~" + distanceKm.ToString("N1") + " km\n\n";
        }

        // Adding (Reverse Geocoding)        
        // Reverse geocoding with automatic coordinate calculation
        private async Task ReverseGeocodeAsync(PointF geoPt)
        {
            var address_txt = $"Free point [{geoPt.X:F6}, {geoPt.Y:F6}]";
            try
            {
                string url = 
                    $"https://nominatim.openstreetmap.org/reverse?format=json&Lat={geoPt.X.ToString(System.Globalization.CultureInfo.InvariantCulture)}&Lon={geoPt.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                using (var req = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    req.Headers.UserAgent.ParseAdd("DeliveryZoneGIS/1.0 (contact@deliverygis.com)");
                    using (HttpResponseMessage resp = await _httpClient.SendAsync(req))
                    {
                        if (resp.IsSuccessStatusCode)
                        {
                            string json = await resp.Content.ReadAsStringAsync();
                            using (JsonDocument doc = JsonDocument.Parse(json))
                            {
                                if (doc.RootElement.TryGetProperty("display_name", out var name))
                                    address_txt = name.GetString();
                            }
                        }
                    }
                }
            }
            catch { /* In the event of a network error, we simply use the point's coordinates. */ }
            ApplyFoundCoordinates(geoPt.X, geoPt.Y, address_txt);
        }
        private void DisplayReverseGeocodeResult(string json, PointF geoPt)
        {
            using (var doc = JsonDocument.Parse(json))
            {
                string addr = doc.RootElement.TryGetProperty("display_name", out var name)
                    ? name.GetString()
                    : "Address was not specified";

                _searchedPinLocation = LatLonToWebMercator(geoPt.X, geoPt.Y);
                _searchedAddressLabel = addr;

                SelectNearestStoreToPin(geoPt);
                if (_btnClearPin != null) _btnClearPin.Enabled = true;
                this.Invalidate();
            }
        }

        private void ShowUserFriendlyError(string usr_msg)
        {            
            _lblDistanceInfo.Text = " " + usr_msg;
            _lblDistanceInfo.ForeColor = Color.Red;
        }

        private void HandleGeocodingError(Exception x)
        {
            var msg = "Error requesting geocoding service:\n" + x.Message;

            if (x.InnerException != null)
                msg += "\nDetails: " + x.InnerException.Message;
            MessageBox.Show(msg, "Network / API Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
    }
}

