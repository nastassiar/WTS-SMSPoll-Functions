#r "Newtonsoft.Json"

 #load "../Common.csx"

using System;
using System.Web;
using System.Web.Http;
using System.Net;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

private static string _apiAuth = GetEnvironmentVariable("EchoApi_Auth");
private static string _apiEid = GetEnvironmentVariable("EchoApi_Kenya_Eid");
private static string _apiPassword = GetEnvironmentVariable("EchoApi_Kenya_Password");
private static string _apiMaxPageSize = GetEnvironmentVariable("EchoApi_Kenya_SurveyMaxPageSize");
private static string _apiSurveySid = GetEnvironmentVariable("EchoApi_Kenya_SurveySid");
private static string _blobStorageConnectionString = GetEnvironmentVariable("BlobStorageConnectionString");

private static string _lastExecTimeBlobName = "EchoApi-SurveyKenyaLastExecutionTime";
private static string _lastExecTimeContainerName = "echo-api-storage";

public static void Run(TimerInfo myTimer, ICollector<object> outputSbMsg, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    long unixLastExecTime = GetUnixLastExecTimestamp(_lastExecTimeBlobName, _lastExecTimeContainerName, log);
    log.Info($"UnixLastExecTime retrieved: {unixLastExecTime}");

    int requestPage = 0;
    int clientsProcessed = 0;
    int totalClientsProcessed = 0;
    long currentTimestamp = GetCurrentDateTimeInUnix();
    do
    {
        log.Info($"Processing page: {requestPage}");

        clientsProcessed = PollSMSClients(requestPage, unixLastExecTime, currentTimestamp, outputSbMsg, log);

        log.Info($"Processed : {clientsProcessed} clients.");

        totalClientsProcessed += clientsProcessed;
        requestPage += 1;

    } while (clientsProcessed > 0);

    log.Info($"******   Processed a total of: {totalClientsProcessed} clients.  ******");

    UpdateBlobLastExecTime(_lastExecTimeBlobName, _lastExecTimeContainerName, currentTimestamp, log);

    log.Info("Done Processing !");
}

private static int PollSMSClients(int requestPage, long since, long until, ICollector<object> outputSbMsg, TraceWriter log)
{
    string restMethod = "POST";
    string urlPath = $"https://m-swali-hrd.appspot.com/api/v2/survey_data?auth={_apiAuth}&eid={_apiEid}&password={_apiPassword}&source=5&since={since}&until={until}&sid={_apiSurveySid}&page_indexing=0&page={requestPage}&max={_apiMaxPageSize}";
    Uri uri = new Uri(urlPath);

    string urlPathModified = urlPath.Replace($"password={_apiPassword}", "password=*******");
    log.Info(urlPathModified);
    
    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
    request.Method = restMethod;
    request.ContentLength = 0;

    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    {
        log.Info($"Executed Call: {response.StatusCode}");

        Stream Answer = response.GetResponseStream();
        StreamReader _Answer = new StreamReader(Answer);

        dynamic d = Newtonsoft.Json.Linq.JObject.Parse(_Answer.ReadToEnd());
        _Answer.Close();

        var clientArray = d.clients;
        if (clientArray != null && clientArray.Count > 0)
        {
            log.Info($"Read: {clientArray.Count} clients.");

            foreach (var client in clientArray)
            {
                log.Info($"Client: {client}");
                
                var entry = new 
                {
                    type = "sms_survey",
                    location = "KE",
                    client = client
                };

                // Send Message to ServiceBus Queue 
                outputSbMsg.Add(entry);
            }

            return clientArray.Count;
        }
        else
        {
            log.Info("No messages to read");

            return 0;
        }
    }
}
