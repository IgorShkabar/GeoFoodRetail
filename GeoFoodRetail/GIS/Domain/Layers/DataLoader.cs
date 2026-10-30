/**
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * The data loader class.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GeoFoodRetail.GIS.Domain.Layers
{
    public class DataLoader
    {
        public static List<T> LoadParallel<T>(IEnumerable<T> rawData)
        {
            var sourceList = rawData is List<T> list ? list : rawData.ToList();

            return sourceList.AsParallel()
                             .WithDegreeOfParallelism(Environment.ProcessorCount)
                             .ToList();
        }
    }
}

