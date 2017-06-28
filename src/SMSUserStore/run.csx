#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(SMSUserInfo queueItem, IEnumerable<dynamic> documents, out object outputDocument, TraceWriter log)
{
    //log.Info("queueItem : "+ queueItem);
    // TODO: Check if there is more than one user record?
    dynamic existingUser = documents.FirstOrDefault();
    if (existingUser != null)
    {
        log.Info("User found! Update!");
        
        //log.Info("inputDocument : "+ existingUser);
        existingUser.Id = queueItem.phoneNumber;
        existingUser.phoneNumber = queueItem.phoneNumber ?? existingUser.phoneNumber;
        existingUser.name = queueItem.name ?? existingUser.name;
        existingUser.startTimestamp = queueItem.startTimestamp  ?? existingUser.startTimeStamp;
        existingUser.completeTimestamp = queueItem.completeTimestamp  ?? existingUser.completeTimestamp;
        existingUser.lat = queueItem.lat != 0 ? queueItem.lat : existingUser.lat;
        existingUser.lon = queueItem.lon != 0 ? queueItem.lon : existingUser.lon;
        existingUser.gender = queueItem.gender ?? existingUser.gender;
        existingUser.yob = queueItem.yob ?? existingUser.yob;
        existingUser.county = queueItem.county ?? existingUser.county;
        existingUser.country = queueItem.country ?? existingUser.country;
        existingUser.source = queueItem.source ?? existingUser.source;
        outputDocument = existingUser;
    }
    else 
    {
        log.Info("User not found! Create!");
        outputDocument = new 
        {
            source = queueItem.source,
            Id = queueItem.phoneNumber,
            phoneNumber = queueItem.phoneNumber,
            name = queueItem.name,
            startTimestamp = queueItem.startTimestamp,
            completeTimestamp = queueItem.completeTimestamp,
            lat = queueItem.lat,
            lon = queueItem.lon,
            gender = queueItem.gender,
            yob = queueItem.yob,
            county = queueItem.county,
            country = queueItem.country
        };
    }
    
    //log.Info("outDocument : "+ outputDocument);
}

public class SMSUserInfo
{
    public string source {get; set;}
    public string country { get; set; }
    public string phoneNumber { get; set; }
    public string name { get; set; }
    public string startTimestamp { get; set; }
    public string completeTimestamp { get; set; }
    public double? lat { get; set; }
    public double? lon { get; set; }
    public string gender { get; set; }
    public string yob { get; set; }
    public string county { get; set; }
}