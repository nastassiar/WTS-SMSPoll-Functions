#r "Newtonsoft.Json"

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(string queueItem, out object outputDocument, TraceWriter log)
{
    // Just save the object
    dynamic msg = JObject.Parse(queueItem);

    var phone = msg.phone;
    var message = msg.message;

    log.Info($"phone: {phone}");
    log.Info($"message: {message}");


    //string json = JsonConvert.SerializeObject(msg);

    outputDocument = msg;

    /*
    dynamic d = JObject.Parse(queueItem);

    var msgArray = d.messages;
    foreach (var msg in msgArray)
    {
        var phone = msg.phone;
        var message = msg.message;

        log.Info($"phone: {phone}");
        log.Info($"message: {message}");

        //string json = JsonConvert.SerializeObject(msg);

        outputDocument.AddAsync(msg);
    }
    */
}
