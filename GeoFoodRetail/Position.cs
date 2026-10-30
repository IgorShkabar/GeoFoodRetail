/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GeoFoodRetail
{
    public class Position
    {
        // to position MessageBoxes
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(IntPtr nme, string tit);

        [DllImport("user32.dll")]
        static extern void MoveWindow(IntPtr hwnd, int X, int Y, int w, int h, bool paint);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect
            (IntPtr hwnd, out Rectangle r);

        public static void FindAndMoveMsgBox(int x, int y, bool paint, string tit)
        {
            Thread t = new Thread(() =>
            {
                IntPtr mBox = IntPtr.Zero;
                while ((mBox = FindWindow(IntPtr.Zero, tit)) == IntPtr.Zero) ;

                Rectangle r = new Rectangle();
                GetWindowRect(mBox, out r);
                MoveWindow(mBox, x, y, r.Width - r.X, r.Height - r.Y, paint);
            });
            t.Start();

            t = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        ~Position()
        {
            string type = GetType().Name;
        }
    }
}
