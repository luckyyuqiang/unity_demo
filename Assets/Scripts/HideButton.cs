using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideButton : MonoBehaviour
{
    public Texture2D hoveredCusor;

    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
        Switch.Hide(transform);
    }

    void ButtonClick()
    {
        List<string> dst = new List<string> { "Icon" };
        Notice notice = new Notice("SwitchTo", null, dst);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
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
