using Anywhere.ArcGIS;
using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowbarWebsite.Helpers
{
    internal static class CASHelpers
    {
        internal static async Task<QueryResponse<Point>> getCrashData(string location)
        {
            string url =
                @"https://services.arcgis.com/CXBb7LAjgIIdcsPt/arcgis/rest/services/CAS_Data_Public/FeatureServer/0/query?where=crashLocation1%20%3D%20'NAME1'%20OR%20crashLocation2%20%3D%20'NAME1'&outFields=&outSR=4326&f=json";


            var gateway = new PortalGateway("https://services.arcgis.com/CXBb7LAjgIIdcsPt/arcgis/");
            string qw = "crashLocation1 = '" + location + "'";
            var query = new Query("CAS_Data_Public/FeatureServer/0".AsEndpoint())
            {
                Where = qw
            };
            query.OutputSpatialReference = SpatialReference.WGS84;
            QueryResponse<Point> result = await gateway.Query<Point>(query);

            return result;
        }

        public static List<Point> GetPoints(this QueryResponse<Point> response)
        {
            List<Point> points = new List<Point>();
            foreach (var feature in response.Features)
            {
                points.Add(feature.Geometry);
            }
            return points;
        }
    }
}
