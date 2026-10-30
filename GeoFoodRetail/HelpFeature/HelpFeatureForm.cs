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

namespace GeoFoodRetail
{
    public partial class HelpFeatureForm : Form
    {
        public HelpFeatureForm()
        {
            InitializeComponent();

            // Windows Form layout
            Rectangle screen = Screen.FromPoint(Cursor.Position).WorkingArea;
            int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 3;
            int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 3;
            Location = new Point(screen.Left + (screen.Width - w) / 3,
                    screen.Top + (screen.Height - h) / 3);

            Size = new Size(w, h);                
        }

    }
}
