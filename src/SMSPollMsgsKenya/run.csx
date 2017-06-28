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
private static string _apiEid = GetEnvironmentVariable("EchoApi_Kenya_Eid");
private static string _apiPassword = GetEnvironmentVariable("EchoApi_Kenya_Password");
private static string _apiMaxPageSize = GetEnvironmentVariable("EchoApi_Kenya_SMSMaxPageSize");
private static string _blobStorageConnectionString = GetEnvironmentVariable("BlobStorageConnectionString");

private static string _lastExecTimeBlobName = "EchoApi-MsgKenyaLastExecutionTime";
private static string _lastExecTimeContainerName = "echo-api-storage";

private static CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(_blobStorageConnectionString);
private static CloudBlobClient _blobClient = _storageAccount.CreateCloudBlobClient();

public static void Run(TimerInfo myTimer, ICollector<object> outputSbMsg, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    log.Info($"_apiAuth : {_apiAuth}");
    log.Info($"_apiAuth : {_apiEid}");
    log.Info($"_apiAuth : {_apiMaxPageSize}");
    log.Info($"_apiAuth : {_blobStorageConnectionString}");

    long unixLastExecTime = GetUnixLastExecTimestamp(_lastExecTimeBlobName, _lastExecTimeContainerName, log);
    log.Info($"UnixLastExecTime retrieved: {unixLastExecTime}");

    int requestPage = 0;
    bool moreToProcess = false;
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

    string urlPathModified = urlPath.Replace($"password={_apiPassword}", "password=**** :) ****");
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

private static long GetUnixLastExecTimestamp(string blobName, string containerName, TraceWriter log)
{
    log.Info($"Entering : GetUnixLastExecTimestamp");

    long unixLastExecTimestamp = 0;

    if (!BlobExists(blobName, containerName, log))
    {
        // Get current datetime minus 1 hour
        unixLastExecTimestamp = GetCurrentDateTimeInUnixAddHours(-1);

        // Create the blob
        UpdateBlobLastExecTime(blobName, containerName, unixLastExecTimestamp, log);
    }
    else
    {
        string blobContent = GetBlobContent(blobName, containerName, log);

        bool parsed = long.TryParse(blobContent, out unixLastExecTimestamp);
        if (!parsed)
            unixLastExecTimestamp = 0;
    }

    log.Info($"Exiting : GetUnixLastExecTimestamp");

    return unixLastExecTimestamp;
}

private static string GetBlobContent(string blobName, string containerName, TraceWriter log)
{
    log.Info($"Entering : GetBlobContent");

    // Retrieve a reference to a container.
    CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

    // Create the container if it doesn't already exist.
    container.CreateIfNotExists();

    string blobContent = String.Empty;
    if (BlobExists(blobName, containerName, log))
    {
        // Retrieve reference to the blob "blobName"
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

        using (var memoryStream = new MemoryStream())
        {
            blockBlob.DownloadToStream(memoryStream);
            blobContent = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }

    log.Info($"Exiting : GetBlobContent");

    return blobContent;
}

private static bool BlobExists(string blobName, string containerName, TraceWriter log)
{
    log.Info($"Entering : BlobExists(string blobName, string containerName, TraceWriter log)");

    log.Info($"Exiting : BlobExists");

    return BlobExists(_blobClient, blobName, containerName, log);
}

private static bool BlobExists(CloudBlobClient blobClient, string blobName, string containerName, TraceWriter log)
{
    log.Info($"Entering : BlobExists(CloudBlobClient blobClient, string blobName, string containerName, TraceWriter log)");

    // Just in case the container does not exists

    // Retrieve a reference to a container.
    CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

    // Create the container if it doesn't already exist.
    container.CreateIfNotExists();

    return blobClient.GetContainerReference(containerName)
                    .GetBlockBlobReference(blobName)
                    .Exists();
}

private static void UpdateBlobLastExecTime(string blobName, string containerName, long value, TraceWriter log)
{
    log.Info($"Entering : UpdateBlobLastExecTime");
    
    // Retrieve reference to a previously created container.
    CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

    // Retrieve reference to a blob named blobName.
    CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

    // Create or overwrite the "myblob" blob with contents from a local file.
    blockBlob.UploadText(value.ToString());

    log.Info($"Exiting : UpdateBlobLastExecTime");
}

private static long GetCurrentDateTimeInUnix()
{
    DateTimeOffset dtOffsetNow = DateTimeOffset.Now;
    return dtOffsetNow.ToUnixTimeSeconds();
}

private static long GetCurrentDateTimeInUnixAddHours(double hoursToAdd)
{
    DateTimeOffset dtOffsetNow = DateTimeOffset.Now;
    return dtOffsetNow.AddHours(hoursToAdd).ToUnixTimeSeconds();
}

private static string GetEnvironmentVariable(string name)
{
    return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
