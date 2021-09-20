using Anywhere.ArcGIS.Common;
using CrowbarWebsite.Models;
using CrowbarWebsite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowbarWebsite.Helpers
{
    public class DisposableMapGenHelper : IDisposable
    {
        private List<Point> _camPts;
        public List<Point> CameraPoints => _camPts;

        public DisposableMapGenHelper(StaticCamera sc, double radius = 0.001)
        {
            _camPts = CacheService.GetPointsForCamera(sc.GetInternalName()).WhereInBounds(sc.StartPos, radius);
        }
        public void Dispose()
        {
            
        }

        
    }
}
