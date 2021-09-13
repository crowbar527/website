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

        public async Task<IActionResult> Index()
        {
            /*
#if DEBUG
            var data = await getCrashData("KILLARNEY ROAD");
#endif
            */

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
                    {
                        var crashData = await getCrashData(camera.Street.ToUpper());
                        camera.setCrashInfo(crashData);
                        cameras.Add(camera);
                    }

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

        static async Task<string> downloadXML()
        {
            var basicCreds = new BasicAWSCredentials(mainAccountAccess, mainAccountSecret);
            var stsClient = new AmazonSecurityTokenServiceClient(basicCreds);
            var sessionResponse = await stsClient.GetSessionTokenAsync();

            var sessionCreds = new SessionAWSCredentials(sessionResponse.Credentials.AccessKeyId,
                sessionResponse.Credentials.SecretAccessKey, sessionResponse.Credentials.SessionToken);
            AWSCredentials tempCredentials =
                new AssumeRoleAWSCredentials(sessionCreds, "arn:aws:iam::383519745720:role/Crowbar_S3-Access", "crowbarwebsite");
            AmazonS3Client s3Client = new AmazonS3Client(tempCredentials, RegionEndpoint.USEast2);
            try
            {
                string xmlDoc = ReadObjectDataAsync(s3Client, "crowbar-staticdata", "static_cameras.xml").Result;
                return xmlDoc;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }

            
        }

        static async Task<string> ReadObjectDataAsync(AmazonS3Client client, String bucketName, String keyName)
        {
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = reader.ReadToEnd(); // Now you process the response body.
                }

                return responseBody;
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                return "";
            }
        }

        static async Task<QueryResponse<Point>> getCrashData(string location)
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
    }
}
