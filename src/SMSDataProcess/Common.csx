private static DateTime ConvertTimestampToDatetime(double unixTimestamp)
{
    // Unix timestamp is seconds past epoch
    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    return dtDateTime.AddSeconds(unixTimestamp).ToLocalTime();
}