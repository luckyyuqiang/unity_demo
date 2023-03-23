using System;
using UnityEngine;

public class test : MonoBehaviour
{
    public Transform message;

    private Transform main;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Transform aMessageTransform = Instantiate(message, transform.GetChild(0).GetChild(0).GetChild(0)).transform;
            Hide();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Show();
        }*/
    }

    public static DateTime ToDateTime1(long timestamp)

    {
        string ID = TimeZoneInfo.Local.Id;

        DateTime start = new DateTime(1970, 1, 1) + TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

        DateTime startTime = TimeZoneInfo.ConvertTime(start, TimeZoneInfo.FindSystemTimeZoneById(ID));

        //根据时间戳类型就行转换

        //例：秒级时间戳

        return startTime.AddSeconds(timestamp);

        //return startTime;

    }



    public DateTime ToDateTime2(string timeStamp)
    {

        DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);

        long lTime = long.Parse(timeStamp + "0000000");

        TimeSpan toNow = new TimeSpan(lTime);

        return dtStart.Add(toNow);

    }

    public DateTime ToDateTime3(long ts)
    {

        DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);

        return dtStart.AddSeconds(ts);

    }


    void Awake()
    {
        //main = GameObject.Find("Canvas/MainPanel").transform;
        //Debug.Log($"{DateTime.Today}");
        //Debug.Log($"{DateTime.Today.ToString("MMMM,d")}");
        //Debug.Log($"{DateTime.Today.ToString("MMM", CultureInfo.GetCultureInfo("en-US"))}");
        //Debug.Log($"{DateTime.Now.ToString("dd")}");
        //Debug.Log($"{DateTime.Now.ToString("HH:mm")}");

        DateTime t = ToDateTime1(1678191473);
        Debug.Log($"{t.ToString()}");
        t = ToDateTime2("1678191473");
        Debug.Log($"second: {t.ToString()}");
        t = ToDateTime3(1678191473);
        Debug.Log($"third: {t.ToString()}");
    }

    /*
    private void Show()
    {
        transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        transform.GetChild(3).localScale = new Vector3(1, 0, 0);
        main.localScale = new Vector3(1, 1, 1);
    }

    private void Hide()
    {
        transform.GetChild(0).localScale = new Vector3(1, 0, 0);
        transform.GetChild(3).localScale = new Vector3(1, 1, 1);
        main.localScale = new Vector3(1, 0, 0);
    }
    */
}
