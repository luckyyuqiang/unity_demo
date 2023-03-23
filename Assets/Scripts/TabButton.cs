using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Texture2D hoveredCursor;
    public Texture2D defaultCursor;

    private HintBubbleButton hintBubble;
    private Image image;
    private TextMeshProUGUI text;
    private List<string> myPath;

    private void Awake()
    {
        hintBubble  = transform.GetChild(2).GetComponent<HintBubbleButton>();
        image       = transform.GetComponent<Image>();
        text        = transform.GetChild(0).GetComponent<TextMeshProUGUI>();        
        myPath      = new List<string> { transform.tag };
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    void ButtonClick()
    {
        SendNoticeForSwitch();
        SendNoticeForBubbleChanged();
    }

    void SendNoticeForSwitch()
    {
        List<string> dst = new List<string> { transform.tag };
        Notice notice = new Notice("SwitchTo", null, dst);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendNoticeForBubbleChanged()
    {
        if (transform.tag.CompareTo("Friends") != 0)
        {
            int hintNum = hintBubble.HintNum();

            List<string> src = new List<string> { transform.tag };
            Notice notice = new Notice("HintBubbleChanged", src, null, -hintNum);
            SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
        }
    }

    void Show()
    {
        image.color = new Color(0, 255, 240, 255);
        text.color = Color.black;
        hintBubble.ShowHide();
    }

    void Hide()
    {
        image.color = new Color(0, 0, 0, 0);
        text.color = new Color(0, 255, 240);
        hintBubble.ShowHide();
    }

    bool IsSamePath(List<string> path)
    {
        bool same;

        if (path[0].CompareTo("Chat") == 0 && myPath[0].CompareTo("Friends") == 0)
        {
            same = true;
        }
        else
        {
            same = Tools.IsSamePath(path, myPath);
        }

        return same;
    }

    public void SwitchTo(Notice notice)
    {
        if (IsSamePath(notice.dst_))
        {
            SendNoticeForBubbleChanged();
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void HintBubbleChanged(Notice notice)
    {
        if (IsSamePath(notice.src_))
        {
            string str = string.Join(",", notice.src_);
            Debug.Log($"Tab button, get bubble change from: {str}");

            int num = int.Parse(notice.params_[0].ToString());
            hintBubble.Add(num);
        }
    }
}
