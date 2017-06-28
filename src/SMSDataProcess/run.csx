#r "Newtonsoft.Json"

 #load "./Common.csx"

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(string queueItem, out string outSBMessage, TraceWriter log)
{
    log.Info("SMSDataProcess function processing message");
    log.Info($"Received: {queueItem}");

    // Parse into a JSon Object
    dynamic msg = JObject.Parse(queueItem);

    var entry = new
    {
        type = "smsmsg",
        status = msg.status,
        direction = msg.direction,
        phone = msg.phone,
        ts_send = msg.ts_send,
        dt_send = msg.ts_send == null ? null : ConvertTimestampToDatetime(msg.ts_send.Value, log),
        message = msg.message,
        delivery_status = msg.delivery_status,
        charge = msg.charge,
        ts_requested = msg.ts_requested,
        dt_requested = msg.ts_requested == null ? null : ConvertTimestampToDatetime(msg.ts_requested.Value, log),
        ts_received = msg.ts_received,
        dt_received = msg.ts_received == null ? null : ConvertTimestampToDatetime(msg.ts_received.Value, log),
        channel_text = msg.channel_text
    };

    log.Info($"phone: {entry.phone}");
    log.Info($"message: {entry.message}");
    log.Info($"dt_send: {entry.dt_send}");
    log.Info($"dt_requested: {entry.dt_requested}");

    string json = JsonConvert.SerializeObject(entry);

    outSBMessage = json;
}