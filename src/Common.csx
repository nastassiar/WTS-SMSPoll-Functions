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

private static DateTime ConvertTimestampToDatetime(string unixTimestamp, TraceWriter log)
{
    double unixDate = 0;
    System.DateTime dtDateTime = System.DateTime.MinValue;

    try
    {
        log.Info($"Converting '{unixTimestamp}' to a double.");
        unixDate = Convert.ToDouble(unixTimestamp);
        log.Info($"Converted '{unixTimestamp}' to {unixDate}.");

        dtDateTime = ConvertTimestampToDatetime(unixDate, log);
    }
    catch (FormatException)
    {
        string error = $"Unable to convert '{unixTimestamp}' to a Double.";
        log.Info(error);
    }
    catch (OverflowException)
    {
        string error = $"'{unixTimestamp}' is outside the range of a Double.";
        log.Info(error);
    }

    return dtDateTime;
}
