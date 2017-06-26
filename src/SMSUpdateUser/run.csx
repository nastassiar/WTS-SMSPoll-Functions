#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(SMSUserInfo item, IEnumerable<dynamic> documents, out object outputDocument, TraceWriter log)
{
    //log.Info("item : "+ item);
    // TODO: Check if there is more than one user record?
    dynamic existingUser = documents.FirstOrDefault();
    if (existingUser != null)
    {
        log.Info("User found! Update!");
        
        //log.Info("inputDocument : "+ existingUser);
        existingUser.Id = item.phoneNumber;
        existingUser.phoneNumber = item.phoneNumber ?? existingUser.phoneNumber ;
        existingUser.name = item.name ?? existingUser.name;
        existingUser.startTimestamp = item.startTimestamp  ?? existingUser.startTimeStamp;
        existingUser.completeTimestamp = item.completeTimestamp  ?? existingUser.completeTimestamp;
        existingUser.lat = item.lat ?? existingUser.lat;
        existingUser.lon = item.lon ?? existingUser.lon;
        existingUser.gender = item.gender ?? existingUser.gender;
        existingUser.yob = item.yob ?? existingUser.yob;
        existingUser.county = item.county ?? existingUser.county;
        outputDocument = existingUser;
    }
    else 
    {
        log.Info("User not found! Create!");
        outputDocument = new 
        {
            Id = item.phoneNumber,
            phoneNumber = item.phoneNumber,
            name = item.name,
            startTimestamp = item.startTimestamp,
            completeTimestamp = item.completeTimestamp,
            lat = item.lat,
            lon = item.lon,
            gender = item.gender,
            yob = item.yob,
            county = item.county
        };
    }
    
    //log.Info("outDocument : "+ outputDocument);
}

public class SMSUserInfo
{
    public string phoneNumber { get; set; }
    public string name { get; set; }
    public string startTimestamp { get; set; }
    public string completeTimestamp { get; set; }
    public double? lat { get; set; }
    public double? lon { get; set; }
    public string gender { get; set; }
    public int? yob { get; set; }
    public string county { get; set; }
}