/** 
 * Copyright (c) 2026 Igor Shkabar. All Rights reserved.
 * The Form to add 50_000 points/objects to the Map.
 * The class I am testing (Component under test).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.GIS.Domain.Models;
using DatabaseServiceLib;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace GeoFoodRetail.GIS.AddManyObjects
{
    public partial class Add50KPointsToMap : Form
    {
        private readonly IDatabaseService _dbService;
        private readonly IStoreRepository _repository;

        // declare a collection of points and a visibility scope at the class level
        private List<MapPointItem> allPoints = new List<MapPointItem>();
        // Example of initial coverage
        private RectangleF currentViewPort = new RectangleF(-180, -90, 360, 180); 
                

        public Add50KPointsToMap()
        {
            InitializeComponent();            
        }
        public Add50KPointsToMap(IDatabaseService dbService)
        {
            InitializeAsync();
            _dbService = dbService;
            InitializeAsync();
            _ = LoadInitialDataAsync(); // Initiating an asynchronous load in a background thread
        }

        public Add50KPointsToMap(IStoreRepository repo)
        {
            _repository = repo;
        }

        private async void InitializeAsync()
        {
            // Initialize of the WebView2 environment
            var env = await CoreWebView2Environment.CreateAsync(null, null);
            await webViewMap.EnsureCoreWebView2Async(env);

            // Option A: If the React application is running locally during the development phase:
            webViewMap.CoreWebView2.Navigate("http://localhost:5173");

            // Option B: If you have built your React app (npm run build) into a "dist" folder:
            // string htmlPath = Path.Combine(Application.StartupPath, "dist", "index.html");
            // webViewMap.CoreWebView2.Navigate(htmlPath);

            // Enable DevTools for WebGL debugging if necessary
            webViewMap.CoreWebView2.Settings.AreDevToolsEnabled = true;            
        }

        private async Task LoadInitialDataAsync()
        {
            string query = 
                "SELECT STORE_ID, STORE_NUMBER, SDO_UTIL.GET_X(STORE_GEOM) AS LON, SDO_UTIL.GET_Y(STORE_GEOM) AS LAT FROM C##GIS_USER.STORES_LOCATIONS";

            // Execute the request in the background, without freezing the UI
            DataTable dt = await Task.Run(() => _dbService.GetDataTable(query));

            allPoints = await Task.Run(() => dt.AsEnumerable().Select(row => new MapPointItem
            {
                Id = row.Field<int>("STORE_ID"),
                Name = row.Field<string>("STORE_NUMBER"),
                X = Convert.ToSingle(row["LON"]),
                Y = Convert.ToSingle(row["LAT"])
            }).ToList());
        }

        public void LoadInitialData()
        {
            // Populate `allPoints` here with data from the database (for example, using `_dbService.GetDataTable`).
            // The query selects coordinates and an ID (assuming your database contains LONGITUDE and LATITUDE columns, or that the data is extracted via SDO_GEOM).
            string query = @"
                SELECT STORE_ID, STORE_NUMBER, 
                        SDO_UTIL.GET_X(STORE_GEOM) AS LON, 
                        SDO_UTIL.GET_Y(STORE_GEOM) AS LAT 
                FROM C##GIS_USER.STORES_LOCATIONS";

            var stores = _repository.GetStoresData();
            foreach (var store in stores)
            {
                var storeId = store.StoreId;
                var storeNmb = store.StoreNmb;
                var lon = store.Lon;
                var lat = store.Lat;
            }
            try
            {
                // We retrieve a DataTable via my secured service
                DataTable dt = _dbService.GetDataTable(query);

                // Quickly convert a DataTable into a list of lightweight MapPointItem objects
                allPoints = dt.AsEnumerable().Select(row => new MapPointItem
                {
                    Id = row.Field<int>("STORE_ID"),
                    Name = row.Field<string>("STORE_NUMBER"),
                    X = Convert.ToSingle(row["LON"]), // Longitude
                    Y = Convert.ToSingle(row["LAT"])  // Latitude
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading points for the map: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // The button on the Form that changes color of 50,000 points on the map
        private async  void btnChangeColor_Click(object sender, EventArgs e)
        {
            // Fast filtering of a large list of points using multiple threads.
            // Parallel filtering of 50,000+ points using all processor cores.
            var visiblePoints = await Task.Run(() =>
                allPoints
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .Where(p => currentViewPort.Contains(p.X, p.Y))
                    .ToList()
            );

            if (webViewMap.CoreWebView2 != null)
            {
                var messageObject = new
                {
                    action = "changeColor",
                    color = "#007AFF",
                    pointsCount = visiblePoints.Count // The filtered points can be passed for rendering.
                };

                string jsonMessage = JsonSerializer.Serialize(messageObject);
                webViewMap.CoreWebView2.PostWebMessageAsJson(jsonMessage);
            }
        }        
        
        private PointF GeoToScreen(GeoCoordinate3D location)
        {
            throw new NotImplementedException();
        }
    }    
}

