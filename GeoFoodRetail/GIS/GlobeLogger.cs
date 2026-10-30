/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Place in GeoFoodRetail\GIS (or GeoFoodRetail\GIS\Utilities\), as it is an auxiliary logging and debugging service 
 * for under-the-hood performance monitoring.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS
{
    internal class GlobeLogger
    {        
        private static readonly System.Collections.Concurrent.ConcurrentQueue<string>
            _logs = new System.Collections.Concurrent.ConcurrentQueue<string>();
        
        private static string _lastMessage = string.Empty; 

        private const int MaxLines = 20;

        public static void Log(string message)
        {
            string formattedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            
            if (_lastMessage == message) return;
            _lastMessage = message;

            _logs.Enqueue(formattedMessage);

            while (_logs.Count > MaxLines)
            {
                _logs.TryDequeue(out _);
            }
        }

        public static void ResetLastMessage()
        {
            _lastMessage = string.Empty;
        }

        public static IEnumerable<string> GetLines() => _logs.ToArray();
    }   
}
