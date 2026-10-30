/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved. 
 */
using System;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.EnterpriseForm;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class Gis3DGlobeControl : Control
    {
        internal async void OnStoreMarkerClicked(RetailStoreMarker store)
        {
            try
            {                
                var (userLat, userLon, _) = await GetUserCoordinatesByIpAsync();

                // Trigger a form event after receiving the coordinates
                StoreSelected?.Invoke(this, store);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }        

        /* An optimized version of the `CalculateDistance` method that avoids formula duplication */
        internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            bool no_internet = !NetworkInterface.GetIsNetworkAvailable();

            //  No any networks connections -> fallback to Ben Gurion Airport
            if (no_internet == true)
            {
                lat1 = 32.0114;
                lon1 = 34.8867;                                
            }  // end: My code
            //const double EarthRadiusKm = 6371.0088;
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return Constants.EarthConst.RadiusInKilometers * (2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));
        }

        internal async Task<(double lat, double lon, string city)> GetUserCoordinatesByIpAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                string json = await client.GetStringAsync("http://ip-api.com/json/");

                using (var doc = System.Text.Json.JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    if (root.GetProperty("status").GetString() == "success")
                    {
                        double lat = root.GetProperty("Lat").GetDouble();
                        double lon = root.GetProperty("Lon").GetDouble();
                        string city = root.GetProperty("City").GetString() ?? "Unknown";
                        return (lat, lon, city);
                    }
                }
            }
            throw new Exception("Failed to retrieve location from IP-API.");
        }
    }
}
