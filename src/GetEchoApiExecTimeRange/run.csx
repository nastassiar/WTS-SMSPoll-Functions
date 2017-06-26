#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"GetEchoApiExecTimeRange was triggered!");

    // We expect nothing in the request
    string jsonContent = await req.Content.ReadAsStringAsync();

    DateTimeOffset dtOffsetNow = DateTimeOffset.UtcNow;
    log.Info($"dtOffsetNow is {dtOffsetNow}");

    long nowInUnixTimeStamp = dtOffsetNow.ToUnixTimeSeconds();
    log.Info($"nowInUnixTimeStamp is {nowInUnixTimeStamp}");

    //
    // For this, we should get the last execution time from somewere...
    DateTimeOffset dtOffsetLastExec = DateTimeOffset.UtcNow.AddMinutes(-60);
    log.Info($"dtOffsetLastExec is {dtOffsetLastExec}");

    long lastExecTimeStamp = dtOffsetLastExec.ToUnixTimeSeconds();
    log.Info($"lastExecTimeStamp is {lastExecTimeStamp}");

    return req.CreateResponse(HttpStatusCode.OK, new
    {
        currentdtInUnixTimeStamp = nowInUnixTimeStamp,
        lastExecutionUnixTimeStamp = lastExecTimeStamp
    });
}
