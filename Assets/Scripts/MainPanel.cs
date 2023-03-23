using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainPanel : MonoBehaviour
{
    public Texture2D defaultCursor;

    private void Awake()
    {
        Switch.Hide(transform);
    }

    public void SwitchTo(Notice notice)
    {
        List<string> dst = notice.dst_;

        if (dst[0].CompareTo("Icon") != 0)
        {
            Switch.Show(transform);
        }
        else
        {
            Switch.Hide(transform);
        }
    }
}
