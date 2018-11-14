using System;

namespace DaOAuthV2.Tools
{
    public static class DateTimeExtension
    {
        public static long NowUtcUnixTimeStamp(this DateTime value)
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
