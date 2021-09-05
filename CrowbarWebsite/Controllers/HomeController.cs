using CrowbarWebsite.Helpers;
using CrowbarWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrowbarWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Download Static Camera XML
            string xmlstr = "<?xml version=\"1.0\" encoding=\"utf-8\"?><static-cameras><camera><street>Twin Coast Discovery Highway </street><area>Twin Coast Discovery Highway </area><startpos><lat>-36.170245</lat><long>174.447012</long></startpos><endpos><lat>-36.190333</lat><long>174.450727</long></endpos><installed>Aug-2018</installed></camera></static-cameras>";

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

            //Pass to Page
            return View(cameras);
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
