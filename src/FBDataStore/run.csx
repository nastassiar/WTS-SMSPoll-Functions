
using System;

public static void Run(object queueItem, out object document, TraceWriter log)
{
    document = queueItem;
    log.Info($"Document to be saved: {document}");
}
