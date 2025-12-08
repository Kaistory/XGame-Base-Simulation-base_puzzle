using System;
using System.Collections.Generic;

public class GameUtil : master.Singleton<GameUtil>
{
    public static string LongTimeSecondToUnixTime(long unixTimeSeconds, string day = "D", string hour = "H",
        string minutes = "M", string second = "S")
    {
        TimeSpan dateTime = TimeSpan.FromSeconds(unixTimeSeconds);
        return TimeSpanToUnixTime(dateTime, day, hour, minutes, second);
    }

    public static string TimeSpanToUnixTime(TimeSpan dateTime, string day = "D", string hour = "H", string minutes = "M", string second = "S")
    {
        string strValue = "";
        if (dateTime.Days > 0)
        {
            if (dateTime.Hours == 0)
            {
                strValue = $"{dateTime.Days}{day}";
            }
            else
            {
                strValue = $"{dateTime.Days}{day} : {dateTime.Hours}{hour}";
            }
        }

        else if (dateTime.Hours > 0)
        {
            if (dateTime.Minutes == 0)
            {
                strValue = $"{dateTime.Hours}{hour}";
            }
            else
            {
                strValue = $"{dateTime.Hours}{hour} : {dateTime.Minutes}{minutes}";
            }
        }
        //return $"{dateTime.Hours}H : {dateTime.Minutes}M";
        else
        {
            if (string.IsNullOrEmpty(second))
            {
                strValue = $"{dateTime.Minutes:00} : {dateTime.Seconds:00}";
            }
            else
            {
                strValue = $"{dateTime.Minutes}{minutes} : {dateTime.Seconds}{second}";
            }
            //return $"{dateTime.Minutes}M : {dateTime.Seconds}S";
        }

        return strValue;
        //StringBuilder strbuilder = new StringBuilder();
        //if (dateTime.Year > 0)
        //{
        //    strbuilder.Append($"{dateTime.Year}Y");
        //}
        //if(dateTime.Month > 0)
        //{
        //    strbuilder.Append($"{dateTime.Month}M");
        //}
        //if (dateTime.Day > 0)
        //{
        //    strbuilder.Append($"{dateTime.Day}D");
        //}
        //if (dateTime.Hour > 0)
        //{
        //    strbuilder.Append($"{dateTime.Hour}H");
        //}
        //if (dateTime.Minute > 0)
        //{
        //    strbuilder.Append($"{dateTime.Minute}M");
        //}
        //if ((dateTime.Second > 0) && (dateTime.Hour <= 0))
        //{
        //    strbuilder.Append($"{dateTime.Second}S");
        //}
        //return strbuilder.ToString();
    }

    public static void Log(string debug)
    {
#if DEBUG_DA
        Debug.Log($"DEBUG_DA: {debug}");
#endif
    }

    public static void LogError(string debug)
    {
#if DEBUG_DA
        Debug.LogError($"DEBUG_DA: {debug}");
#endif
    }
}


public static class HelperClass
{
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}