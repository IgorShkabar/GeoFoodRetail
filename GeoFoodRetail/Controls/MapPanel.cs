/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * MapPanel — is a visual control component (a custom component).
 * Simply put, MapPanel provides protection against flickering.
 * I created a new folder called Controls in my project GeoFoodRetail.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.AddManyObjects
{
    public class MapPanel : Panel
    {
        /// <summary>
        /// Custom panel with double buffering
        /// for smooth rendering of 100k+ points without flickering
        /// </summary>
         
        public MapPanel() 
        : base()
        {
            // Enable double buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint |
                        ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
        /// This control is used in place of the standard `Panel` component 
        /// on which I draw the map.
    }
    /// <summary>
    /// How to use it in the Designer Form:
    /// After creating this class, rebuild the project (Build -> Rebuild Solution).
    /// A new control named MapPanel will automatically appear in the Visual Studio Toolbox.
    /// You can drag and drop it from the Toolbox directly onto a form (such as StoresLocations or any other), 
    /// replacing the standard Panel control that we otherwise use to draw the map.
    /// </summary>
}