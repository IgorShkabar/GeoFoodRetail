/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text.Json; // Use a fast serializer
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace GeoFoodRetail.MainDashboard
{
    internal static class TaxService
    {
        private const string APIUrl = "https://api.yourdomain.com/v1/us-taxes"; // the reference on my online API / JSON
        private static readonly string _cacheFilePath = 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taxes.json");
        
        public static Dictionary<string, decimal> StateTaxes { get; private set; } = 
            new Dictionary<string, decimal>();

        public static List<string> stateName = StateTaxes.Keys.ToList();

        // Override ToString so that the ComboBox automatically displays the country name in a nice format
        // public override string ToString() => Country;

        public static async Task InitializeAsync()
        {
            // Attempting to download the latest data from the internet
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
                {
                    string jsonFromNet = await client.GetStringAsync(APIUrl);
                    
                    StateTaxes = 
                         JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonFromNet);
                    // Download successful — updating the local file cache
                    File.WriteAllText(_cacheFilePath, jsonFromNet);
                    return;
                }
            }
            catch
            {
                // Network failures, lack of internet, or timeouts — switching to the local cache
            }
            TryReadLocalFile();
            // Hardcoded fallback (used if neither an internet connection nor a file is available)
            GetHardcodedFallback();
        }

        private static void TryReadLocalFile()
        {
            // If there is no network, try reading from the local file cache
            if (File.Exists(_cacheFilePath))
            {
                try
                {
                    string jsonFromCache = File.ReadAllText(_cacheFilePath);
                    var serializer = new JavaScriptSerializer();

                    // Read as a dictionary of objects
                    var rawDictionary = serializer.Deserialize<Dictionary<string, object>>(jsonFromCache);
                                        
                    StateTaxes = rawDictionary.ToDictionary(
                        item => item.Key,
                        item => Convert.ToDecimal(item.Value));
                                           
                    StateTaxes = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonFromCache);
                    return;         
                }
                catch
                {
                    // Error reading cache file
                    // Safe recovery: deleting the corrupted file
                    TryDeleteCorruptedCache();
                }
            }
            else
            {
                MessageBox.Show("The tax data file is missing or corrupted.", "File does not Exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Find(AppDomain.CurrentDomain.BaseDirectory, "taxes.json");
            }
        }

        private static void TryDeleteCorruptedCache()
        {
            try
            {
                if (File.Exists(_cacheFilePath))
                {
                    File.Delete(_cacheFilePath);
                    Console.WriteLine("The corrupted cache file has been successfully deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete the corrupted cache: {ex.Message}");
            }
        }

        // A simple file-searching method in C# using Directory.EnumerateFiles 
        // to locate a file by name within a root directory and all its subdirectories
        private static string Find(string rootPath, string fileName)
        {
            try
            {
                return Directory.EnumerateFiles(rootPath, fileName, SearchOption.AllDirectories)
                    .FirstOrDefault();
            }
            catch(UnauthorizedAccessException)
            {
                return null;
            }
        }

        private static void GetHardcodedFallback()
        {
            StateTaxes = new Dictionary<string, decimal>
            {
                { "Alabama", 0.10m },
                { "Alaska", 0.0785m },
                { "California", 0.1075m },
                { "Colorado", 0.112m },
                { "Connecticut", 0.0635m },
                { "Delaware", 0.0000m },
                { "Florida", 0.06m },
                { "Georgia", 0.075m },
                { "Hawaii", 0.09m },
                { "Idaho", 0.045m },
                { "Illinois", 0.09m },
                { "Indiana", 0.115m },
                { "Iowa", 0.07m },
                { "Kansas", 0.07m },
                { "Kentucky", 0.085m },
                { "Louisiana", 0.06m },
                { "Maine", 0.1295m },
                { "Maryland", 0.055m },
                { "Massachusetts", 0.06m },
                { "Michigan", 0.0625m },
                { "Minnesota", 0.06m },
                { "Mississippi", 0.07875m },
                { "Missouri", 0.09m },
                { "Montana", 0.101m },
                { "Nebraska", 0.00m },
                { "Nevada", 0.00m },
                { "New Hampshire", 0.075m },
                { "New Jersey", 0.08375m },
                { "New Mexico", 0.0m },
                { "New York", 0.06625m },
                { "North Carolina", 0.12625m },
                { "North Dakota", 0.08875m },
                { "Ohio", 0.075m },
                { "Oklahoma", 0.08m },
                { "Oregon", 0.08m },
                { "Pennsylvania", 0.115m },
                { "Rhode Island", 0.00m },
                { "South Carolina", 0.08m },
                { "South Dakota", 0.07m },
                { "Tennessee", 0.09m },
                { "Texas", 0.062m },
                { "Utah", 0.0975m },
                { "Vermont", 0.0825m },
                { "Virginia", 0.0905m },
                { "Washington", 0.07m },
                { "West Virginia", 0.07m },
                { "Wisconsin", 0.0106m },
                { "Wyoming", 0.07m },
                { "Afghanistan", 0.00m },
                { "Albania", 0.20m },
                { "Algeria", 0.09m },
                { "American Samoa", 0.00m },
                { "Andorra", 0.01m },
                { "Angola", 0.14m },
                { "Anguilla", 0.00m },
                { "Antarctica", 0.00m },
                { "Antigua and Barbuda", 0.17m },
                { "Argentina", 0.21m },
                { "Armenia", 0.20m },
                { "Aruba", 0.07m },
                { "Australia", 0.10m },
                { "Austria", 0.049m },
                { "Azerbaijan", 0.18m },
                { "Bahamas", 0.00m },
                { "Bahrain", 0.00m },
                { "Bangladesh", 0.15m },
                { "Barbados", 0.175m },
                { "Belarus", 0.10m },
                { "Belgium", 0.06m },
                { "Belize", 0.00m },
                { "Benin", 0.18m },
                { "Bermuda", 0.00m },
                { "Bolivia", 0.13m },
                { "Bonaire, Sint Eustatius and Saba", 0.00m },
                { "Bosnia & Herzegovina", 0.17m },
                { "Botswana", 0.14m },
                { "Bouvet Island", 0.00m },
                { "Brazil", 0.25m },
                { "British Virgin Islands", 0.00m },
                { "Brunei", 0.00m },
                { "Bulgaria", 0.20m },
                { "Burkina Faso", 0.00m },
                { "Burundi",  0.18m },
                { "Cabo Verde", 0.15m },
                { "Cambodia", 0.10m },
                { "Cameroon", 0.1925m },
                { "Canada", 0.00m },
                { "Cayman Islands", 0.00m },
                { "Central African Republic", 0.19m },
                { "Chad", 0.09m },
                { "Chile", 0.19m },
                { "China", 0.13m },
                { "Christmas Island", 0.00m },
                { "Cocos (Keeling) Islands", 0.00m }, 
                { "Colombia", 0.19m },
                { "Comoros", 0.00m },
                { "Congo Democratic Republic", 0.00m },
                {  "Congo", 0.00m },
    { "Cook Islands", 0.00m },
    { "Costa Rica", 0.00m },
    { "Croatia", 0.25m },
    { "Cuba", 0.10m },
    { "Cyprus", 0.09m },
    { "Czechia", 0.12m },
    { "Denmark", 0.25m },
    { "Djibouti", 0.00m },
    { "Dominica", 0.15m },
    { "Dominican Republic", 0.18m },
    { "Ecuador", 0.00m },
    { "Egypt", 0.14m },
    { "El Salvador", 0.00m },
    { "Equatorial Guinea", 0.00m },
    { "Estonia", 0.24m },
    { "Ethiopia", 0.00m },
    { "Falkland Islands", 0.00m },
    { "Faroe Islands", 0.25m },
    { "Fiji", 0.125m },
    { "Finland", 0.14m },
    { "France", 0.055m },
    { "French Guiana", 0.00m },
    { "French Polynesia", 0.05m },
    { "Gabon", 0.18m },
    { "Gambia", 0.15m },
    { "Georgia no USA", 0.18m },
    { "Germany", 0.19m },
    { "Ghana", 0.20m },
    { "Gibraltar", 0.00m },
    { "Greece", 0.13m },
    { "Greenland", 0.00m },
    { "Grenada", 0.00m },
    { "Guatemala", 0.12m },
    { "Guernsey, Channel Islands", 0.00m },
    { "Guinea-Bissau", 0.10m },
    { "Guyana", 0.14m },
    { "Haiti", 0.10m },
    { "Heard Island and McDonald Islands", 0.00m },
    { "Honduras", 0.00m },
    { "Hong Kong", 0.00m },
    { "Hungary", 0.27m },
    { "Iceland", 0.11m },
    { "India", 0.18m },
    { "Indonesia", 0.00m },
    { "Iran", 0.00m },
    { "Iraq", 0.00m },
    { "Ireland", 0.00m },
    { "Isle of Man", 0.00m },
    { "Israel", 0.00m },
    { "Italy", 0.00m },
    { "Jamaica", 0.00m },
    { "Japan", 0.00m },
    { "Jordan", 0.00m },
    { "Kazakhstan", 0.00m },
    { "Kenya", 0.00m },
    { "Korea Democratic People's Republic", 0.00m },
    { "Kuwait", 0.00m }, 
    { "Kyrgyzstan", 0.00m },
    { "Laos", 0.00m },
    { "Latvia", 0.00m },
    { "Lebanon", 0.00m },
    { "Lesoto", 0.00m },
    { "Liberia", 0.00m },
    { "Libya", 0.00m },
    { "Liechtenstein", 0.00m },
    { "Lithuania", 0.00m },
    { "Luxembourg", 0.00m },
    { "Macao", 0.00m },
    { "Madagascar", 0.00m },
    { "Malawi", 0.00m },
    { "Malaysia", 0.00m },
    { "Maldives", 0.00m },
    { "Mali", 0.00m },
    { "Malta", 0.00m },
    { "Martinique", 0.00m },
    { "Mauritania", 0.00m },
    { "Mauritius", 0.00m },
    { "Mexico", 0.00m },
    { "Moldova", 0.00m },
    { "Monaco", 0.00m },
    { "Mongolia", 0.00m },
    { "Montenegro", 0.00m },
    { "Morocco", 0.00m },
    { "Mozambique", 0.00m },
    { "Namibia", 0.00m },
    { "Nepal", 0.00m },
    { "Netherlands", 0.09m },
    { "New Caledonia", 0.11m },
    { "New Zealand", 0.15m },
    { "Nicaragua", 0.00m },
    { "Nigeria", 0.00m },
    { "North Macedonia", 0.10m },
    { "Norway", 0.15m },
    { "Oman", 0.00m },
    { "Pakistan", 0.18m },
    { "Palestine", 0.00m },
    { "Panama", 0.00m },
    { "Papua New Guinea", 0.00m },
    { "Paraguay", 0.10m },
    { "Peru", 0.18m },
    { "Philippines", 0.12m },
    { "Poland", 0.23m },
    { "Portugal", 0.13m },
    { "Puerto Rico", 0.00m },
    { "Qatar", 0.00m }, 
    { "Romania", 0.11m },
        { "Russia", 0.22m },
        { "Rwanda", 0.00m },
        { "Saint Lucia", 0.00m },
        { "Samoa", 0.00m },
        { "San Marino", 0.06m },
        { "Saudi Arabia", 0.15m },
        { "Senegal", 0.18m },
        { "Serbia", 0.20m },
        { "Singapore", 0.09m },
            { "Slovakia", 0.19m },
            { "Slovenia", 0.095m },
        { "South Africa", 0.00m },
        { "Spain", 0.10m },
        { "Sri Lanka", 0.18m },
        { "Sweden", 0.06m },
        { "Switzerland", 0.026m },
        { "Thailand", 0.07m },
        { "Taiwan", 0.05m },
        { "Tajikistan", 0.15m },
        { "Tanzania", 0.18m },
        { "The Great Britain", 0.20m },
        { "the United Arab Emirates", 0.05m },
        { "Trinidad and Tobago", 0.125m },
        { "Tunisia", 0.07m },
        { "Turkmenistan", 0.00m },
        { "Turkye", 0.00m },
        { "Uganda", 0.00m },
        { "Ukraine", 0.20m },
        { "Uruguay", 0.22m },
        { "Uzbekistan", 0.12m },
        { "Vanuatu", 0.15m },
        { "Venezuela", 0.16m },
        { "Vietnam", 0.10m },
        { "Zambia", 0.00m },
        { "Zimbabwe", 0.15m }        
            };
        }
    }
}

