#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"GetCurrentDateTimeInUnix was triggered!");

    // We expect nothing in the request
    string jsonContent = await req.Content.ReadAsStringAsync();

    DateTimeOffset dtOffsetNow = DateTimeOffset.UtcNow;
    log.Info($"dtOffsetNow is {dtOffsetNow}");

    long unixTimeStampInSeconds = dtOffsetNow.ToUnixTimeSeconds();
    log.Info($"unixTimeStampInSeconds is {unixTimeStampInSeconds}");

    return req.CreateResponse(HttpStatusCode.OK, new
    {
        currentdtInUnixTimeStamp = unixTimeStampInSeconds
    }); 
}