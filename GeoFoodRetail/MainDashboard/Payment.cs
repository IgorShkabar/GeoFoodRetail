/** 
 * Copyright (_c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Console;

namespace GeoFoodRetail
{
    internal class Payment
    {
        public static decimal StorePayment { get; set; }
        public static int Nmb { get; } = 3;
        public static bool Button4wasClicked { get; set; }
        public static decimal Price { get; set; }
        public static decimal Quantity { get; set; }

        public static char Symb { get; } = ' ';

        // constructs & destruct
        public Payment() { }
        public Payment(decimal p, int q)   // construct
        {
            Price = p;
            Quantity = q;
        }
        
        // Methods
        decimal GetPrice()
        {
            return Price;
        }
        decimal GetQuantity()
        {
            return Quantity;
        }
    }
}