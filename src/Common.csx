private static DateTime ConvertTimestampToDatetime(double unixTimestamp)
{
    // Unix timestamp is seconds past epoch
    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
    return dtDateTime.AddSeconds(unixTimestamp).ToLocalTime();
}

private static DateTime ConvertTimestampToDatetime(double unixTimestamp, TraceWriter log)
{
    // Unix timestamp is seconds past epoch
    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
    dtDateTime = dtDateTime.AddSeconds(unixTimestamp).ToLocalTime();

    log.Info($"Converted '{unixTimestamp}' to {dtDateTime}.");

    return dtDateTime;
}