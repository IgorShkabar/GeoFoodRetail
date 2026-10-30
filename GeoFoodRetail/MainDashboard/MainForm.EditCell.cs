/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Change text in the cell of ListView. Editing cells.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail
{
    internal partial class MainForm
    {
        // In-place editing — changing the cell text directly in the table by 2-ble clicking
                
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listView1.LabelEdit = true;
            
            ListViewItem item = listView1.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                // Enable built-in text box 
                item.BeginEdit();
            }
        } 
    }
}
