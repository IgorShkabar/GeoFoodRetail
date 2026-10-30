/** 
 Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 ClusterNode is a class that helps group nearby points into clusters.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GeoFoodRetail.GIS.Math3D
{
    internal class ClusterService
    {
        public List<ClusterNode> ClusterPoints(IEnumerable<PointF> points, double gridStep)
        {
            var clusters = new Dictionary<Point, ClusterNode>();

            foreach (var pt in points)
            {
                int gridX = (int)(pt.X / gridStep);
                int gridY = (int)(pt.Y / gridStep);
                var cellKey = new Point(gridX, gridY);

                if (!clusters.TryGetValue(cellKey, out var cluster))
                {
                    cluster = new ClusterNode { Center = pt, Count = 0 };
                    clusters[cellKey] = cluster;
                }
                cluster.Count++;
            }
            return new List<ClusterNode>(clusters.Values);
        }
    }
}
