using System;

namespace Assets.Src.Utils
{
    public static class DateTimeHelper
    {

        public static DateTime FromUnixTimestamp(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime;
        }
    }
}