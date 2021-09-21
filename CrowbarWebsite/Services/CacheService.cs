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
        const int UPDATEINTERVAL = 3600000;
        const double CASMATCHBOUNDS = 0.001; //In Meters, ~100m

        static ConcurrentDictionary<string, List<Point>> _cache = new ConcurrentDictionary<string, List<Point>>();
        static DateTime _lastUpdate = DateTime.Now;
        static int _expectedCount = 0;
        static bool _isInitial = true;
        static bool _backgroundRunning = false;



        static event EventHandler _onContentUpdated;

        public static bool IsCacheReady => _cache.Count != 0 && !_isInitial;
        public static DateTime LastUpdated => _lastUpdate;
        public static int RecommendedTTL => UPDATEINTERVAL;
        public static int LoadPercentage => _expectedCount == 0 ? 0 : (int)Math.Min(100, Math.Max(0, 100 * ((float)_cache.Count) / _expectedCount));

        public static double CASRadiusInMeters => ConvertToMeters(CASMATCHBOUNDS);

        private static double ConvertToMeters(double cASMATCHBOUNDS)
        {
            return (double)(cASMATCHBOUNDS * 110000);
        }

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
        internal static void Update()
        {
            DownloadAsync();
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

                    //Count Cameras
                    var cams = new List<StaticCamera>();

                    //For Each Camera
                    using (var xmlr = XMLHelpers.CreateFromString(xmlstr))
                    {
                        StaticCamera camera = null;
                        do
                        {
                            camera = StaticCamera.FromXML(xmlr);
                            if (camera != null)
                            {
                                cams.Add(camera);
                            }

                        } while (camera != null);
                    }
                    _expectedCount = cams.Count;
                    foreach (var cam in cams)
                    {
                        try
                        {
                            var crashData = await CASHelpers.getCrashData(cam.Street.ToUpper());
                            var points = crashData.GetPoints();//.Where(x => x.IsInBounds(cam.StartPos, CASMATCHBOUNDS)).ToList();
                            _cache.AddOrUpdate($"{cam.Street}.{cam.Area}", points, (k, v) => points);
                        }
                        catch
                        {
                            _expectedCount--;
                        }
                    }
                    cams = null;
                    //Tell all waiting threads to resume
                    _backgroundRunning = false;
                    _lastUpdate = DateTime.Now;
                    _isInitial = false;
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
