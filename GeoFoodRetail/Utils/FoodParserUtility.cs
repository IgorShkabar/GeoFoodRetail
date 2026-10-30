/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * To organize a utility class for parsing.
 * Parses the OpenFoodFacts gzip archive, cleans up the names, and saves a flat text file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace GeoFoodRetail.Utils
{
    public static class FoodParserUtility
    {
        /// <summary>
        /// Parses the OpenFoodFacts gzip archive, cleans up the names, and saves them to a flat text file.
        /// </summary>
        /// <param name="inputGzPath">Path to the source .jsonl.gz file</param>
        /// <param name="outputTxtPath">Path for saving the resulting foods.txt file</param>
        /// <param name="onProgress">Optional callback for outputting progress (rows, unique products)</param>

        public static void ParseAndSaveFoodNames(string outputTxtPath)
        {
            var uniqueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Streaming a large gzip archive
            using (var fileStream = new FileStream(outputTxtPath, FileMode.Open, FileAccess.Read, FileShare.Read))            
            using (var reader = new StreamReader(outputTxtPath, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string rawName = ExtractProductName(line);
                    if (!string.IsNullOrEmpty(rawName))
                    {
                        string cleaned = CleanName(rawName);
                        if (!string.IsNullOrEmpty(cleaned) && !IsDigitsOnly(cleaned) && cleaned.Length > 1)
                        {
                            string formattedName = char.ToUpper(cleaned[0]) + cleaned.Substring(1);
                            uniqueNames.Add(formattedName);
                        }
                    }
                }
            }
            var sortedNames = uniqueNames.OrderBy(n => n).ToList();            
            File.WriteAllLines(outputTxtPath, sortedNames, System.Text.Encoding.UTF8);
        }

        private static string ExtractProductName(string jsonLine)
        {
            var match = Regex.Match(jsonLine, @"""product_name(?:_en)?""\s*:\s*""([^""]+)""");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string CleanName(string name)
        {
            name = name.Trim().ToLower();
            name = Regex.Replace(name, @"[^\w\s\-]", "");
            name = Regex.Replace(name, @"\s+", " ");
            name = Regex.Replace(name, @"\-+", "-");
            return name.Trim();
        }

        private static bool IsDigitsOnly(string str)
        {
            return str.All(char.IsDigit);
        }
    }
}
