/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFoodRetail
{
    internal class Money
    {
        // Class members
        readonly decimal _clientMoney;
        readonly decimal _pay;

        public Money(decimal cm, decimal pay)
        {
            _clientMoney = cm;
            _pay = pay;
            Banknote.AddRange(new decimal[] { 1, 2, 5, 10, 20, 50, 100 });
        }
        ~Money()
        {
            string type = GetType().Name;
        }

        public static ArrayList Banknote { get; set; }


        // Methods
        decimal GetClientMoney()
        {
            return _clientMoney;
        }
        decimal GetSum()
        {
            return _pay;
        }
        decimal GetChange()
        {
            return (_clientMoney - _pay);
        }

        /* Clean Garbage */

        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        protected virtual void Dispose(bool disposing)
        {            
            Banknote = null;            
        }       
    }
}
