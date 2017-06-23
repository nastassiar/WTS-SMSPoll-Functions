#r "Newtonsoft.Json"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

public static void Run(FacebookUpdates item, out object document, TraceWriter log)
{
    
    document = item.record;
    log.Info($"Document to be saved: {document}");
    
    
}

public class FacebookUpdates
{
    public string collectionName { get; set; }
    public object record { get; set; }
}
