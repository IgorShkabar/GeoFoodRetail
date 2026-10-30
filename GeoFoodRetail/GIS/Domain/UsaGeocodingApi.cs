/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.Domain.Models;

namespace GeoFoodRetail.GIS.Domain
{
    internal class UsaGeocodingApi
    {
        private readonly List<GeoAddress> addressDatabase = new List<GeoAddress>();
        
        internal UsaGeocodingApi()
        {
            InitAddressDatabase();
        }

        public static List<GeoAddress> Address { get; set; }

        private void InitAddressDatabase()
        {
            addressDatabase.Clear();
            // States & capitals USA
            addressDatabase.AddRange(new[]
            {
                new GeoAddress { Street = "100 S Union St", City = "Montgomery", State = "AL", Lat = 32.3770f, Lon = -86.3006f },
                new GeoAddress { Street = "120 Fourth St", City = "Juneau", State = "AK", Lat = 58.3019f, Lon = -134.4197f },
                new GeoAddress { Street = "1700 W Washington St", City = "Phoenix", State = "AZ", Lat = 33.4484f, Lon = -112.0740f },
                new GeoAddress { Street = "500 Woodlane St", City = "Little Rock", State = "AR", Lat = 34.7465f, Lon = -92.2896f },
                new GeoAddress { Street = "1315 10th St", City = "Sacramento", State = "CA", Lat = 38.5766f, Lon = -121.4936f },
                new GeoAddress { Street = "200 E Colfax Ave", City = "Denver", State = "CO", Lat = 39.7392f, Lon = -104.9848f },
                new GeoAddress { Street = "210 Capitol Ave", City = "Hartford", State = "CT", Lat = 41.7637f, Lon = -72.6851f },
                new GeoAddress { Street = "411 Legislative Ave", City = "Dover", State = "DE", Lat = 39.1573f, Lon = -75.5197f },
                new GeoAddress { Street = "400 S Monroe St", City = "Tallahassee", State = "FL", Lat = 30.4381f, Lon = -84.2816f },
                new GeoAddress { Street = "206 Washington St SW", City = "Atlanta", State = "GA", Lat = 33.7490f, Lon = -84.3880f },
                new GeoAddress { Street = "415 S Beretania St", City = "Honolulu", State = "HI", Lat = 21.3070f, Lon = -157.8584f },
                new GeoAddress { Street = "700 W Jefferson St", City = "Boise", State = "ID", Lat = 43.6177f, Lon = -116.1997f },
                new GeoAddress { Street = "401 S 2nd St", City = "Springfield", State = "IL", Lat = 39.7983f, Lon = -89.6549f },
                new GeoAddress { Street = "200 W Washington St", City = "Indianapolis", State = "IN", Lat = 39.7684f, Lon = -86.1581f },
                new GeoAddress { Street = "1007 E Grand Ave", City = "Des Moines", State = "IA", Lat = 41.5910f, Lon = -93.6037f },
                new GeoAddress { Street = "300 SW 10th Ave", City = "Topeka", State = "KS", Lat = 39.0489f, Lon = -95.6780f },
                new GeoAddress { Street = "700 Capital Ave", City = "Frankfort", State = "KY", Lat = 38.1973f, Lon = -84.8631f },
                new GeoAddress { Street = "900 N 3rd St", City = "Baton Rouge", State = "LA", Lat = 30.4571f, Lon = -91.1874f },
                new GeoAddress { Street = "210 State St", City = "Augusta", State = "ME", Lat = 44.3072f, Lon = -69.7817f },
                new GeoAddress { Street = "100 State Circle", City = "Annapolis", State = "MD", Lat = 38.9788f, Lon = -76.4910f },
                new GeoAddress { Street = "24 Beacon St", City = "Boston", State = "MA", Lat = 42.3581f, Lon = -71.0636f },
                new GeoAddress { Street = "100 N Capitol Ave", City = "Lansing", State = "MI", Lat = 42.7336f, Lon = -84.5554f },
                new GeoAddress { Street = "75 Rev Dr Martin Luther King Jr Blvd", City = "St. Paul", State = "MN", Lat = 44.9537f, Lon = -93.1022f },
                new GeoAddress { Street = "400 High St", City = "Jackson", State = "MS", Lat = 32.2990f, Lon = -90.1800f },
                new GeoAddress { Street = "201 W Capitol Ave", City = "Jefferson City", State = "MO", Lat = 38.5767f, Lon = -92.1736f },
                new GeoAddress { Street = "1301 E 6th Ave", City = "Helena", State = "MT", Lat = 46.5857f, Lon = -112.0184f },
                new GeoAddress { Street = "1445 K St", City = "Lincoln", State = "NE", Lat = 40.8081f, Lon = -96.7026f },
                new GeoAddress { Street = "101 N Carson St", City = "Carson City", State = "NV", Lat = 39.1639f, Lon = -119.7661f },
                new GeoAddress { Street = "107 N Main St", City = "Concord", State = "NH", Lat = 43.2072f, Lon = -71.5376f },
                new GeoAddress { Street = "125 W State St", City = "Trenton", State = "NJ", Lat = 40.2206f, Lon = -74.7597f },
                new GeoAddress { Street = "490 Old Santa Fe Trail", City = "Santa Fe", State = "NM", Lat = 35.6828f, Lon = -105.9397f },
                new GeoAddress { Street = "Washington Ave", City = "Albany", State = "NY", Lat = 42.6526f, Lon = -73.7562f },
                new GeoAddress { Street = "1 E Edenton St", City = "Raleigh", State = "NC", Lat = 35.7804f, Lon = -78.6391f },
                new GeoAddress { Street = "600 E Boulevard Ave", City = "Bismarck", State = "ND", Lat = 46.8083f, Lon = -100.7837f },
                new GeoAddress { Street = "1 S High St", City = "Columbus", State = "OH", Lat = 39.9612f, Lon = -82.9988f },
                new GeoAddress { Street = "2300 N Lincoln Blvd", City = "Oklahoma City", State = "OK", Lat = 35.4922f, Lon = -97.5033f },
                new GeoAddress { Street = "900 Court St NE", City = "Salem", State = "OR", Lat = 44.9383f, Lon = -123.0309f },
                new GeoAddress { Street = "501 N 3rd St", City = "Harrisburg", State = "PA", Lat = 40.2644f, Lon = -76.8837f },
                new GeoAddress { Street = "82 Smith St", City = "Providence", State = "RI", Lat = 41.8309f, Lon = -71.4149f },
                new GeoAddress { Street = "1100 Gervais St", City = "Columbia", State = "SC", Lat = 34.0007f, Lon = -81.0348f },
                new GeoAddress { Street = "500 E Capitol Ave", City = "Pierre", State = "SD", Lat = 44.3683f, Lon = -100.3510f },
                new GeoAddress { Street = "600 Dr. Martin L. King Jr. Blvd", City = "Nashville", State = "TN", Lat = 36.1659f, Lon = -86.7844f },
                new GeoAddress { Street = "1100 Congress Ave", City = "Austin", State = "TX", Lat = 30.2747f, Lon = -97.7404f },
                new GeoAddress { Street = "350 State St", City = "Salt Lake City", State = "UT", Lat = 40.7774f, Lon = -111.8882f },
                new GeoAddress { Street = "115 State St", City = "Montpelier", State = "VT", Lat = 44.2601f, Lon = -72.5754f },
                new GeoAddress { Street = "1000 Bank St", City = "Richmond", State = "VA", Lat = 37.5388f, Lon = -77.4336f },
                new GeoAddress { Street = "416 Sid Snyder Ave SW", City = "Olympia", State = "WA", Lat = 47.0358f, Lon = -122.9048f },
                new GeoAddress { Street = "1900 Kanawha Blvd E", City = "Charleston", State = "WV", Lat = 38.3362f, Lon = -81.6123f },
                new GeoAddress { Street = "2 E Main St", City = "Madison", State = "WI", Lat = 43.0747f, Lon = -89.3842f },
                new GeoAddress { Street = "200 W 24th St", City = "Cheyenne", State = "WY", Lat = 41.1399f, Lon = -104.8202f }
            });

            Address = addressDatabase.ToList();
        }

        /* It would take a miracle for another U.S. State to be added. */
        private void AddRecord(string street, string city, string state, float lat, float lon)
        {
            addressDatabase.Add(new GeoAddress
            {
                Street = street,
                City = city,
                State = state,
                Lat = lat,
                Lon = lon
            });
        }

        internal GeoAddress? ForwardGeocode(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return null;

            string q = query.Trim().ToLower();

            foreach (var item in addressDatabase)
            {
                if (item.FullAddress.ToLower().Contains(q) || item.City.ToLower().Contains(q) || item.Street.ToLower().Contains(q))
                    return item;
            }
            return null;
        }

        internal GeoAddress? ReverseGeocode(float lat, float lon)
        {
            if (addressDatabase.Count == 0)
                return null;

            GeoAddress nearest = addressDatabase[0];
            double minDist = GetDistSq(lat, lon, nearest.Lat, nearest.Lon);

            for (int i = 1; i < addressDatabase.Count; i++)
            {
                double d = GetDistSq(lat, lon, addressDatabase[i].Lat, addressDatabase[i].Lon);
                if (d < minDist)
                {
                    minDist = d; nearest = addressDatabase[i];
                }
            }
            return nearest;
        }

        private double GetDistSq(float lat1, float lon1, float lat2, float lon2)
        {
            double dLat = lat1 - lat2;
            double dLon = lon1 - lon2;
            return dLat * dLat + dLon * dLon;
        }
    }
}
