#r "Newtonsoft.Json"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

public static void Run(object item, out object document, TraceWriter log)
{
    document = item;
    log.Info($"Document to be saved: {document}");
}
