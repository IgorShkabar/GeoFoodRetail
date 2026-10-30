/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using GeoFoodRetail.GIS.Abstractions;

namespace GeoFoodRetail.GIS.TileProviders
{
    internal class OsmTileProvider : Abstractions.ITileProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, Image> _memoryCache = new Dictionary<string, Image>();
        private readonly string _cacheFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_tileCache");

        public OsmTileProvider()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WinFormsGisApp/1.0");
            Directory.CreateDirectory(_cacheFolder);
        }       

        public async Task<Image> GetTileAsync(int x, int y, int zoom)
        {
            string key = $"{zoom}_{x}_{y}";
            if (_memoryCache.TryGetValue(key, out var cachedImg))
                return cachedImg;

            string filePath = Path.Combine(_cacheFolder, $"{key}.png");
            // Checking the local disk cache 
            if (File.Exists(filePath))
            {
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        var img = new Bitmap(Image.FromStream(ms));
                        _memoryCache[key] = img;
                        return img;
                    }
                }
                catch
                {
                    // Cache read error — proceeding to download
                }
            }
            // If the file is missing or damaged, download it from the web.
            try
            {
                string url = $"https://tile.openstreetmap.org/{zoom}/{x}/{y}.png";
                byte[] bytes = await _httpClient.GetByteArrayAsync(url);

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    await fs.WriteAsync(bytes, 0, bytes.Length);
                }
                using (var ms = new MemoryStream(bytes))
                {
                    var img = new Bitmap(Image.FromStream(ms));
                    _memoryCache[key] = img;
                    return img;
                }
            }
            catch
            {
                return null; // Fallback placeholder for when there is no network connection
            }
        }
        Task<Image> ITileProvider.GetTileAsync(int x, int y, int zoom)
        {
            throw new NotImplementedException();
        }
    }
}



