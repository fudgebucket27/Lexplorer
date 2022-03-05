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
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(Double.Parse(unixTimeStamp)).ToLocalTime().ToUniversalTime();
        }

        public static Double? ToTimeStamp(DateTime? dateTimeUTC)
        {
            if (dateTimeUTC == null) return null;
            return ((DateTimeOffset)dateTimeUTC).ToUnixTimeSeconds();
        }
    }
}
