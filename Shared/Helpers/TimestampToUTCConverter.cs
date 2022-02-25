namespace Lexplorer.Helpers
{
    public static class TimestampToUTCConverter
    { 
        public static string Convert(string unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(Double.Parse(unixTimeStamp)).ToLocalTime().ToUniversalTime();
            return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }
}
