using CrowbarWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace CrowbarWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            GetS3Data();
        }

        public IActionResult Index()
        {
            return View();
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

        static async Task GetS3Data()
        {
#if !DEBUG
            TransferUtility fileTransfer = new TransferUtility(new AmazonS3Client(RegionEndpoint.USEast2));
            fileTransfer.Download("/var/www/websiteapp/", "crowbar-staticdata", "static_cameras.xml");
#else
            Debug.WriteLine("Won't try to access S3 on debug build");
#endif
        }
    }
}
