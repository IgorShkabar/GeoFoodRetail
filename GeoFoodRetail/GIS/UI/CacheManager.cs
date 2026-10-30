/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GeoFoodRetail.GIS.Domain.Models;
using static GeoFoodRetail.GIS.Domain.Models.RetailStoreMarker;

namespace GeoFoodRetail.GIS.UI
{
    public sealed class CacheManager : IDisposable
    {
        private readonly Dictionary<Color, Pen> _penCache = new Dictionary<Color, Pen>();
        private readonly Dictionary<Color, Brush> _brushCache = new Dictionary<Color, Brush>();
        private readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>();
        private string _anyName;

        public Pen GetPen(Color color, float width = 1f)
        {
            if (!_penCache.TryGetValue(color, out var pen))
            {
                pen = new Pen(color, width);
                _penCache[color] = pen;
            }
            return pen;
        }

        public Brush GetBrush(Color color)
        {
            if (!_brushCache.TryGetValue(color, out var brush))
            {
                brush = new SolidBrush(color);
                _brushCache[color] = brush;
            }
            return brush;
        }

        public Font GetFont(string familyName, float size, FontStyle style = FontStyle.Regular)
        {
            string key = $"{familyName}_{size}_{style}";
            if (!_fontCache.TryGetValue(key, out var font))
            {
                font = new Font(familyName, size, style);
                _fontCache[key] = font;
            }
            return font;
        }

        // Getter & Setter
        public string GetLine() => _anyName;

        public string SetLine(string name, string entry)
        {
            if (!string.IsNullOrEmpty(entry) && name != entry)
            {
                _anyName = entry;
            }
            return _anyName;
        }

        public void Dispose()
        {
            foreach (var pen in _penCache.Values) pen.Dispose();
            foreach (var brush in _brushCache.Values) brush.Dispose();
            foreach (var font in _fontCache.Values) font.Dispose();
            _penCache.Clear();
            _brushCache.Clear();
            _fontCache.Clear();
        }
    }
}
