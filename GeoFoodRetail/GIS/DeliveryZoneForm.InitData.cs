/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using GeoFoodRetail.GIS.Math3D;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm
    {
        public void BuildStoreListUI()
        {  
            Label lblStoresTitle = new Label
            {
                Text = "Retail Chains/Stores:",
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 5)
            };
            _mainRightContainer.Controls.Add(lblStoresTitle);
            // Retrieve items from the Store and display their names
            var storeMarkers = Domain.Models.StoreData.Store;
            int countToDisplay = Math.Min(38, storeMarkers.Count);
            
            for (int i = 0; i < countToDisplay; i++)
            {                
                var currentMarker = storeMarkers[i];
                var marker = storeMarkers[i];

                Label lblStoreItem = new Label
                {
                    Text = $"• {marker.Name}",
                    AutoSize = true,
                    Font = new Font("Arial", 8.5f),
                    ForeColor = Color.FromArgb(40, 40, 40),
                    BackColor = Color.Transparent, 
                    Margin = new Padding(5, 2, 0, 2),
                    Cursor = Cursors.Hand
                };
                      
                lblStoreItem.Click += (s, e) =>
                {
                    // Interactive: when you click on a store, you can center the map or highlight a pin
                    _txtSearch.Text = marker.Name;
                    // Here you can call a method to focus on the coordinates marker.Coordinate

                    double lat = marker.Location.Latitude; // X; 
                    double lon = marker.Location.Longitude; // Y;
                    
                    ApplyFoundCoordinates(lat, lon, $"Store: {marker.Name}");

                    // Find the index of this store to calculate distances and update the UI
                    int idx = Domain.Models.StoreData.Store.IndexOf(marker);
                    if (idx != -1)
                    {
                        _selectedStoreIndex = idx;
                        _storeState.SelectedRegionIndex = idx;

                        // Calculate the distance to the same point (will be 0 km) or from the current pin
                        double dist = _searchedPinLocation.HasValue ?
                            GeometryCalc.CalculateHaversineDistance(new PointF((float)lat, (float)lon), _storeCoordArray[idx]) : 0;

                        UpdateUIWithCoordinates(idx, dist, (float)lat, (float)lon);
                    }
                    // Force the form to redraw so that the pin is rendered immediately
                    this.Invalidate();
                };
                _mainRightContainer.Controls.Add(lblStoreItem);
            } 
        }

        //  Spatial Data Initialization 
        private void InitStoresSpatialData()
        {            
            _storePixelPositions.Clear();
            _storeNames.Clear();

            for (int i = 0; i < _storeCoordArray.Length; i++)
            {
                // Convert Lat/Lon from _storeCoordArray to Web Mercator
                PointF mercatorPoint = LatLonToWebMercator(_storeCoordArray[i].X, _storeCoordArray[i].Y);
                _storePixelPositions.Add(mercatorPoint);
                _storeNames.Add(StoreNameArr[i].ToString());
            }
        }

        private unsafe void InitRTreeData()
        {
            // Initialize the R-tree structure (since it's a struct, we call new)
            _usaRegionsRTree = new GIS.StreamingRTree.RTreePage();
            _usaRegionsRTree.IsLeaf = 1;
            _usaRegionsRTree.Count = 4;

            // To modify the fixed buffer directly, we need an unsafe context.
            // Pin the pointer to BoxesData within our RTreePage instance.
            fixed (float* boxesPtr = _usaRegionsRTree.BoxesData)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_usaRegionPaths[i] != null)
                    {
                        RectangleF bounds = _usaRegionPaths[i].GetBounds();

                        // Write min_X to indices 0, 1, 2, 3
                        boxesPtr[i] = bounds.Left;
                        // min_Y -> 4, 5, 6, 7
                        boxesPtr[i + 4] = bounds.Top;
                        // max_X -> 8, 9, 10, 11
                        boxesPtr[i + 8] = bounds.Right;
                        //  max_Y -> 12, 13, 14, 15
                        boxesPtr[i + 12] = bounds.Bottom;
                    }
                }
            }
        }

        private void InitUsaGraphicsPaths()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "us-states.json");
                if (File.Exists(filePath))
                {
                    ParseUsaGeoJson(filePath);
                    return; // States successfully loaded from file!
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading us-states.json: " + ex.Message);
            }
            // Fallback for the case of a file-reading error. It will only trigger if the file does not exist at all.
            CreateFallbackUsaPaths();
        }
        private void ParseUsaGeoJson(string path)
        {           
            string json = File.ReadAllText(path);
            using (var doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                var features = root.GetProperty("features");
                int index = 0;

                foreach (var feature in features.EnumerateArray())
                {
                    if (index >= _usaRegionPaths.Length) break;

                    if (feature.TryGetProperty("geometry", out var geometry) && geometry.ValueKind != JsonValueKind.Null)
                    {
                        string type = geometry.GetProperty("type").GetString();
                        var coords = geometry.GetProperty("coordinates");

                        _usaRegionPaths[index] = BuildPathFromCoordinates(type, coords);

                        // Debugging: output the state name if it exists in properties
                        string stateName = "Unknown";
                        if (feature.TryGetProperty("properties", out var props) && props.TryGetProperty("name", out var nameProp))
                        {
                            stateName = nameProp.GetString();
                        }
                        System.Diagnostics.Debug.WriteLine($"Loaded State [{index}]: {stateName}");

                        index++;
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Total states loaded: {index}");
            }
            CenterMapOnUsaBounds();
        }
        private void CenterMapOnUsaBounds()
        {
            // Coordinates of the center of the contiguous United States (Kansas / geographic center)
            PointF centerLatLon = new PointF(39.8283f, -98.5795f);
            PointF centerWorldPt = LatLonToWebMercator(centerLatLon.X, centerLatLon.Y);
            CenterMapOnPoint(centerWorldPt);
        }
        private GraphicsPath BuildPathFromCoordinates(string geoType, JsonElement coordsElement)
        {
            var path = new GraphicsPath();
            if (geoType == "Polygon")
            {                
                if (coordsElement.GetArrayLength() > 0)
                {
                    AddPolygonRingToPath(path, coordsElement[0]);
                }
            }
            else if (geoType == "MultiPolygon")
            {                
                foreach (var polygon in coordsElement.EnumerateArray())
                {
                    if (polygon.GetArrayLength() > 0)
                    {
                        AddPolygonRingToPath(path, polygon[0]);
                    }
                }
            }
            path.CloseFigure();
            return path;
        }
        private void AddPolygonRingToPath(GraphicsPath path, JsonElement ring)
        {
            List<PointF> points = new List<PointF>();
            foreach (var pt in ring.EnumerateArray())
            {
                float lon = pt[0].GetSingle();
                float lat = pt[1].GetSingle();
                
                PointF pixelPt = LatLonToWebMercator(lat, lon);
                points.Add(pixelPt);
            }
            if (points.Count > 2)
            {                
                path.StartFigure();
                path.AddLines(points.ToArray());
                path.CloseFigure();
            }
        }
        
        private void CreateFallbackUsaPaths()
        {            
            _usaRegionPaths[0] = new GraphicsPath();
            _usaRegionPaths[0].AddRectangle(new RectangleF(50, 50, 200, 200));
        }
        
        private GraphicsPath CreatePolygon(PointF[] pts)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(pts);
            return path;
        }
    }
}

