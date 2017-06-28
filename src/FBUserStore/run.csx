#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(FacebookUserInfo queueItem, IEnumerable<dynamic> documents, out object outputDocument, TraceWriter log)
{
    //log.Info("queueItem : "+ queueItem);
    // TODO: Check if there is more than one user record?
    dynamic existingUser = documents.FirstOrDefault();
    if (existingUser != null)
    {
        log.Info("User found! Update!");
        //log.Info("inputDocument : "+ existingUser);
        existingUser.lastUpdatedTime = queueItem.createdTime;
        existingUser.facebookName = queueItem.senderName;
        outputDocument = existingUser;
    }
    else 
    {
        log.Info("User not found! Create!");
        outputDocument = new 
        {
            Id = queueItem.senderId,
            facebookName = queueItem.senderName,
            facebookId = queueItem.senderId,
            createdTime = queueItem.createdTime,
            lastUpdatedTime = queueItem.createdTime,
            source = "FB"
        };
    }
    
    //log.Info("outDocument : "+ outputDocument);
}

public class FacebookUserInfo
{
    public string senderId { get; set; }
    public string senderName { get; set; }
    public DateTime createdTime { get; set; }
}