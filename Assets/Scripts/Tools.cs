using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class Switch
{
    public static void Show(Transform trans, List<string> myPath = null)
    {
        if (null == trans) return;
        trans.localScale = new Vector3(1, 1, 1);

        if (null != myPath)
        {
            string myPathStr = string.Join(",", myPath);
            Debug.Log($" --- {myPathStr} show --- ");
        }
    }

    public static void Hide(Transform trans, List<string> myPath = null)
    {
        if (null == trans) return;
        trans.localScale = new Vector3(0, 1, 0);

        if (null != myPath)
        {
            string myPathStr = string.Join(",", myPath);
            Debug.Log($" --- {myPathStr} Hide --- ");
        }
    }

    public static bool SwitchTo(List<string> dst, List<string> myPath, Transform trans, int pathDeep = 1)
    {
        if (Tools.IsSamePath(dst, myPath, pathDeep))
        {
            Show(trans, myPath);
            return true;
        }
        else
        {
            Hide(trans, myPath);
            return false;
        }
    }
}

public class FileParser
{
    public static Dictionary<string, string> Parse(string fn)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        // Cannot find the file, return directly
        if (!File.Exists(fn)) return dict;

        string[] content = File.ReadAllLines(fn);
        
        foreach (var it in content)
        {
            string left = GetLeftPart(it);
            string right = GetRightPart(it);

            Debug.Log($"{it}; left: {left}; right: {right}");

            if (left.Length > 0 && right.Length > 0)
            {
                dict.Add(left, right);
            }
        }
        return dict;
    }

    public static string GetLeftPart(string line)
    {
        if (string.IsNullOrEmpty(line)) return "";
        var pos = line.IndexOf("=");
        if (pos <= 0) return "";
        return line.Substring(0, pos);
    }

    public static string GetRightPart(string line)
    {
        if (string.IsNullOrEmpty(line)) return "";
        var pos = line.IndexOf("=");
        if (pos <= 0) return "";
        return line.Substring(pos + 1, (line.Length - pos - 1));
    }
}

public class Tools
{
    // Not work for APP on Mac platform
    // GetCurrentDirectory will return directory under /private/var/folders...
    public static string GetCurrentDirectory()
    {
#if UNITY_EDITOR
        return System.Environment.CurrentDirectory;
#else
        return System.AppDomain.CurrentDomain.BaseDirectory;
#endif
    }

    public static string GetCurrentDirectory2(string fn)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fn);
        return path;
    }

    public static bool IsSamePath(List<string> path1, List<string> path2, int pathDeep = 1)
    {
        if (null == path1 || null == path2) return false;

        if (path1.Count < pathDeep || path2.Count < pathDeep) return false;

        for (int i = 0; i < pathDeep; i++)
        {
            if (path1[i] != path2[i])
            {
                return false;
            }
        }

        return true;
    }

    public static string GetAvatarIndex(string userName)
    {
        char[] chars = userName.ToCharArray();
        int sum = 0;
        foreach (var it in chars)
        {
            sum += it;
        }
        sum = sum % 12; // 12 according to avartar number in resource

        if (sum <= 0) sum = 1;

        Debug.Log($"index is: {sum}");

        return sum.ToString();
    }

    public static string GetEmojiUnicode(string emojiName)
    {
        string prefix;
        if (emojiName.Length == 5)
        {
            prefix = "\\U000";
        }
        else if (emojiName.Length == 4)
        {
            prefix = "\\U0000";
        }
        else
        {
            return "";
        }

        string src = prefix + emojiName;
        string dst;

        string str = src.Substring(0, 10).Substring(2);

        byte[] bytes = new byte[4];

        bytes[3] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
        bytes[2] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
        bytes[1] = byte.Parse(int.Parse(str.Substring(4, 2), System.Globalization.NumberStyles.HexNumber).ToString());
        bytes[0] = byte.Parse(int.Parse(str.Substring(6, 2), System.Globalization.NumberStyles.HexNumber).ToString());

        dst = Encoding.UTF32.GetString(bytes);
        return dst;
    }

    public static bool IsDoubleCharEmoji(char codePoint)
    {
        bool ret = (codePoint == 0x0) || (codePoint == 0x9) || (codePoint == 0xA) || (codePoint == 0xD)
                         || ((codePoint >= 0x20) && (codePoint <= 0xD7FF))
                         || ((codePoint >= 0xE000) && (codePoint <= 0xFFFD))
                         || ((codePoint >= 0x10000) && (codePoint <= 0x10FFFF));
        return (!ret);
    }


    public static int GetRealCaretPosition(string str, int position)
    {
        int index = position - 1;

        List<int> doubleChatPosList = new List<int>();

        int count = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (IsDoubleCharEmoji(str[i]))
            {
                count++;
            }
            else
            {
                continue;
            }

            if (1 == count)
            {
                doubleChatPosList.Add(i - doubleChatPosList.Count);
                continue;
            }
            else if (2 == count)
            {
                count = 0;
                continue;
            }
        }

        int incrementals = 0;
        foreach (var it in doubleChatPosList)
        {
            if (index >= it)
            {
                incrementals++;
            }
            else
            {
                break;
            }
        }

        int realPos = index + incrementals + 1;

        if (incrementals > 0)
            return realPos;
        else
            return position;
    }

    public static string GetMonthEn(DateTime t)
    {
        return t.ToString("MMM", CultureInfo.GetCultureInfo("en-US"));
    }

    public static string GetMonthEnFromTs(long ts)
    {
        DateTime dt = ToDateTime(ts);
        return GetMonthEn(dt);
    }

    public static string GetDay(DateTime t)
    {
        return t.ToString("dd");
    }

    public static string GetDayFromTs(long ts)
    {
        DateTime dt = ToDateTime(ts);
        return GetDay(dt);
    }

    public static string GetMonthAndDayEn(DateTime t)
    {
        string mon = GetMonthEn(t);
        string day = GetDay(t);
        return mon + " " + day;
    }

    public static string GetMonthAndDayEnFromTs(long ts)
    {
        DateTime dt = ToDateTime(ts);
        return GetMonthAndDayEn(dt);
    }

    public static string GetMonthAndDayEnAndHHMM(DateTime t)
    {
        string mmdd = GetMonthAndDayEn(t);
        string time = GetTimeHHMM(t);
        return mmdd + "," + time;
    }

    public static string GetMonthAndDayEnAndHHMMFromTs(long ts)
    {
        DateTime dt = ToDateTime(ts);
        return GetMonthAndDayEnAndHHMM(dt);
    }

    public static string GetTimeHHMM(DateTime t)
    {
        return t.ToString("HH:mm");
    }

    public static string GetTimeHHMMFromTs(long ts)
    {
        DateTime dt = ToDateTime(ts);
        return GetTimeHHMM(dt);
    }

    public static DateTime ToDateTime(long ts)
    {
        DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
        return dtStart.AddSeconds(ts);
    }

    public static long ToTimeStamp(DateTime t)
    {
        DateTime t1970 = new DateTime(1970, 1, 1).ToLocalTime();
        return (long)(DateTime.Now.ToLocalTime() - t1970).TotalSeconds;
    }
}
