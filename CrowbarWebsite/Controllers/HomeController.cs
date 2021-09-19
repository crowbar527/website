using CrowbarWebsite.Helpers;
using CrowbarWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.Util;
using Anywhere.ArcGIS;
using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.Operation;
using Newtonsoft.Json;
using CrowbarWebsite.Services;

namespace CrowbarWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static string mainAccountAccess = "AKIAQHSUYTC4ZUPJYWRK";
        private static string mainAccountSecret = "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";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult RedirAgent()
        {
            return PartialView("RedirAgent");
        }
        public async Task<IActionResult> Index()
        {
            //If Cache is not ready
            if (!CacheService.IsCacheReady)
            {
                //Return Splash Screen
                CacheService.Update();
                ViewData["IsSplash"] = true;
                return View("Splash");
            }
            else
            {
                //Download Static Camera XML
                string xmlstr = await AWSHelpers.downloadXML();

                //Generate Camera Objects from XML
                List<StaticCamera> cameras = new List<StaticCamera>();
                using (var xmlr = XMLHelpers.CreateFromString(xmlstr))
                {
                    StaticCamera camera = null;
                    do
                    {
                        camera = StaticCamera.FromXML(xmlr);
                        if (camera != null)
                            cameras.Add(camera);

                    } while (camera != null);
                }
                return View("Index", cameras);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
