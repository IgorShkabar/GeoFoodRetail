/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;
using GMap.NET;
using GMap.NET.WindowsForms;
using GeoFoodRetail.GIS.StreamingRTree;
using GeoFoodRetail.GIS.Math3D;
using GeoFoodRetail.GIS.UI;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm 
    { 
        // GMap.NET lib through the NuGet pack   
        private void InitializeGMapIntegrated()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.ServicePointManager.SecurityProtocol =
                (System.Net.SecurityProtocolType)3072 | (System.Net.SecurityProtocolType)768 | (System.Net.SecurityProtocolType)192;

            // enforcing the use of the modern TLS security protocol within the main form constructor, prior to loading the map:
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            
            _gMapControl1 = new GMapControl();

            GMap.NET.MapProviders.GMapProvider.UserAgent = "DeliveryZoneGIS/1.0 (Contact: admin@delivery.local)";

            // Expand the map to fill the entire form, while leaving space for the right panel 
            // (assuming that _rightPanel is already set to DockStyle.Right or has a specified width)
            _gMapControl1.Dock = DockStyle.Fill;
            // Configuring the OpenStreetMap provider
            _gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            // Instead of OpenStreetMap, use ArcGIS, which does not ban standard GMap requests.
            _gMapControl1.MapProvider = GMap.NET.MapProviders.ArcGIS_World_Street_MapProvider.Instance;

            _gMapControl1.Position = new PointLatLng(39.8283, -98.5795);  // US Center and Zoom
            _gMapControl1.MinZoom = 2;
            _gMapControl1.MaxZoom = 18;
            _gMapControl1.Zoom = 5;       
            // Mouse control: hold down the left button to move
            _gMapControl1.DragButton = MouseButtons.Left; 
            _gMapControl1.ShowTileGridLines = false;
            // Add the map to the form—place it at the back (so that it doesn’t overlap your `_rightPanel`)
            this.Controls.Add(_gMapControl1);
            _gMapControl1.SendToBack(); // Move the map to the background, beneath the right-hand panel
        }

        private void LoadStoreMarkersToGMap()
        {
            // create a layer for markers
            GMapOverlay markersOverlay = new GMapOverlay("markers");

            for (int i = 0; i < _storeCoordArray.Length; i++)
            {
                var storeCoord = _storeCoordArray[i];
                var pointLatLng = new GMap.NET.PointLatLng(storeCoord.X, storeCoord.Y);

                var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(pointLatLng, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red);

                // retrieve the store name by index (if it exists in _storeNames)
                    
                if (i < _storeNames.Count)
                {
                    marker.ToolTipText = _storeNames[i];
                }
                markersOverlay.Markers.Add(marker);
            }
            _gMapControl1.Overlays.Add(markersOverlay); // Add a layer to the map
        }

        private void InitializeStoreMarkers()
        {
            _storePixelPositions.Clear();
            _storeNames.Clear();

            foreach (var store in Domain.Models.StoreData.Store)
            {
                // Get the latitude and longitude from the store
                float lat = (float)store.Location.Latitude;
                float lon = (float)store.Location.Longitude;

                // Convert geographic coordinates into screen pixels 
                // (Use the same projection function that you are already using for the map/pins)
                PointF pixelPos = LatLonToWebMercator(lat, lon);

                _storePixelPositions.Add(pixelPos);
                // Use the store's actual name instead of "MapMarker"
                _storeNames.Add(store.Name);
            }
        }
        // Free IP-based location lookup service
        private async void FreeIP_basedLookupService()
        {
            // Automatic location detection via IP (GeoIP Init)
            // Upon application startup, you can immediately determine the user's approximate 
            // city /state based on their IP address, place a marker on the map, and center the map accordingly—all without requiring manual input:
            try
            {
                string json = await _httpClient.GetStringAsync("https://ipapi.co/json/");
                using (var doc = JsonDocument.Parse(json))
                {
                    float lat = doc.RootElement.GetProperty("latitude").GetSingle();
                    float lon = doc.RootElement.GetProperty("longitude").GetSingle();
                    string city = doc.RootElement.GetProperty("City").GetString();

                    _searchedPinLocation = LatLonToWebMercator(lat, lon);
                    _searchedAddressLabel = $"Your location (IP): {city}\nCoordinates: {lat:F4}, {lon:F4}";

                    SelectNearestStoreToPin(new PointF(lat, lon));
                    CenterMapOnPoint(_searchedPinLocation.Value);

                    if (_btnClearPin != null) _btnClearPin.Enabled = true;
                    this.Invalidate(); // Force the map to redraw with a new marker
                }
            }
            catch
            {
                // Ignore network errors at startup; the map will remain at its default scale.
            }
        }
        private void ConfigureSearchTooltipsAndEvents()
        {
            // Quickly clearing focus and setting tooltips
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(_txtSearch, "Введите адрес супермаркета (например: Walmart, Austin TX)");
            toolTip.SetToolTip(_btnClearPin, "Сбросить текущую метку с карты");

            // Handling the Enter key press in a search field
            _txtSearch.KeyDown += (s, args) => {
                if (args.KeyCode == Keys.Enter) { OnSearchClick(s, args); args.SuppressKeyPress = true; }
            };
            EnableBuiltInAutocomplete();
        }
        private void CenterMapOnDefaultUsaLocation()
        {
            // Initial map centering on the center of the United States (39.8283° N, -98.5795° W)
            PointF usaCenterWorld = LatLonToWebMercator(39.8283f, -98.5795f);
            CenterMapOnPoint(usaCenterWorld);
        }

        private void EnableBuiltInAutocomplete()
        {
            // Enable the built-in autocomplete feature with popular U.S. chains.
            var storeSuggestions = new AutoCompleteStringCollection();
            storeSuggestions.AddRange(new[]
            {
                "Walmart Supercenter, New York",
                "Target, Chicago",
                "Whole Foods Market, Houston",
                "Costco Wholesale, Los Angeles"
            });
            _txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            _txtSearch.AutoCompleteCustomSource = storeSuggestions;
        }

        private void ConfigureFormSettings()
        {
            this.Text = "GIS Map USA - Geocoding API"; 
            this.Size = new Size(1000, 550);
            this.MinimumSize = new Size(800, 450);
            this.DoubleBuffered = true;
        }

        // Geocoding API    

        private void SelectNearestStoreToPin(PointF pinGeo)
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
                UpdateUIWithPinDistance(nearestIndex, minDistance);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Store not found"); 
            }
        }

        private void UpdateUIWithPinDistance(int storeIdx, double distanceKm)
        {
            UpdateUI();
            PointF geoCoords = _searchedPinLocation.HasValue
                ? WebMercatorToLatLon(_searchedPinLocation.Value)
                : new PointF(0, 0);
            
            string storeName = (storeIdx >= 0 && storeIdx < StoreNameArr.Count)
                ? StoreNameArr[storeIdx]
                : $"{storeIdx + 1}";

            _lblDistanceInfo.ForeColor = Color.Black;
            _lblDistanceInfo.Text = "=== GEOCODING RESULT ===\n" +
                                    $"Address: {_searchedAddressLabel}\n" +
                                    $"Coordinates: LAT {geoCoords.X:F6}, LON {geoCoords.Y:F6}\n" +
                                    $"Nearest Store: {storeName}\n" +
                                    $"Distance: ~{distanceKm:N1} km\n\n" +
                                    _lblDistanceInfo.Text;
        }

        private void ExecuteSpatialHitTest(Point mousePt)
        {
            PointF world_Pt = ScreenToWorld(mousePt);
            int mask = _usaRegionsRTree.IntersectsBatch4(world_Pt.X, world_Pt.Y, world_Pt.X, world_Pt.Y, 0);

            int last_hover = _storeState.HoveredRegionIndex;
            _storeState.HoveredRegionIndex = PerformNarrowPhaseTest(mask, world_Pt);

            if (last_hover != _storeState.HoveredRegionIndex)
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
    }
}
