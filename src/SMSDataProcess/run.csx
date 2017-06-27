#r "Newtonsoft.Json"

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(string queueItem, out string outSBMessage, TraceWriter log)
{
    // Just save the object
    dynamic msg = JObject.Parse(queueItem);

    var entry = new
    {
        status = msg.status,
        direction = msg.direction,
        phone = msg.phone,
        ts_send = msg.ts_send,
        message = msg.message,
        delivery_status = msg.delivery_status,
        charge = msg.charge,
        ts_requested = msg.ts_requested,
        channel_text = msg.channel_text
    };

    log.Info($"phone: {entry.phone}");
    log.Info($"message: {entry.message}");

    string json = JsonConvert.SerializeObject(msg);

    outSBMessage = json;
}
