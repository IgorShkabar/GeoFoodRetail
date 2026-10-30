/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using GeoFoodRetail.GIS;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.UI;
using System.Device.Location;
using GeoFoodRetail.GIS.Math3D;
using static GeoFoodRetail.GIS.Domain.Models.RetailStoreMarker;
using static GeoFoodRetail.GIS.UI.Gis3DGlobeControl;      
using static GeoFoodRetail.GIS.Math3D.DistanceCalculator;

namespace GeoFoodRetail.EnterpriseForm
{
    internal partial class RetailForm : Form
    {        
        private Gis3DGlobeControl _globeControl;
        private Label             _labelDistance;
        private Label             _labelStore;
        private TextBox           _textBoxDistance;
        private TextBox           _textBoxDistBetweenStores;
        private CheckBox          _betweenRetailers;        
        private StoreItem         _firstStore = null;
        private StoreItem         _secondStore = null;
        private int               _clickStep = 1; // 1 — waiting for the first point, 2 1 — waiting for the second one

        public RetailForm()
        {
            Text = "Enterprise Food Retail Networks GIS Platform";            
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            /// <summary>
            /// Important details for a smooth transition:
            /// 1.Maintaining the Aspect Ratio: The 3D engine must recalculate the aspect ratio(Width / Height) whenever the size changes. Otherwise, stretching the window horizontally would cause a sphere to turn into a flattened ellipsoid.
            /// 2.Double Buffering: To prevent the form from flickering when its corner is dragged with the mouse, ensure that double buffering is enabled for the form within its constructor:
            /// </summary>
            /// 
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            InitializeComponent();
            InitCustomComponent();           
            GlobeLogger.Log("System has been launched...");
            mapPanel1.SendToBack();
            LoadRetailData();

            // create controls on the Form itself
            _labelDistance = 
                new Label { Text = "Distance:", Location = new Point(8, 12), AutoSize = true, Font = new Font("Arial", 9.5f, FontStyle.Bold), Visible = true, BackColor = Color.Transparent };
            _labelStore =
                new Label { Text = "From retail to retail:", Location = new Point(10, 42), AutoSize = true, Font = new Font("Arial", 9.5f, FontStyle.Bold), Visible = true, BackColor = Color.Transparent };

            _textBoxDistance = 
                new TextBox { Location = new Point(90, 12), Width = 390, ReadOnly = true, Font = new Font("Arial", 9f, FontStyle.Regular), Visible = true, ForeColor = SystemColors.HotTrack, BackColor = SystemColors.Window };

            _textBoxDistBetweenStores =
                new TextBox { Location = new Point(8, 72), Width = 450, ReadOnly = true, Font = new Font("Arial", 9f, FontStyle.Regular), Visible = true, ForeColor = SystemColors.HotTrack, BackColor = SystemColors.Window };

            _betweenRetailers =
                new CheckBox { Location = new Point(170, 42), Width = 25, Font = new Font("Arial", 9f, FontStyle.Regular), Visible = true, ForeColor = SystemColors.HotTrack };

            Controls.Add(_labelDistance);
            Controls.Add(_labelStore);
            Controls.Add(_textBoxDistance);
            Controls.Add(_textBoxDistBetweenStores);
            Controls.Add(_betweenRetailers);
            _labelDistance.BringToFront();
            _labelStore.BringToFront();
            _textBoxDistance.BringToFront();
            _textBoxDistBetweenStores.BringToFront();
            _betweenRetailers.BringToFront();
            _betweenRetailers.Checked = true;
            DistanceToRetailer();            
        }        

        internal static Gis3DGlobeControl GlobeControl { get; set; }

        // pick up a selected retailer store 
        // compute the distance from your PC to that store
        
        private void DistanceToRetailer()
        {            
            if (_globeControl == null) return;
            _globeControl.StoreSelected += async (sender, store) =>
            {
                try
                {
                    if (store?.Location == null) return;
                    
                    if (_textBoxDistance != null) // CALCULATION based on user / Airport (output to _textBoxDistance)
                    {
                        _textBoxDistance.Text = string.Empty;
                        try
                        {
                            var (userLat, userLon, _) = await _globeControl.GetUserCoordinatesByIpAsync();
                            double distance = 
                                Gis3DGlobeControl.CalculateDistance(userLat, userLon, store.Location.Latitude, store.Location.Longitude);
                            
                            _textBoxDistance.Text = $"{distance:N2} km to {store.Name} from you";
                        }
                        catch
                        {
                            // Offline backup (Ben-Gurion)
                            var (offlineLat, offlineLon, _) = GetUserCoordinatesOffline();
                            double airportDistance = 
                                Gis3DGlobeControl.CalculateDistance(offlineLat, offlineLon, store.Location.Latitude, store.Location.Longitude);
                            
                            _textBoxDistance.Text = $"{airportDistance:N2} km to {store.Name} from Ben Gurion Airport";
                         }
                    }
                    
                    if (_textBoxDistBetweenStores != null) // CALCULATION between 2 point (output to _textBoxDistBetweenStores)
                    {
                        var currentStore = new StoreItem( store.Name ?? "Store",
                                                        store.Location.Latitude,
                                                        store.Location.Longitude );
                        if (_clickStep == 1)
                        {
                            _firstStore = currentStore;
                            _textBoxDistBetweenStores.Text = $"1st: {_firstStore.Name}. Click 2nd store...";
                            _clickStep = 2;
                        }
                        else
                        {
                            _secondStore = currentStore;

                            double distanceBetweenStores = 
                                CalculateHaversineDistance(_firstStore.Latitude, _firstStore.Longitude,
                                                            _secondStore.Latitude, _secondStore.Longitude );

                            _textBoxDistBetweenStores.Text = $"{distanceBetweenStores:N2} km between \"{_firstStore.Name}\" and \"{_secondStore.Name}\"";
                            _clickStep = 1; // Reset for the next measurement
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Click error: {ex.Message}");
                }
            };
        }

        /// <summary>
        /// Implementation for Offline Fallback
        /// Integrate the Windows sensor with a safe fallback coordinate(e.g., your default 
        /// City center) if the device lacks a GPS/Wi-Fi positioning sensor or if it's disabled:
        /// </summary>
        /// <returns>item1, item2, item3</returns>

        internal (double lat, double lon, string city) GetUserCoordinatesOffline()
        {
            try
            {
                using (var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default))
                {
                    watcher.Start();

                    // wait up to 3 seconds for location data
                    if (watcher.TryStart(false, TimeSpan.FromMilliseconds(3000)))
                    {
                        var coord = watcher.Position.Location;
                        if (!coord.IsUnknown)
                        {
                            return (coord.Latitude, coord.Longitude, "Local Device Location");
                        }
                    }
                }
            }
            catch
            {
                // Ignore hardware/permissions errors and fallback
            }
            // Default fallback coordinates if offline and no GPS sensor 
            // (e.g., Tel Aviv Ben Gurion Airport, or your office center)
            return (32.0114, 34.8867, "Default Location (Offline)");
        }

        private void InitCustomComponent()
        {           
            _globeControl = new Gis3DGlobeControl
            {
                Dock = DockStyle.Fill
            };
            GlobeControl = _globeControl;

            // Constructing a safe file path within the application's working directory
            string resourcePath = 
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Resources","earth_map.jpg");

            // Checking for the file's existence before loading
            if (System.IO.File.Exists(resourcePath))
            {
                _globeControl.WorldTexture = new Bitmap(resourcePath);
            }
            else
            {
                MessageBox.Show($"Файл текстуры не найден: {resourcePath}");
            }        

            _globeControl.StoreSelected += (s, store) =>
            {                
                MessageBox.Show($"Chain: {store.RetailChain}\nPoint: {store.Name}\nCoordinates: Lat {store.Location.Latitude}, Lon {store.Location.Longitude}",
                    "GIS Inspection System", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            this.Controls.Add(_globeControl);
        }
        
        private void OnGlobeControlResize(object sender, EventArgs e)
        {
            if (_globeControl.Width == 0 || _globeControl.Height == 0) return;

            // Calculating the minimum side length of a window to preserve 
            // the proportions of a sphere
            var minDimension = Math.Min(_globeControl.Width, _globeControl.Height);

            // If the control has a scale property (Zoom / Radius / CameraDistance):
            // Calculate the new radius/zoom based on the current window size
            float newRadius = minDimension / 2.0f;

            // Example (depends on the properties of my 3D control):
            // globe_control.GlobeRadius = newRadius;
            // or globe_control.ZoomLevel = minDimension * 0.1f;

            _globeControl.Invalidate();
        }        
    }
}