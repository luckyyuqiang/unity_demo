using System;
using UnityEngine;

public class Tools
{
    public static void CheckTransform(Transform transform, string name)
    {
        if (null == transform)
        {
            Debug.LogError($"Cannot find transform for {name}");
        }
    }

    public static void CheckGameObject(GameObject obj, string name)
    {
        if (null == obj)
        {
            Debug.LogError($"Cannot find obj for {name}");
        }
    }

    // Text Area return a string which has one more character than real string
    // Using this function to remove the unuseless character.
    public static string GetPureContent(string src)
    {
        int len = src.Length;
        if (1 == len) return "";

        return src.Substring(0, len-1);
    }

    public static string GetAvatar(string userName)
    {
        if (userName.CompareTo("yqtest1") == 0)
        {
            return "2";
        }
        else
        {
            return "1";
        }        
    }

    public static string GetCurrentTime_HHMM()
    {
        return "7:00 AM";
    }

    public static string GetCurrentTime()
    {
        int h = DateTime.Now.Hour; //获取当前时间的小时部分

        int m = DateTime.Now.Minute; //获取当前时间的分钟部分

        int s = DateTime.Now.Second; //获取当前时间的秒部分

        DateTime t = DateTime.Now; //获取当前时间，格式为“年/月/日 星期 时/分/秒”

        string t1 = DateTime.Now.ToString(); //将当前时间转换为字符串

        string t2 = t1.Substring(13, 8); //截取字符串的“时/分/秒”部分

        Console.WriteLine("现在时间是{0}:{1}:{2}", h, m, s);

        Console.WriteLine("现在时间是{0}", t);

        Console.WriteLine("现在时间是{0}", t2);

        return t2;
    }

    public static string GetTimeHHMM(DateTime t)
    {
        int h = t.Hour;
        int m = t.Minute; 

        string aMDesignator = "AM";

        if (h > 12) 
        {
            h = h - 12;
            aMDesignator = "PM";
        }

        string mm = "";
        if (m < 10)
        {
            mm = "0" + m.ToString();
        }
        else
        {
            mm = m.ToString();
        }

        string ftime = h.ToString() + ":" + mm  + " " + aMDesignator;
        return ftime;
    }

    public static DateTime GetTimeFromTS(long ts)
    {
        // TODO: need to implement the logic
        return DateTime.Now;
    }

    public static bool SaveConfigFile(string fn, string content)
    {
        return true;
    }

    public static string ParseConfigFile(string fn)
    {
        return "";
    }

    // Get content from src basing on itemName
    public static string GetItem(string itemName, string src)
    {
        return "";
    }

    // Set itemName=itemContent into dst and return it
    public static string SetItem(string itemName, string itemContent, string dst)
    {
        return "";
    }
}
