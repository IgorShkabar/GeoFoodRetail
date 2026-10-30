/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.OSparts
{
    internal partial class OsPartsForm : Form
    {
        public OsPartsForm()
        {
            InitializeComponent();
        }

        private void OsPartsForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = CheckVersion.OsProp;
            textBox2.Text = Convert.ToInt16(CheckVersion.NmbProcessorsProp).ToString();
            textBox3.Text = CheckVersion.DotNetcoreVrsProp;
            textBox4.Text = CheckVersion.NETFrameworkProp;

            CheckVersion.IsWeb2ViewRuntimeInstalledOnMyWindows();
        }
    }
}
