/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * To calculate distances between a user and retail locations in C#, the task is divided into two stages: 
 * Determining coordinates and calculating the distance using geocoordinates.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

/** For this code to work, the Windows UWP/WinRT 
 * namespace must be included at the top of the file: */

namespace GeoFoodRetail.GIS.Domain.Models
{
    internal class GeoDetermination
    {
        public async Task<(double Lat, double Lng)?> GetWindowsLocationAsync()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus != GeolocationAccessStatus.Allowed)
                return null; // User has denied access

            var geolocator = new Geolocator { DesiredAccuracyInMeters = 50 };
            Geoposition pos = await geolocator.GetGeopositionAsync();

            return (pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
        }


        /** By IP address (for the backend, if GPS/Browser access is unavailable)

        If the client has not granted geolocation permission, it can be determined by 
        the server's IP address. This method provides low accuracy (usually at the city or ISP level).
        This method uses databases or APIs such as MaxMind GeoIP2, IpInfo, or IpApi. */

        public async Task<(double Lat, double Lng)?> GetLocationByIpAsync(string ipAddress)
        {
            // for production, it's better to use paid analogs with tokens
            using(var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"http://ip-api.com{ipAddress}");
                using (var doc = JsonDocument.Parse(response))
                {
                    var root = doc.RootElement;
                    if (root.GetProperty("status").GetString() == "success")
                    {
                        return
                            (root.GetProperty("lat").GetDouble(), root.GetProperty("lon").GetDouble());
                    }
                }
            }
#if NET8_0_OR_GREATER
            using var client = new HttpClient();
            // Пример с ip-api.com (для продакшена лучше использовать платные аналоги с токенами)
            var response = await client.GetStringAsync($"http://ip-api.com{ipAddress}");
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            if (root.GetProperty("status").GetString() == "success")
            {
                return (root.GetProperty("lat").GetDouble(), root.GetProperty("lon").GetDouble());
            }
#endif
            return null;
        }
    }
}
