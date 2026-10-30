/** 
 Copyright (c) 2026 Igor Shkabar. All rights reserved.
 For calculating distances between points on the Earth's surface using the Haversine formula 
 - implementation in C#. Getting distance to the store.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.GIS.Projections;
using GeoFoodRetail.EnterpriseForm; 
using GeoFoodRetail.GIS.Domain.Models;
using GeoFoodRetail.GIS.UI;
using Windows.Devices.Geolocation;

namespace GeoFoodRetail.GIS.Math3D
{
    internal class DistanceCalculator
    {
        // Average radius of the Earth in kilometers.
        // Used to approximate the Earth as a perfect sphere. RadiusInKilometers

        /// <summary>
        /// Calculates the distance between two points on the Earth's sphere using the haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of the first point (in degrees)</param>
        /// <param name="lon1">Longitude of the first point (in degrees)</param>
        /// <param name="lat2">Latitude of the second point (in degrees)</param>
        /// <param name="lon2">Longitude of the second point (in degrees)</param>
        /// <returns>Distance in kilometers</returns>

        public static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Converting degrees to radians
            double radLat1 = DegreesToRadians(lat1);
            double radLon1 = DegreesToRadians(lon1);
            double radLat2 = DegreesToRadians(lat2);
            double radLon2 = DegreesToRadians(lon2);

            // Difference in coordinates
            double diffrLat = radLat2 - radLat1;
            double diffrLon = radLon2 - radLon1;

            // Calculation of the haversine of an angle (the square of half the chord between points)
            double a = Math.Sin(diffrLat / 2) * Math.Sin(diffrLat / 2) +
                       Math.Cos(radLat1) * Math.Cos(radLat2) *
                       Math.Sin(diffrLon / 2) * Math.Sin(diffrLon / 2);

            // Calculation of angular distance in radians (central angle)
            double angularDistance = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Total great-circle distance
            return Constants.EarthConst.RadiusInKilometers * angularDistance;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        // To make the process seamless, automatically obtaining geolocation via 
        // APIs instead of manual entry is a great and standard solution.
        public static async Task<double?> GetDistanceToStoreAsync(double storeLat, double storeLon)
        {
            try
            {
                // 1. Проверяем доступ к геолокации в Windows
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus != GeolocationAccessStatus.Allowed)
                {
                    // Доступ запрещен пользователем, возвращаем null (нужен фолбек на ручной ввод)
                    return null;
                }

                // 2. Инициализируем геолокатор
                var geolocator = new Geolocator
                {
                    DesiredAccuracyInMeters = 50 // Нам не нужна точность до сантиметра для поиска ретейл-сетей
                };

                // Получаем текущую позицию
                Geoposition pos = await geolocator.GetGeopositionAsync();
                double userLat = pos.Coordinate.Point.Position.Latitude;
                double userLon = pos.Coordinate.Point.Position.Longitude;

                // 3. Считаем расстояние по формуле Гаверсинусов
                return CalculateHaversineDistance(userLat, userLon, storeLat, storeLon);
            }
            catch (Exception)
            {
                // Обработка ситуации, когда GPS выключен в системе
                return null;
            }
        }

        public static async Task<double?> GetDistanceBetweenStores(double lat1, double lon1, double lat2, double lon2)
        {
            var geolocator = new Geolocator
            {
                DesiredAccuracyInMeters = 50 // Meter-level precision is not required for locating retail chains.
            };
            Geoposition pos = await geolocator.GetGeopositionAsync();
            return CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        }        
    }
}

