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
private static string _api_TZ_Eid = GetEnvironmentVariable("EchoApi_Tanzania_Eid");
private static string _api_TZ_Password = GetEnvironmentVariable("EchoApi_Tanzania_Password");
private static string _api_TZ_MaxPageSize = GetEnvironmentVariable("EchoApi_Tanzania_SurveyMaxPageSize");
private static string _api_TZ_SurveySid = GetEnvironmentVariable("EchoApi_Tanzania_SurveySid");
private static string _blobStorageConnectionString = GetEnvironmentVariable("BlobStorageConnectionString");

private static string _lastExecTimeBlobName = "EchoApi-SurveyTanzaniaLastExecutionTime";
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
    string urlPath = $"https://m-swali-hrd.appspot.com/api/v2/survey_data?auth={_apiAuth}&eid={_api_TZ_Eid}&password={_api_TZ_Password}&source=5&since={since}&until={until}&sid={_api_TZ_SurveySid}&page_indexing=0&page={requestPage}&max={_api_TZ_MaxPageSize}";
    Uri uri = new Uri(urlPath);

    string urlPathModified = urlPath.Replace($"password={_api_TZ_Password}", "password=*******");
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
                    location = "TZ",
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
