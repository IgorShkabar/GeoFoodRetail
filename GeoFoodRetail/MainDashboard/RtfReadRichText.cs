/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
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
    internal partial class RtfReadRichText : Form
    {
        public RtfReadRichText()
        {
            InitializeComponent();            
        }

        public RtfReadRichText(string txtToShow) : this()
        {
            // Assign the text directly to the RichTextBox element within this form
            // Make sure the control's name matches (e.g., richTextBox1)
            richTextBox1.Text = txtToShow;
        }

        public static RichTextBox RichText { get; set; }

        private void RtfReadRichText_Load(object sender, EventArgs e)
        {
            /// RichText = richTextBox1; особо не нужно, хотя можно юзать
        }      
    }
}
