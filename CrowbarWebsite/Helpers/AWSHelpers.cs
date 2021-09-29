using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AspNetCore.Identity.DynamoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CrowbarWebsite.Helpers
{
    internal class AWSHelpers
    {
        //These are needed for service access, previous keys in commit history are invalid!
        private static string mainAccountAccess = "";
        private static string mainAccountSecret = "";

        internal static async Task<string> downloadXML()
        {
            var basicCreds = new BasicAWSCredentials(mainAccountAccess, mainAccountSecret);
            var stsClient = new AmazonSecurityTokenServiceClient(basicCreds);
            var sessionResponse = await stsClient.GetSessionTokenAsync();

            var sessionCreds = new SessionAWSCredentials(sessionResponse.Credentials.AccessKeyId,
                sessionResponse.Credentials.SecretAccessKey, sessionResponse.Credentials.SessionToken);
            AWSCredentials tempCredentials =
                new AssumeRoleAWSCredentials(sessionCreds, "insertyourrolehere", "crowbarwebsite");
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

        public static async Task<Dictionary<string, string>> GetSecret()
        {
            string secretName = "prod/GOOGL";
            string region = "ap-southeast-2";
            string secret = "";

            MemoryStream memoryStream = new MemoryStream();

            var basicCreds = new BasicAWSCredentials(mainAccountAccess, mainAccountSecret);
            var stsClient = new AmazonSecurityTokenServiceClient(basicCreds);
            var sessionResponse = await stsClient.GetSessionTokenAsync();

            var sessionCreds = new SessionAWSCredentials(sessionResponse.Credentials.AccessKeyId,
                sessionResponse.Credentials.SecretAccessKey, sessionResponse.Credentials.SessionToken);

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(sessionCreds, RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

            GetSecretValueResponse response = null;

            // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InvalidParameterException e)
            {
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                throw;
            }
            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (ResourceNotFoundException e)
            {
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(secret);
            return data;
            // Your code goes here.
        }

        public static async Task<AWSCredentials> getCreds()
        {
            var basicCreds = new BasicAWSCredentials(mainAccountAccess, mainAccountSecret);
            var stsClient = new AmazonSecurityTokenServiceClient(basicCreds);
            var sessionResponse = await stsClient.GetSessionTokenAsync();

            var sessionCreds = new SessionAWSCredentials(sessionResponse.Credentials.AccessKeyId,
                sessionResponse.Credentials.SecretAccessKey, sessionResponse.Credentials.SessionToken);
            return sessionCreds;
        }
    }
}
