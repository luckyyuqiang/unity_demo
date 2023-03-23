using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsScrollView : MonoBehaviour
{
    public Transform friendItemButtonPrefab;

    private Context context;
    private Transform scrollviewContent;

    private Dictionary<string, Transform> name2frienditem;
    private List<string> myPath;

    private void Awake()
    {
        context = GameObject.Find("Canvas").GetComponent<Context>();
        scrollviewContent = transform.GetChild(0).GetChild(0);
        name2frienditem = new Dictionary<string, Transform>();
        myPath = new List<string> { transform.tag };
        Switch.Hide(transform);
    }

    Transform GetFriendItem(string name)
    {
        if (!name2frienditem.ContainsKey(name))
        {
            Transform friendItemTransform = Instantiate(friendItemButtonPrefab, scrollviewContent).transform;
            name2frienditem.Add(name, friendItemTransform);
        }

        return name2frienditem[name];
    }

    bool IsSamePath(List<string> path)
    {
        bool same;
        if (path[0].CompareTo("Chat") == 0)
        {
            same = true;
        }
        else
        {
            same = Tools.IsSamePath(path, myPath);
        }
        return same;
    }

    void ProcessNotice(Notice notice)
    {
        string friendName;

        if (null != notice.dst_ && notice.dst_.Count >= 2)
        {
            friendName = notice.dst_[1].ToString();
        }
        else if (null != notice.src_ && notice.src_.Count >= 2)
        {
            friendName = notice.src_[1].ToString();
        }
        else
        {
            Debug.Log($"Cannot find path in notice:{notice.func_}");
            return;
        }

        Transform friendItemTransform = GetFriendItem(friendName);
        friendItemTransform.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
    }

    public void ReceiveAMessage(Notice notice)
    {
        if (!IsSamePath(notice.dst_)) return;
        ProcessNotice(notice);
    }

    public void SendAMessage(Notice notice)
    {
        if(!IsSamePath(notice.dst_)) return;
        ProcessNotice(notice);
    }

    public void LoadAContactWithMsg(Notice notice)
    {
        if (!IsSamePath(notice.dst_)) return;
        ProcessNotice(notice);
    }

    public void LoadAContactWithoutMsg(Notice notice)
    {
        if (!IsSamePath(notice.dst_)) return;
        ProcessNotice(notice);
    }

    public void ReceiveContactInvited(Notice notice)
    {
        if (!IsSamePath(notice.dst_)) return;
        ProcessNotice(notice);
    }

    public void HintBubbleChanged(Notice notice)
    {
        if (!IsSamePath(notice.src_)) return;

        string str = string.Join(",", notice.src_);
        Debug.Log($"FriendScrollview, get bubble change from: {str}");

        ProcessNotice(notice);
    }

    public void SwitchTo(Notice notice)
    {
        Switch.SwitchTo(notice.dst_, myPath, transform);
    }

    public void DeclineFriend(Notice notice)
    {
        string friend = notice.params_[0].ToString();

        Transform trans = name2frienditem[friend];

        if (null == trans) return;

        trans.gameObject.SetActive(false);

        name2frienditem.Remove(friend);

        Destroy(trans.gameObject);
    }

    public void ReceiveFriendRequestAccepted(Notice notice)
    {
        if (!IsSamePath(notice.dst_)) return;
        ProcessNotice(notice);
    }
}
