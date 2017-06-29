#r "Newtonsoft.Json"

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
private static string _apiEid = GetEnvironmentVariable("EchoApi_Tanzania_Eid");
private static string _apiPassword = GetEnvironmentVariable("EchoApi_Tanzania_Password");
private static string _apiMaxPageSize = GetEnvironmentVariable("EchoApi_Tanzania_SMSMaxPageSize");
private static string _blobStorageConnectionString = GetEnvironmentVariable("BlobStorageConnectionString");

private static string _lastExecTimeBlobName = "EchoApi-MsgTanzaniaLastExecutionTime";
private static string _lastExecTimeContainerName = "echo-api-storage";

public static void Run(TimerInfo myTimer, ICollector<object> outputSbMsg, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    long unixLastExecTime = GetUnixLastExecTimestamp(_lastExecTimeBlobName, _lastExecTimeContainerName, log);
    log.Info($"UnixLastExecTime retrieved: {unixLastExecTime}");

    int requestPage = 0;
    int msgProcessed = 0;
    int totalMsgProcessed = 0;
    long currentTimestamp = GetCurrentDateTimeInUnix();
    do
    {
        log.Info($"Processing page: {requestPage}");

        msgProcessed = PollSMSMessages(requestPage, unixLastExecTime, currentTimestamp, outputSbMsg, log);

        log.Info($"Processed : {msgProcessed} messages.");

        totalMsgProcessed += msgProcessed;
        requestPage += 1;

    } while (msgProcessed > 0);

    log.Info($"******   Processed a total of: {totalMsgProcessed} messages.  ******");

    UpdateBlobLastExecTime(_lastExecTimeBlobName, _lastExecTimeContainerName, currentTimestamp, log);

    log.Info("Done Processing !");
}

private static int PollSMSMessages(int requestPage, long since, long until, ICollector<object> outputSbMsg, TraceWriter log)
{
    string restMethod = "GET";
    string urlPath = $"https://m-swali-hrd.appspot.com/api/cms/msglog?auth={_apiAuth}&eid={_apiEid}&password={_apiPassword}&source=5&since={since}&until={until}&page={requestPage}&max={_apiMaxPageSize}";
    Uri uri = new Uri(urlPath);

    string urlPathModified = urlPath.Replace($"password={_apiPassword}", "password=*******");
    log.Info(urlPathModified);
    
    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
    request.Method = restMethod;

    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    {
        log.Info($"Executed Call: {response.StatusCode}");

        Stream Answer = response.GetResponseStream();
        StreamReader _Answer = new StreamReader(Answer);

        dynamic d = Newtonsoft.Json.Linq.JObject.Parse(_Answer.ReadToEnd());
        _Answer.Close();

        var msgArray = d.messages;
        if (msgArray != null && msgArray.Count > 0)
        {
            log.Info($"Read: {msgArray.Count} messages.");

            foreach (var msg in msgArray)
            {
                // Send Message to ServiceBus Queue 
                outputSbMsg.Add(msg);
            }

            return msgArray.Count;
        }
        else
        {
            log.Info("No messages to read");

            return 0;
        }
    }
}