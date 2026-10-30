/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Net.Configuration;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using DatabaseServiceLib;
using GeoFoodRetail.GIS.UI;
using System.Drawing.Drawing2D;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm : MapState
    {      
        public DeliveryZoneForm()
        {
            InitializeComponent();
            
            InitializeGMapIntegrated();
            InitializeStoreMarkers();
            LoadStoreMarkersToGMap();
            ConfigureFormSettings();
            InitUsaGraphicsPaths();
            InitStoresSpatialData();
            InitUI();
            RegisterGisEvents();

            mapPanel1.SendToBack();

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeliveryZoneGIS/1.0");
            _storeState.InitStoreData();            
            InitRTreeData();
        }
        public decimal ExternalCashgivenProperty { get; set; } = 1000.00m;

        private async void DeliveryZoneForm_Load(object sender, EventArgs e)
        {            
            ConfigureSearchTooltipsAndEvents();
            CenterMapOnDefaultUsaLocation();
            await AnimateFormFadeInAsync();                    
            FreeIP_basedLookupService();
            //BuildStoreListUI();

            // Calculate the initial BoundingBox for the default map center
            GeoBoundingBox initialBbox = GetCurrentMapBoundingBox();            
        }

        /// Methods:
        // THIS PART is VERY important!!! 
        private GeoBoundingBox GetCurrentMapBoundingBox()
        {
            // Example of a static box covering the entire territory of the USA or your region
            return new GeoBoundingBox(
                minLat: 24.396308,
                maxLat: 49.384358,
                minLong: -125.000000,
                maxLong: -66.934570
            );
        }

        private async Task AnimateFormFadeInAsync()
        {
            // Smooth appearance of a window
            
            this.Opacity = 0;
            while (this.Opacity < 1.0 && !this.IsDisposed)
            {
                await Task.Delay(15);
                this.Opacity += 0.05;
            }
        }                       

        private void ConfigureRightPanel(Panel rightPanel)
        {
            this._rightPanel = rightPanel;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.BackColor = SystemColors.Window;
            rightPanel.BorderStyle = BorderStyle.FixedSingle;
            rightPanel.BringToFront();
            
            this.Resize += (s, e) => UpdatePanelWidth();
            UpdatePanelWidth();
        }
        private void UpdatePanelWidth()
        {
            if (_rightPanel == null) return;
            // The panel takes up 30% of the window width, but at least 280px
            int calculatedWidth = (int)(this.ClientSize.Width * 0.30);
            _rightPanel.Width = Math.Max(280, calculatedWidth);
        }
        
        private void BuildInformationLabels(Panel rightPanel)
        {
            _lblCashInfo = new Label { Left = 15, Top = 90, Width = 335, Height = 25, Font = new Font("Consolas", 10, FontStyle.Bold), ForeColor = Color.DarkBlue, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _lblSelectedInfo = new Label { Text = "Please select a region on the map!", Left = 15, Top = 120, Width = 335, Height = 45, Font = new Font("Arial", 9.5f, FontStyle.Regular), ForeColor = Color.Red, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _lblDistanceInfo = new Label { Left = 15, Top = 175, Width = 335, Height = 250, Font = new Font("Consolas", 8.5f, FontStyle.Regular), ForeColor = Color.Black, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };

            rightPanel.Controls.Add(_lblCashInfo);
            rightPanel.Controls.Add(_lblSelectedInfo);
            rightPanel.Controls.Add(_lblDistanceInfo);
        }            
    }
}