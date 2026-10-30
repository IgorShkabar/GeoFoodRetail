/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * Declaring fields as public in C# structs is acceptable 
 * and does not violate design principles, 
 * provided the struct serves as a simple data container.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Declaring fields as `public` in C# structs is acceptable and does not violate design principles, provided the struct serves as a simple data container (a Data Transfer Object or a Cohesive Value).
/// In my specific case, since the struct is marked as `internal` and is used exclusively within a single assembly, the use of `public` fields is entirely justified.
/// Why this is acceptable in my scenario:
/// Absence of business logic: 
/// Coordinates (Lat, Lon) and address strings are independent, atomic pieces of data. 
/// There are no validation rules (invariants) here that would require protection through encapsulation.
/// </summary>
/// 
namespace GeoFoodRetail.GIS.Domain.Models
{
    internal struct GeoAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public float  Lat { get; set; }
        public float  Lon { get; set; }

        public string FullAddress => $"{Street}, {City}, {State}";
    }
}

