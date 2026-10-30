/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

/* Проверяется значение записи .NET Framework выпуска
 в реестре, чтобы найти установленные версии платформ
 .NET Framework 4.5 - 4.8.1   */

namespace GeoFoodRetail
{
    public partial class CheckVersion
    {
        const string SubKey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
        
        public static string OsProp { get; set; }
        public static int NmbProcessorsProp { get; set; }
        public static string DotNetcoreVrsProp { get; set; }
        public static string NETFrameworkProp { get; set; }
        
        public static object ShowEnvironmentDetails()
        {            
            foreach (string drive in Environment.GetLogicalDrives())
            {
                System.Console.WriteLine("Drive: {0}", drive); // Logical devices
            }
            System.Console.WriteLine("OS: {0}", Environment.OSVersion);
            OsProp = System.Environment.OSVersion.ToString();

            System.Console.WriteLine("Nmb of processors: {0}", Environment.ProcessorCount);
            NmbProcessorsProp = Convert.ToInt16(System.Environment.ProcessorCount);

            System.Console.WriteLine(".NET Core version: {0}", Environment.Version);
            DotNetcoreVrsProp = System.Environment.Version.ToString();
               

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(SubKey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    NETFrameworkProp = CheckVersionDotNet((int)ndpKey.GetValue("Release"));
                    return $".NET Framework Version: {CheckVersionDotNet((int)ndpKey.GetValue("Release"))}";                     
                }
                else
                {
                    return  $".NET Framework Version 4.5 or later is not detected.";
                }
            }      
        }

        // Checking the version using >= enables forward compatibility.

        public static string CheckVersionDotNet(int releaseKey)
        {
            if (releaseKey >= 533320)
                return "4.8.1 or later";
            if (releaseKey >= 528040)
                return "4.8";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            ShowEnvironmentDetails();

            // This code should never be executed. 
            // A non-zero release key should indicate that version 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }

        // Check if the WebView2 Runtime installed on to Windows OS
        internal static bool IsWeb2ViewRuntimeInstalledOnMyWindows()
        {
            try
            {
                // try to get a version of the installed WebView2 Runtime
                string vrs = CoreWebView2Environment.GetAvailableBrowserVersionString();
                MessageBox.Show($"WebView2 Runtime installed. Version: {vrs}");
                return true;
            }
            catch (Exception)
            {
                // Exception for all: WebView2 & other issues 
                MessageBox.Show("WebView2 Runtime IS NOT installed or is corrupted.");
                return false;
            }
        }
    }
}
// https://learn.microsoft.com/ru-ru/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed

