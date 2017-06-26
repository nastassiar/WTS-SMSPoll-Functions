
using System;

public static void Run(object item, out object document, TraceWriter log)
{
    document = item;
    log.Info($"Document to be saved: {document}");
}
