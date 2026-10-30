/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using static System.IO.Directory;
using static System.IO.Path;
using static System.Environment;

namespace GeoFoodRetail
{
    public class Files 
    {
        // construct, distruct
        public Files(string t, string b)
        {
            txtfile = t;
            backup = b; 
        }
        ~Files()
        {
            string type = GetType().Name;
        }

        // Properties
        public static string Txtfile => txtfile;
        public static string Backup => backup;

        // Methods
        public static string WorkwithFiles()
        {
            // get info about 
            // Use Path.Combine instead of concatenating strings with "\\"
            string file_path = Path.Combine(Application.StartupPath, "Asort.txt");

            // 2. FileInfo — the local var.
            // there is no need to store it as a class field
            var file_info = new FileInfo(file_path);

            string usrFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] cust_folder = new string[] { usrFolder };
            
            string dir = Combine(cust_folder);
            // define paths to files
            CreateDirectory(dir);

            txtfile = Combine(dir, "Asort.txt");
            backup = Combine(dir, "Asort.back");

            // check if the file exists
            switch(File.Exists(txtfile))
            {
                case true:
                    File.Copy(sourceFileName: txtfile, destFileName: backup, overwrite: false);
                    MessageBox.Show("This file already exists.");
                    break;
                case false:
                    StreamWriter sw = File.CreateText(txtfile);
                    sw.WriteLine("60.00" +
                        "29.00" +
                        "36.00" +
                        "100.00" +
                        "56.00" +
                        "36.00" +
                        "11.00" +
                        "13.00" +
                        "13.00" +
                        "13.00" +
                        "13.00" +
                        "17.00" +
                        "17.00" +
                        "8.00" +
                        "9.00" +
                        "32.00" +
                        "29.00" +
                        "9.00" +
                        "13.00");
                    sw.Close();
                    break;
            }
            // read the csv-file
            StreamReader sr = File.OpenText(txtfile);
            sr.ReadToEnd();
            sr.Close();                     
            return Convert.ToString(sr);
        }
        
        // data: class fields
        static string txtfile;
        static string backup;        
    }
}
