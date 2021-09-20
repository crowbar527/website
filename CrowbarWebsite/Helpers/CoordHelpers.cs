using Anywhere.ArcGIS.Common;
using CrowbarWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowbarWebsite.Helpers
{
    public static class CoordHelpers
    {
        public static bool IsInBounds(this Point p, Coord camera, double radius)
        {
            return p.X >= camera.Longtitude - radius && p.X <= camera.Longtitude + radius && p.Y >= camera.Latitude - radius && p.Y <= camera.Latitude + radius;
        }

        public static List<Point> WhereInBounds(this List<Point> points, Coord camera, double radius)
        {
            return points.Where(x => x.IsInBounds(camera, radius)).ToList();
        }
    }
}
