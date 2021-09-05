using CrowbarWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            downloadXML();
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

        static async Task downloadXML()
        {
            AssumeRoleRequest assumeRequest = new AssumeRoleRequest
            {
                RoleArn = "arn:aws:iam::383519745720:role/Crowbar_S3-Access",
                DurationSeconds = 3600,
                Policy =
                    "{\"Version\": \"2012-10-17\",\"Statement\": [{\"Sid\": \"VisualEditor0\",\"Effect\": \"Allow\",\"Action\": \"sts:AssumeRole\",\"Resource\": \"arn:aws:iam::383519745720:role/Crowbar_S3-Access\",\"Condition\": {\"StringEquals\": {\"aws:RequestedRegion\": \"us-east-2\"}}}]}",
            };
            AmazonSecurityTokenServiceClient assumeRoleResult =
                new AmazonSecurityTokenServiceClient(RegionEndpoint.USEast2);
            AssumeRoleResponse response = assumeRoleResult.AssumeRoleAsync(assumeRequest).Result;
            AWSCredentials tempCredentials = new SessionAWSCredentials(
                response.Credentials.AccessKeyId,
                response.Credentials.SecretAccessKey,
                response.Credentials.SessionToken);
            AmazonS3Client s3Client = new AmazonS3Client(tempCredentials);
            string xmlDoc = ReadObjectDataAsync(s3Client, "crowbar-staticdata", "static_cameras.xml").Result;
            System.IO.File.WriteAllText("cameras.xml", xmlDoc);
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
    }
}
