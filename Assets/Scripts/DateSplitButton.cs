using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateSplitButton : MonoBehaviour
{
    private Transform dateText;

    private void Awake()
    {
        dateText = transform.GetChild(0);
    }

    public void SetDate(long ts)
    {
        string mmdd = Tools.GetMonthAndDayEnFromTs(ts);
        dateText.GetComponent<TextMeshProUGUI>().text = mmdd;
    }
}
