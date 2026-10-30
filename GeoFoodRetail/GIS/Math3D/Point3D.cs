/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * 
 * This C# code defines a structure for handling three-dimensional coordinates 
 * within a specialized system.
 */

/** 1. Scope of Application (Context)
 GeoFoodRetail.GIS.Math3D (Namespace): Indicates that the code is part of a 
 Geographic Information System (GIS). GIS systems handle maps, topography, and spatial data. 
 The "Math3D" component signifies that this is a module for performing 3D mathematical or 
 geometric calculations. 
2. Purpose of the Point3D structure (3D point): This is a basic data model for storing 
   an object's coordinates in three-dimensional space. X, Y, Z (Properties): 
   Double-precision floating-point numbers (double). In a GIS context, they can represent: 
   X and Y: Latitude and longitude (or planar map projection coordinates in meters). 
   Z: Elevation above sea level, depth, or the number of stories of an object.     
    
3. internal: Limits the visibility of the structure. It is accessible only within the assembly 
   in which it is compiled. 
   struct (Value type): Choosing a struct instead of a class optimizes memory usage. 
   Points are often created by the millions (for example, when importing a laser-scanned ground 
   point cloud). Structs help avoid placing a load on the Garbage Collector. 

   public Point3D(...): A constructor that allows you to quickly initialize a point in 
   a single line of code by passing all three coordinates at once.

Where can this be used? 
Calculating distances between objects on hilly terrain.
Generating 3D models of buildings or terrain (digital elevation models).
Logistics and navigation that account for elevation.
Designing utility networks (e.g., cabling) where burial depth (Z-coordinate) is critical. */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoFoodRetail.GIS.Math3D
{
    internal struct Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }        
    }
}