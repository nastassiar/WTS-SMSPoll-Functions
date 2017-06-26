#r "Newtonsoft.Json"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

public static void Run(string item, ICollector<object> output, TraceWriter log)
{
    dynamic i = JObject.Parse(item);

    var surveys = i.client_surveys;

    // Need to do this since the API doesn't return a list!
    JObject attributesAsJObject = i.client_surveys;
    Dictionary<string, dynamic> values = attributesAsJObject.ToObject<Dictionary<string, object>>();
    foreach(KeyValuePair<string, dynamic> entry in values)
    {
        dynamic e = entry.Value;
        IEnumerable<dynamic> responses = e.responses;
        // From Responses
        string name =  responses.FirstOrDefault(p => p.qid == "NAME")?.parsed_response; //"qid":"NAME"
        int yob = responses.FirstOrDefault(p => p.qid == "Y.O.B")?.parsed_response;// "qid":"Y.O.B"
        string gender = responses.FirstOrDefault(p => p.qid == "GENDER")?.parsed_response; // "qid":"GENDER"
        string county = responses.FirstOrDefault(p => p.qid == "COUNTY")?.parsed_response;; // "qid":"COUNTY"

        var obj = new {
            startTimestamp = e.start_timestamp,
            completeTimestamp = e.complete_timestamp,
            phoneNumber = i.phone,
            lat = i.lat,
            lon = i.lon,
            gender = gender == "1" ? "Female" : "Male",
            yob = yob,
            name = name ?? i.name, 
            county = county ?? i.locationTextRaw
        };
        
        log.Info("OBJ : "+obj);
        output.Add(obj);
    }
}
