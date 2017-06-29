using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.ServiceBus.Messaging;

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

private static bool BlobExists(string blobName, string containerName, TraceWriter log)
{
    log.Info($"Entering : BlobExists(string blobName, string containerName, TraceWriter log)");

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobStorageConnectionString);
    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

    CloudBlobContainer container = blobClient.GetContainerReference(containerName);

    // Create the container if it doesn't already exist.
    container.CreateIfNotExists();

    return blobClient.GetContainerReference(containerName)
                    .GetBlockBlobReference(blobName)
                    .Exists();
}

private static string GetBlobContent(string blobName, string containerName, TraceWriter log)
{
    log.Info($"Entering : GetBlobContent");

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobStorageConnectionString);
    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

    CloudBlobContainer container = blobClient.GetContainerReference(containerName);

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

private static void UpdateBlobLastExecTime(string blobName, string containerName, long value, TraceWriter log)
{
    log.Info($"Entering : UpdateBlobLastExecTime");
    
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobStorageConnectionString);
    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

    // Retrieve reference to a previously created container.
    CloudBlobContainer container = blobClient.GetContainerReference(containerName);

    // Retrieve reference to a blob named blobName.
    CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

    // Create or overwrite the "myblob" blob with contents from a local file.
    blockBlob.UploadText(value.ToString());

    log.Info($"Exiting : UpdateBlobLastExecTime");
}