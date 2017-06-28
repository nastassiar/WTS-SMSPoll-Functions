#r "Newtonsoft.Json"

 #load "../Common.csx"

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(string queueItem, out string outSBMessage, TraceWriter log)
{
    log.Info("SMSDataProcess function processing message");

    // Just save the object
    dynamic msg = JObject.Parse(queueItem);

    log.Info($"Received: {queueItem}");

    var entry = new
    {
        type = "smsmsg",
        status = msg.status,
        direction = msg.direction,
        phone = msg.phone,
        ts_send = msg.ts_send,
        dt_send = ConvertTimestampToDatetime(msg.ts_send.Value, log),
        message = msg.message,
        delivery_status = msg.delivery_status,
        charge = msg.charge,
        ts_requested = msg.ts_requested,
        dt_requested = ConvertTimestampToDatetime(msg.ts_requested.Value, log),
        channel_text = msg.channel_text
    };

    log.Info($"phone: {msg.phone.Value}");
    log.Info($"message: {msg.message.Value}");
    log.Info($"dt_send: {msg.dt_send.Value}");
    log.Info($"dt_requested: {msg.dt_requested.Value}");

    string json = JsonConvert.SerializeObject(entry);

    outSBMessage = json;
}