/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Record in the log. 
 * A minimal concept in C# for logging errors to a text file using a static logger class.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.Logging
{
    internal class RecordLog
    {        
        private static readonly string logFilePath = 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "error.log");

               
        public static void LogError(Exception ex)
        {
            try
            {                
                string dir = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                
                string logMessage = 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {ex.GetType().Name}: {ex.Message}\nStackTrace:\n{ex.StackTrace}\n{new string('-', 40)}\n";
                
                File.AppendAllText(logFilePath, logMessage);
            }
            catch (Exception writeEx)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to write log: {writeEx.Message}",
                    "Failed log entry");
            }
        }

        // Example of writing log
        static void MyErrorMaking()
        {
            try
            {                
                int zero = 0;
                int result = 10 / zero;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"ERROR: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            System.Windows.Forms.MessageBox.Show("The program has finished running successfully.");
        }
    }
}

