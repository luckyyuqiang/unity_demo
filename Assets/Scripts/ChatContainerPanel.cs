using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatContainerPanel : MonoBehaviour
{
    public  Transform scrollViewPrefab;

    private Dictionary<string, Transform> name2chat;
    private Transform chatPanelTransform;
    private Transform nameTransform;
    private Transform backImageTransform;
    private Transform currentScrollViewTransform;

    private List<string> myPath;

    private List<Transform> delayDestoryList;

    private void Awake()
    {
        nameTransform = transform.GetChild(0).GetChild(1).transform;
        backImageTransform = transform.GetChild(0).GetChild(0).transform;
        backImageTransform.GetComponent<Button>().onClick.AddListener(ButtonClick);

        chatPanelTransform = transform.GetChild(1);

        name2chat = new Dictionary<string, Transform>();
        myPath = new List<string> { transform.tag };
        delayDestoryList = new List<Transform>();
        Switch.Hide(transform);
    }

    void SetChatName(string name)
    {
        nameTransform.GetComponent<TextMeshProUGUI>().text = name;
    }

    void SendNoticeForSwitch()
    {
        List<string> dst = new List<string> { "Friends" };
        Notice notice = new Notice("SwitchTo", null, dst);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void ButtonClick()
    {
        SendNoticeForSwitch();
    }

    void DelayDestory()
    {
        foreach(var it in delayDestoryList)
        {
            Destroy(it.gameObject);
        }
        delayDestoryList.Clear();
    }

    Transform GetChatScrollView(string name)
    {
        if (!name2chat.ContainsKey(name))
        {
            RectTransform chatPanelRectTransform = chatPanelTransform.GetComponent<RectTransform>();

            // Instantiate a scrollview for a chat
            Transform scrollViewTransform = Instantiate(scrollViewPrefab, chatPanelTransform).transform;
            // Set local position
            scrollViewTransform.localPosition = new Vector3(0, 0, 0);
            // Set size
            scrollViewTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(chatPanelRectTransform.sizeDelta.x, chatPanelRectTransform.sizeDelta.y + 50);
            // Set chat path for created scrollview
            List<string> chatPath = new List<string> { transform.tag, name };
            scrollViewTransform.SendMessage("SetMyPath", chatPath, SendMessageOptions.DontRequireReceiver);

            name2chat.Add(name, scrollViewTransform);
        }

        return name2chat[name];
    }

    void ProcessNotice(Notice notice)
    {
        string chatName;

        if (null != notice.dst_ && notice.dst_.Count >= 2)
        {
            chatName = notice.dst_[1].ToString();
        }
        else if (null != notice.src_ && notice.src_.Count >= 2)
        {
            chatName = notice.src_[1].ToString();
        }
        else
        {
            return;
        }

        if (string.IsNullOrEmpty(chatName)) return;

        Transform scrollViewTransform = GetChatScrollView(chatName);
        scrollViewTransform.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
    }

    public void SendAMessage(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void ReceiveAMessage(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void LoadAContactWithMsg(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void LoadAContactWithoutMsg(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void ReceiveContactInvited(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void MessageHint(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void SwitchTo(Notice notice)
    {
        List<string> dst = notice.dst_;

        if (Tools.IsSamePath(dst, myPath))
        {
            Switch.Show(transform);

            string chatName = dst[1].ToString();
            SetChatName(chatName);

            currentScrollViewTransform = GetChatScrollView(chatName);
            if (null != currentScrollViewTransform)
            {
                currentScrollViewTransform.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (null != currentScrollViewTransform)
            {
                currentScrollViewTransform.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
            }
            Switch.Hide(transform);
            DelayDestory();
        }
    }

    public void ReceiveFriendRequestAccepted(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath)) return;
        ProcessNotice(notice);
    }

    public void DeclineFriend(Notice notice)
    {
        string friend = notice.params_[0].ToString();

        if (!name2chat.ContainsKey(friend)) return;

        Transform scrollviewTrans = name2chat[friend];

        name2chat.Remove(friend);

        currentScrollViewTransform = null;

        delayDestoryList.Add(scrollviewTrans);
    }
}
