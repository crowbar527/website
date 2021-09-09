using CrowbarWebsite.Helpers;
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
        private static string mainAccountAccess = "AKIAQHSUYTC4ZUPJYWRK";
        private static string mainAccountSecret = "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";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Download Static Camera XML
            //string xmlstr = await downloadXML();
            string xmlstr = "<?xml version=\"1.0\" encoding=\"utf-8\"?><static-cameras><camera><type>STATIC</type><street>Twin Coast Discovery Highway </street><area>Twin Coast Discovery Highway </area><startpos><lat>-36.170245</lat><long>174.447012</long></startpos><endpos><lat>-36.190333</lat><long>174.450727</long></endpos><installed>Aug-2018</installed></camera><camera><type>STATIC</type><street>Great North Road </street><area>Great North Road </area><startpos><lat>-35.649532</lat><long>174.295668</long></startpos><endpos><lat>-35.660956</lat><long>174.293687</long></endpos><installed>Jan-2018</installed></camera><camera><type>STATIC</type><street>West Coast Road </street><area>West Coast Road </area><startpos><lat>-36.909258</lat><long>174.634322</long></startpos><endpos><lat>-36.908515</lat><long>174.64278</long></endpos><installed>Nov-2015</installed></camera><camera><type>STATIC</type><street>Candia Road </street><area>Candia Road </area><startpos><lat>-36.897066</lat><long>174.596018</long></startpos><endpos><lat>-36.868174</lat><long>174.589258</long></endpos><installed>Aug-2015</installed></camera><camera><type>STATIC</type><street>Great North Road </street><area>Great North Road </area><startpos><lat>-36.9089</lat><long>174.666421</long></startpos><endpos><lat>-36.909201</lat><long>174.672753</long></endpos><installed>Mar-2015</installed></camera><camera><type>STATIC</type><street>Old North Road </street><area>Old North Road </area><startpos><lat>-36.776609</lat><long>174.576509</long></startpos><endpos><lat>-36.787316</lat><long>174.576956</long></endpos><installed>Jun-2017</installed></camera><camera><type>STATIC</type><street>Coatesville-Riverhead Highway </street><area>Coatesville-Riverhead Highway </area><startpos><lat>-36.73427</lat><long>174.623297</long></startpos><endpos><lat>-36.740262</lat><long>174.621261</long></endpos><installed>Jul-2018</installed></camera><camera><type>STATIC</type><street>State Highway 1 </street><area>State Highway 1 </area><startpos><lat>-36.329138</lat><long>174.546518</long></startpos><endpos><lat>-36.303463</lat><long>174.52711</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>State Highway 17 </street><area>State Highway 17 </area><startpos><lat>-36.714135</lat><long>174.674193</long></startpos><endpos><lat>-36.721391</lat><long>174.688924</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>Great North Road </street><area>Great North Road </area><startpos><lat>-36.879606</lat><long>174.635283</long></startpos><endpos><lat>-36.881554</lat><long>174.639746</long></endpos><installed>Aug-2018</installed></camera><camera><type>STATIC</type><street>Twin Coast Discovery Highway </street><area>Twin Coast Discovery Highway </area><startpos><lat>-36.359092</lat><long>174.603529</long></startpos><endpos><lat>-36.348726</lat><long>174.590635</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>Rata Street </street><area>Rata Street </area><startpos><lat>-36.90669</lat><long>174.678493</long></startpos><endpos><lat>-36.903267</lat><long>174.681476</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>East Coast Road </street><area>East Coast Road </area><startpos><lat>-36.697902</lat><long>174.717437</long></startpos><endpos><lat>-36.688736</lat><long>174.705979</long></endpos><installed>Dec-2017</installed></camera><camera><type>STATIC</type><street>Ngapipi Road </street><area>Ngapipi Road </area><startpos><lat>-36.856688</lat><long>174.810312</long></startpos><endpos><lat>-36.852601</lat><long>174.805502</long></endpos><installed>Feb-2017</installed></camera><camera><type>STATIC</type><street>Tamaki Drive </street><area>Tamaki Drive </area><startpos><lat>-36.847683</lat><long>174.78771</long></startpos><endpos><lat>-36.852929</lat><long>174.805641</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>Hillsborough Road </street><area>Hillsborough Road </area><startpos><lat>-36.922161</lat><long>174.754243</long></startpos><endpos><lat>-36.924856</lat><long>174.752129</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>Great North Road </street><area>Great North Road </area><startpos><lat>-36.866494</lat><long>174.738467</long></startpos><endpos><lat>-36.866773</lat><long>174.74017</long></endpos><installed>Apr-2018</installed></camera><camera><type>STATIC</type><street>Great South Road </street><area>Great South Road </area><startpos><lat>-36.957346</lat><long>174.852735</long></startpos><endpos><lat>-36.953253</lat><long>174.84674</long></endpos><installed>Feb-2015</installed></camera><camera><type>STATIC</type><street>Mill Road </street><area>Mill Road </area><startpos><lat>-36.995929</lat><long>174.922528</long></startpos><endpos><lat>-37.010572</lat><long>174.93542</long></endpos><installed>Dec-2014</installed></camera><camera><type>STATIC</type><street>Murphys Road </street><area>Murphys Road </area><startpos><lat>-36.989559</lat><long>174.915508</long></startpos><endpos><lat>-36.967326</lat><long>174.924771</long></endpos><installed>Dec-2014</installed></camera><camera><type>STATIC</type><street>Mahia Road </street><area>Mahia Road </area><startpos><lat>-37.03814</lat><long>174.88803</long></startpos><endpos><lat>-37.039674</lat><long>174.883694</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>Massey Road </street><area>Massey Road </area><startpos><lat>-36.976446</lat><long>174.80071</long></startpos><endpos><lat>-36.977587</lat><long>174.804097</long></endpos><installed>Apr-2018</installed></camera><camera><type>STATIC</type><street>Waiuku Road </street><area>Waiuku Road </area><startpos><lat>-37.214831</lat><long>174.797426</long></startpos><endpos><lat>-37.219555</lat><long>174.780204</long></endpos><installed>Jan-2018</installed></camera><camera><type>STATIC</type><street>Glenbrook Road </street><area>Glenbrook Road </area><startpos><lat>-37.171918</lat><long>174.797638</long></startpos><endpos><lat>-37.176456</lat><long>174.785253</long></endpos><installed>Apr-2018</installed></camera><camera><type>STATIC</type><street>Awhitu Road </street><area>Awhitu Road </area><startpos><lat>-37.226418</lat><long>174.700419</long></startpos><endpos><lat>-37.231488</lat><long>174.70224</long></endpos><installed>Jan-2018</installed></camera><camera><type>STATIC</type><street>State Highway 2 </street><area>State Highway 2 </area><startpos><lat>-37.253839</lat><long>175.239172</long></startpos><endpos><lat>-37.247991</lat><long>175.18874</long></endpos><installed>Jan-2018</installed></camera><camera><type>STATIC</type><street>State Highway 2 </street><area>State Highway 2 </area><startpos><lat>-37.413266</lat><long>175.778624</long></startpos><endpos><lat>-37.41261</lat><long>175.771779</long></endpos><installed>Apr-2018</installed></camera><camera><type>STATIC</type><street>Main Road </street><area>Main Road </area><startpos><lat>-37.83374</lat><long>174.917244</long></startpos><endpos><lat>-37.834374</lat><long>174.940296</long></endpos><installed>Apr-2018</installed></camera><camera><type>STATIC</type><street>State Highway 29 </street><area>State Highway 29 </area><startpos><lat>-37.879988</lat><long>175.89201</long></startpos><endpos><lat>-37.882796</lat><long>175.902924</long></endpos><installed>May-2018</installed></camera><camera><type>STATIC</type><street>Otorohanga Road </street><area>Otorohanga Road </area><startpos><lat>-38.160194</lat><long>175.272309</long></startpos><endpos><lat>-38.177086</lat><long>175.246914</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>State Highway 2 </street><area>State Highway 2 </area><startpos><lat>-37.402826</lat><long>175.812433</long></startpos><endpos><lat>-37.400593</lat><long>175.82313</long></endpos><installed>Jul-2018</installed></camera><camera><type>STATIC</type><street>Te Awamutu Cambridge Road </street><area>Te Awamutu Cambridge Road </area><startpos><lat>-37.924079</lat><long>175.434617</long></startpos><endpos><lat>-37.932778</lat><long>175.429719</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>State Highway 2 </street><area>State Highway 2 </area><startpos><lat>-37.834581</lat><long>176.593268</long></startpos><endpos><lat>-37.835581</lat><long>176.600478</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>State Highway 3 </street><area>State Highway 3 </area><startpos><lat>-39.626946</lat><long>174.369921</long></startpos><endpos><lat>-39.631213</lat><long>174.377168</long></endpos><installed>Jun-2018</installed></camera><camera><type>STATIC</type><street>State Highway 56 </street><area>State Highway 56 </area><startpos><lat>-40.40276</lat><long>175.50014</long></startpos><endpos><lat>-40.4082</lat><long>175.488532</long></endpos><installed>Nov-2018</installed></camera><camera><type>STATIC</type><street>State Highway 1 </street><area>State Highway 1 </area><startpos><lat>-41.23178</lat><long>174.80985</long></startpos><endpos><lat>-41.242505</lat><long>174.813068</long></endpos><installed>Jul-2014</installed></camera><camera><type>STATIC</type><street>Wainuiomata Road </street><area>Wainuiomata Road </area><startpos><lat>-41.230198</lat><long>174.91905</long></startpos><endpos><lat>-41.253402</lat><long>174.928128</long></endpos><installed>Oct-2014</installed></camera><camera><type>STATIC</type><street>Whitford Brown Avenue </street><area>Whitford Brown Avenue </area><startpos><lat>-41.121885</lat><long>174.854932</long></startpos><endpos><lat>-41.121319</lat><long>174.866798</long></endpos><installed>Oct-2014</installed></camera><camera><type>STATIC</type><street>State Highway 1 </street><area>State Highway 1 </area><startpos><lat>-41.273876</lat><long>174.779398</long></startpos><endpos><lat>-41.287117</lat><long>174.771724</long></endpos><installed>Nov-2014</installed></camera><camera><type>STATIC</type><street>Wainui Road </street><area>Wainui Road </area><startpos><lat>-41.227782</lat><long>174.915852</long></startpos><endpos><lat>-41.228977</lat><long>174.917965</long></endpos><installed>Oct-2014</installed></camera><camera><type>STATIC</type><street>Hutt Road </street><area>Hutt Road </area><startpos><lat>-41.226858</lat><long>174.850459</long></startpos><endpos><lat>-41.223824</lat><long>174.859986</long></endpos><installed>Dec-2014</installed></camera><camera><type>STATIC</type><street>State Highway 1 </street><area>State Highway 1 </area><startpos><lat>-44.256131</lat><long>171.274527</long></startpos><endpos><lat>-44.258896</lat><long>171.272488</long></endpos><installed>May-2018</installed></camera><camera><type>STATIC</type><street>Leeston Road </street><area>Leeston Road </area><startpos><lat>-43.643211</lat><long>172.426578</long></startpos><endpos><lat>-43.661453</lat><long>172.419255</long></endpos><installed>Apr-2018</installed></camera><camera><type>STATIC</type><street>Dunedin Southern Motorway </street><area>Dunedin Southern Motorway </area><startpos><lat>-45.901681</lat><long>170.438882</long></startpos><endpos><lat>-45.900118</lat><long>170.455011</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>King Edward Street </street><area>King Edward Street </area><startpos><lat>-45.890143</lat><long>170.49581</long></startpos><endpos><lat>-45.891849</lat><long>170.497246</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>Otatara Road </street><area>Otatara Road </area><startpos><lat>-46.396552</lat><long>168.301982</long></startpos><endpos><lat>-46.406937</lat><long>168.293665</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>Maclaggan Street </street><area>Maclaggan Street </area><startpos><lat>-45.878631</lat><long>170.495084</long></startpos><endpos><lat>-45.877155</lat><long>170.499925</long></endpos><installed>Mar-2018</installed></camera><camera><type>STATIC</type><street>Wansbeck Street </street><area>Wansbeck Street </area><startpos><lat>-45.102626</lat><long>170.955619</long></startpos><endpos><lat>-45.101622</lat><long>170.949713</long></endpos><installed>Mar-2018</installed></camera><camera><type>TRAFFIC</type><street>Halsey St. / Fanshawe St. Intersect.</street><area>Halsey St. / Fanshawe St. Intersect.</area><startpos><lat>-36.846043</lat><long>174.56528</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Ash St. / Rosebank Rd. Intersect.</street><area>Ash St. / Rosebank Rd. Intersect.</area><startpos><lat>-36.893612</lat><long>174.692825</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Pigeon Mt. Rd. / Rakuranga Rd. Intersect.</street><area>Pigeon Mt. Rd. / Rakuranga Rd. Intersect.</area><startpos><lat>-36.899078</lat><long>174.902333</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Te Irirangi Dr. / Smales Rd. Intersect.</street><area>Te Irirangi Dr. / Smales Rd. Intersect.</area><startpos><lat>-36.942131</lat><long>174.908419</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Chapel Rd. / Stancombe Rd. Intersect</street><area>Chapel Rd. / Stancombe Rd. Intersect</area><startpos><lat>-36.957418</lat><long>174.908626</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Lambie Dr. Interchange (East Bound Offramp)</street><area>Lambie Dr. Interchange (East Bound Offramp)</area><startpos><lat>-36.995204</lat><long>174.874315</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Te Irirangi Dr. / Ti Rakau Dr. Intersect.</street><area>Te Irirangi Dr. / Ti Rakau Dr. Intersect.</area><startpos><lat>-36.928842</lat><long>174.912486</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera><camera><type>TRAFFIC</type><street>Karo Dr. / Victoria St. Intersect.</street><area>Karo Dr. / Victoria St. Intersect.</area><startpos><lat>-41.2961</lat><long>174.771709</long></startpos><endpos><lat>0</lat><long>0</long></endpos><installed>Jan-2015</installed></camera></static-cameras>";

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
    }
}
