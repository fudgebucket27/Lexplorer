using System;

namespace Lexplorer.Helpers
{
    public static class TimestampConverter
    {
        public static string? ToUTCString(string? unixTimeStamp)
        {
            if (unixTimeStamp == null) return null;
            DateTime? dateTime = ToUTCDateTime(unixTimeStamp);
            if (dateTime == null) return null;
            return dateTime.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }

        public static DateTime? ToUTCDateTime(string? unixTimeStamp)
        {
            if (unixTimeStamp == null) return null;
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(unixTimeStamp)).UtcDateTime;
        }

        public static double? ToTimeStamp(DateTime? dateTimeUTC)
        {
            if (dateTimeUTC == null) return null;
            return ((DateTimeOffset)dateTimeUTC).ToUnixTimeSeconds();
        }
    }
}
