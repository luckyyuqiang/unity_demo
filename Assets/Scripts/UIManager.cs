using System.Collections.Generic;
using UnityEngine;

public class Notice
{
    public string func_;
    public List<string> src_;
    public List<string> dst_;
    public List<object> params_;

    public Notice(string func, List<string> src, List<string> dst, params object[] ps)
    {
        func_   = func;
        src_    = src;
        dst_    = dst;
        params_ = new List<object>(ps);
    }
}

public class UIManager : MonoBehaviour
{

    List<Transform> noticeList;

    private Context context;

    private void Awake()
    {
        context = GameObject.Find("Canvas").GetComponent<Context>();

        noticeList = new List<Transform>();

        Transform trans = GameObject.Find("Canvas/BackGroundPanel/InvokeButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/HideButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/TabsButton/WorldTabButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/TabsButton/GuildTabButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/TabsButton/PartyTabButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/TabsButton/FriendsTabButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/ContainerPanel/WorldScrollView").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/ContainerPanel/GuildScrollView").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/ContainerPanel/PartyScrollView").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/ContainerPanel/FriendsScrollView").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/ContainerPanel/ChatContainerPanel").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/MainPanel/SendContainerButton").transform;
        noticeList.Add(trans);

        trans = GameObject.Find("Canvas/EmojiPanel").transform;
        noticeList.Add(trans);
    }

    private void PreProcessNotice(Notice notice)
    {
        // Save target tag
        if (notice.func_.CompareTo("SwitchTo") == 0)
        {
            context.SavePath(notice.dst_);
        }
    }

    // Broadcast message to notice list
    public void UIManagerNotice(Notice notice)
    {
        PreProcessNotice(notice);

        foreach (var it in noticeList)
        {
            it.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
        }
    }
}
