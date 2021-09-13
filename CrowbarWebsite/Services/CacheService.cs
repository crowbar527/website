using Anywhere.ArcGIS.Common;
using CrowbarWebsite.Helpers;
using CrowbarWebsite.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrowbarWebsite.Services
{
    public static class CacheService
    {
        const int UPDATEINTERVAL = 300000;

        static ConcurrentDictionary<string, List<Point>> _cache = new ConcurrentDictionary<string, List<Point>>();
        static DateTime _lastUpdate = DateTime.Now;
        static bool _backgroundRunning = false;

        static event EventHandler _onContentUpdated;

        public static List<Point> GetPointsForCamera(string camera)
        {
            if (!_cache.ContainsKey(camera))
            {

                //Prepare to Make User Wait
                var are = new AutoResetEvent(false);
                _onContentUpdated += (s, e) =>
                {
                    are.Set();
                };
                //Try Download
                DownloadAsync();
                //Wait for Event Handler to release
                are.WaitOne();
                if (!_cache.ContainsKey(camera))
                {
                    Debug.WriteLine($"Camera {camera} Not Found in cache");
                    throw new Exception("Camera Not Defined");
                }
                else
                    return _cache[camera];
            }
            else
            {
                //Return Data
                //Update if data is stale
                if ((DateTime.Now - _lastUpdate).TotalMilliseconds >= UPDATEINTERVAL)
                    DownloadAsync();
                return _cache[camera];
            }

        }

        private static void DownloadAsync()
        {
            //If backgroundRunning flag is set, then process already running
            if (!_backgroundRunning)
            {
                _backgroundRunning = true;
                Task.Run(async () =>
                {
                    //Download Cameras
                    string xmlstr = await AWSHelpers.downloadXML();

                    //For Each Camera
                    using (var xmlr = XMLHelpers.CreateFromString(xmlstr))
                    {
                        StaticCamera camera = null;
                        do
                        {
                            camera = StaticCamera.FromXML(xmlr);
                            if (camera != null)
                            {
                                var crashData = await CASHelpers.getCrashData(camera.Street.ToUpper());
                                var points = crashData.GetPoints();
                                _cache.AddOrUpdate($"{camera.Street}.{camera.Area}", points, (k, v) => points);
                            }

                        } while (camera != null);
                    }
                    //Tell all waiting threads to resume
                    _backgroundRunning = false;
                    if (_onContentUpdated != null)
                    {
                        foreach (var @delegate in _onContentUpdated.GetInvocationList())
                        {
                            @delegate.DynamicInvoke(null, EventArgs.Empty);
                        }
                    }
                });
            }
        }
    }
}
