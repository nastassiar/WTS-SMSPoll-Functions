#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(FacebookUserInfo item, IEnumerable<dynamic> documents, out object outputDocument, TraceWriter log)
{
    //log.Info("item : "+ item);
    // TODO: Check if there is more than one user record?
    dynamic existingUser = documents.FirstOrDefault();
    if (existingUser != null)
    {
        log.Info("User found! Update!");
        //log.Info("inputDocument : "+ existingUser);
        existingUser.lastUpdatedTime = item.createdTime;
        existingUser.facebookName = item.senderName;
        outputDocument = existingUser;
    }
    else 
    {
        log.Info("User not found! Create!");
        outputDocument = new 
        {
            Id = item.senderId,
            facebookName = item.senderName,
            facebookId = item.senderId,
            createdTime = item.createdTime,
            lastUpdatedTime = item.createdTime
        };
    }
    
    //log.Info("outDocument : "+ outputDocument);
}

public class FacebookUserInfo
{
    public string senderId { get; set; }
    public string senderName { get; set; }
    public int createdTime { get; set; }
}