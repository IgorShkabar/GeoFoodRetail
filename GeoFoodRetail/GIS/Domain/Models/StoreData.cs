/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * If the map projection or rendering logic in my engine is designed to expect positive values ​​
 * for western longitudes (or if the `LatLonToWebMercator` formula itself incorporates an 
 * inverted X-axis coefficient), then the presence of a minus sign in the source data will 
 * simply mirror the markers to the opposite side of the globe.
 * 
 * Remove the negative signs from the longitudes in the `Store` list — since this results in 
 * the correct positioning on the canvas (both visually and mathematically) for my renderer, 
 * it means the repository’s coordinate system functions exactly this way.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoFoodRetail.GIS.Domain.Models.RetailStoreMarker;

namespace GeoFoodRetail.GIS.Domain.Models
{
    internal static class StoreData
    {
        public static List<RetailStoreMarker> Store { get; set; } = new List<RetailStoreMarker>()
        {
            new RetailStoreMarker("Walmart Inc. (WMT)", "Walmart Supercenters, Discount Stores, Neighborhood Markets, and Sam's Club warehouses", new GeoCoordinate3D(36.3726, -94.2088, 0), StoreType.Retail, Color.Blue),
            new RetailStoreMarker("The Kroger Co. (KR)", "over 2,700 grocery stores and supermarkets", new GeoCoordinate3D(39.1054, -84.5112, 0), StoreType.Grocery, Color.DarkBlue),
            new RetailStoreMarker("Costco Wholesale Corp. (COST)", "Costco Wholesale warehouses", new GeoCoordinate3D(47.5301, 122.0326, 0), StoreType.Supermarket, Color.Red),
            new RetailStoreMarker("Target Corp. (TGT)", "Target chain of discount department stores", new GeoCoordinate3D(44.9785, 93.2716, 0), StoreType.Retail, Color.DarkRed),
            new RetailStoreMarker("Albertsons Companies, Inc. (ACI)", "Albertsons Companies operates grocery stores and pharmacies", new GeoCoordinate3D(43.6166, -116.2008, 0), StoreType.Supermarket, Color.LightBlue),
            new RetailStoreMarker("Sprouts Farmers Market, Inc. (SFM)", "A specialty grocery store chain focused on fresh, natural, and organic foods", new GeoCoordinate3D(33.6115, 111.9778, 0), StoreType.Grocery, Color.Green),
            new RetailStoreMarker("Weis Markets, Inc. (WMK)", "A regional supermarket and grocery store chain", new GeoCoordinate3D(40.8626, 76.7933, 0), StoreType.Grocery, Color.DarkGreen),
            new RetailStoreMarker("Grocery Outlet Holding Corp. (GO)", "United Grocery Outlet.", new GeoCoordinate3D(37.8405, 122.2882, 0), StoreType.Grocery, Color.Orange),
            new RetailStoreMarker("Ingles Markets, Inc. (IMKTA)", "A regional supermarket chain", new GeoCoordinate3D(35.6132, 82.3393, 0), StoreType.Grocery, Color.Red),
            new RetailStoreMarker("Casey's General Stores, Inc. (CASY)", "American convenience store chain", new GeoCoordinate3D(41.7247, 93.6022, 0), StoreType.Retail, Color.Brown),
            new RetailStoreMarker("Village Super Market, Inc. (VLGEA)", "30 supermarket locations across New Jersey, New York, Pennsylvania, and Maryland", new GeoCoordinate3D(40.6926, 74.3160, 0), StoreType.Supermarket, Color.Goldenrod),
            new RetailStoreMarker("The Andersons, Inc. (ANDE)", "A regional retail store chain", new GeoCoordinate3D(41.5645, 83.6538, 0), StoreType.Retail, Color.Gray),

                // Distributors and the supply chain
            new RetailStoreMarker("Sysco Corp. (SYY)", "A global supply chain", new GeoCoordinate3D(29.7752, 95.5463, 0), StoreType.Retail, Color.Navy),
            new RetailStoreMarker("US Foods Holding Corp. (USFD)", "cash-and-carry retail stores", new GeoCoordinate3D(42.0011, 87.8711, 0), StoreType.Retail, Color.DarkGreen),
            new RetailStoreMarker("Performance Food Group Co. (PFGC)", "a major food and foodservice supply chain distributor", new GeoCoordinate3D(37.6441, 77.5750, 0), StoreType.Retail, Color.Purple),
            new RetailStoreMarker("SpartanNash Co. (SPTN)", "more than 200 corporate-run grocery stores", new GeoCoordinate3D(42.8122, 85.7061, 0), StoreType.Grocery, Color.Olive),
            new RetailStoreMarker("United Natural Foods, Inc. (UNFI)", "A massive supply chain and wholesale distribution network ", new GeoCoordinate3D(41.8308, 71.4150, 0), StoreType.Grocery, Color.DarkOliveGreen),
            new RetailStoreMarker("The Chef's Warehouse, Inc. (CHEF)", "B2B (business-to-business) supply chain and distribution network", new GeoCoordinate3D(41.2815, 73.4982, 0), StoreType.Grocery, Color.SaddleBrown),

                // Private supermarkets and chains
            new RetailStoreMarker("Publix Super Markets", "An employee-owned American supermarket chain",  new GeoCoordinate3D(27.9744, 82.0253, 0), StoreType.Supermarket, Color.Green),
            new RetailStoreMarker("H-E-B Grocery Company", "A privately held American supermarket and grocery chain", new GeoCoordinate3D(29.4262, 98.4975, 0), StoreType.Supermarket, Color.Red),
            new RetailStoreMarker("Wegmans Food Markets", "A large-format, family-owned regional supermarket chain", new GeoCoordinate3D(43.1415, -77.6715, 0), StoreType.Supermarket, Color.Maroon),
            new RetailStoreMarker("Hy-Vee, Inc.", "An employee-owned supermarket chain", new GeoCoordinate3D(41.5694, -93.7630, 0), StoreType.Supermarket, Color.Crimson),
            new RetailStoreMarker("Meijer Inc.", "Meijer Inc", new GeoCoordinate3D(43.0239, -85.6705, 0), StoreType.Supermarket, Color.Blue),
            new RetailStoreMarker("Wakefern Food Corp. (ShopRite)", "Several retail chains", new GeoCoordinate3D(40.5234, -74.3013, 0), StoreType.Supermarket, Color.OrangeRed),
            new RetailStoreMarker("Giant Eagle, Inc.", "A regional American supermarket and pharmacy chain", new GeoCoordinate3D(40.4901, -79.8785, 0), StoreType.Supermarket, Color.DarkOrange),
            new RetailStoreMarker("WinCo Foods", "An employee-owned supermarket chain",  new GeoCoordinate3D(43.6015, -116.2422, 0), StoreType.Supermarket, Color.SeaGreen),
            new RetailStoreMarker("Trader Joe's", "German supermarket chain Aldi Nord", new GeoCoordinate3D(34.1411, -118.0012, 0), StoreType.Grocery, Color.DarkRed),
            new RetailStoreMarker("Aldi US", "Aldi Süd (Aldi South)", new GeoCoordinate3D(41.8353, 88.2917, 0), StoreType.Grocery, Color.DarkBlue),
            new RetailStoreMarker("Southeast Grocers", "A flagship grocery", new GeoCoordinate3D(30.1983, -81.6033, 0), StoreType.Supermarket, Color.Teal),
            new RetailStoreMarker("Save Mart Supermarkets", "A regional grocery store operator", new GeoCoordinate3D(37.6692, -121.0003, 0), StoreType.Supermarket, Color.ForestGreen),
            new RetailStoreMarker("Brookshire Grocery Company", "More than 200 stores across Texas, Louisiana, Arkansas, and Oklahoma", new GeoCoordinate3D(32.3276, -95.3056, 0), StoreType.Grocery, Color.DarkCyan),
            new RetailStoreMarker("Raley's Supermarkets", "A Raley's Supermarkets", new GeoCoordinate3D(38.5674, -121.5244, 0), StoreType.Supermarket, Color.Firebrick),
            new RetailStoreMarker("Stater Bros. Markets", "A private regional supermarket chain", new GeoCoordinate3D(34.1353, -117.2656, 0), StoreType.Supermarket, Color.SteelBlue),
            new RetailStoreMarker("Bashas' Supermarkets", "The Raley's Companies", new GeoCoordinate3D(33.2801, -111.8340, 0), StoreType.Supermarket, Color.Chocolate),
            new RetailStoreMarker("Demoulas Market Basket", "A regional supermarket chain", new GeoCoordinate3D(42.6143, -71.2223, 0), StoreType.Supermarket, Color.IndianRed),
            new RetailStoreMarker("Schnuck Markets, Inc.", "American supermarket chain", new GeoCoordinate3D(38.6534, -90.4190, 0), StoreType.Supermarket, Color.Crimson),
            new RetailStoreMarker("Alex Lee, Inc. (Lowes Foods)", "KJ's Market, IGA (via W. Lee Flowers & Company), and Just Save", new GeoCoordinate3D(35.7332, -81.3412, 0), StoreType.Grocery, Color.DarkOliveGreen),
            new RetailStoreMarker("Associated Wholesale Grocers", "Largest retailer-owned grocery cooperative in the United States", new GeoCoordinate3D(39.0632, -94.6980, 0), StoreType.Retail, Color.SlateGray)
        };

        public static string GetString(string prm)
        {
            string variable = prm;
            foreach (var store in Store)
            {
                if (store != null)
                {
                    variable = store.ToString();
                }
            }
            return variable;
        }
    }
}
