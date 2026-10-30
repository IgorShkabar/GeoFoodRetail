/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS
{
    internal class InitStoreUpdate
    {
        //  GIS data 
        private GIS.StreamingRTree.RTreePage _storePages;
        
        
        private int _hoveredRegionIndex = -1;    
        private int _selectedRegionIndex = -1;     

        // Properties:
        public GIS.StreamingRTree.RTreePage StorePages  
        {
            get { return _storePages;  }
            set { _storePages = value; }
        }        
        public int HoveredRegionIndex
        {
            get { return _hoveredRegionIndex; }
            set { _hoveredRegionIndex = value; }
        }
        public int SelectedRegionIndex
        {
            get { return _selectedRegionIndex; }
            set { _selectedRegionIndex = value; }
        }

        internal unsafe void InitStoreData()
        {
            StorePages = new GIS.StreamingRTree.RTreePage();

            _storePages = new GIS.StreamingRTree.RTreePage();
            fixed (float* ptr = _storePages.BoxesData)
            {
                // SoA format for 4 stores (AABB zones)
                // min_X
                ptr[0] = 20; ptr[1] = 200; ptr[2] = 50; ptr[3] = 220;
                // min_Y
                ptr[4] = 30; ptr[5] = 40; ptr[6] = 250; ptr[7] = 260;
                // max_X
                ptr[8] = 150; ptr[9] = 350; ptr[10] = 180; ptr[11] = 360;
                // max_Y
                ptr[12] = 160; ptr[13] = 180; ptr[14] = 380; ptr[15] = 390;
            }
        }        
    }
}
