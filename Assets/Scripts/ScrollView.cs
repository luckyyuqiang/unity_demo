using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{
    public Transform dateSplitButtonPrefab;
    public Transform messageButtonPrefab;
    public Transform messageHintButtonPrefab;
    public Transform applyActionButtonPrefab;

    private Transform scrollviewContent;
    private Scrollbar scrollbar;

    private string currentDate;
    private Context context;

    private List<string> myPath;

    private bool newMessageAdded;

    private void Awake()
    {
        context             = GameObject.Find("Canvas").GetComponent<Context>();
        scrollviewContent   = transform.GetChild(0).GetChild(0);
        scrollbar           = transform.GetChild(1).GetComponent<Scrollbar>();
        currentDate         = "";
        newMessageAdded     = false;
        myPath              = new List<string> { transform.tag };
        Switch.Hide(transform);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("AdjustScrollBar");
    }

    private IEnumerator AdjustScrollBar()
    {
        yield return new WaitForEndOfFrame();

        if (newMessageAdded)
        {
            newMessageAdded = false;

            // Set currentScrollBar.value with Non-Zero
            // Then at next frame, currentScrollBar.value can be successfully to set with zero!
            scrollbar.value = 0.1F;

            yield return new WaitForFixedUpdate();
            scrollbar.value = 0;
        }
    }

    public void SetMyPath(List<string> path)
    {
        myPath = path.ToList();
        if (myPath.Count == 2) Debug.Log($"mypath is : {myPath[0]}, {myPath[1]}");
    }

    public void SwitchTo(Notice notice)
    {
        Switch.SwitchTo(notice.dst_, myPath, transform, myPath.Count);
    }

    public void ReceiveAMessage(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath, myPath.Count)) return;

        long ts = long.Parse(notice.params_[3].ToString());

        AddDateSplitLine(ts);
        AddAMessage(notice);
        SendNoticeForBubbleChanged();
        newMessageAdded = true;
    }

    public void SendAMessage(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath, myPath.Count)) return;

        long ts = long.Parse(notice.params_[3].ToString());

        AddDateSplitLine(ts);
        AddAMessage(notice);
        newMessageAdded = true;
    }

    public void LoadAContactWithMsg(Notice notice)
    {
        // Same behavior with SendAMessage
        SendAMessage(notice);
        newMessageAdded = true;
    }

    public void LoadAContactWithoutMsg(Notice notice)
    {
        // Do nothing
        return;
    }

    public void ReceiveContactInvited(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath, myPath.Count)) return;
        long ts = long.Parse(notice.params_[2].ToString());

        AddDateSplitLine(ts);
        AddInvitation(notice);
        SendNoticeForBubbleChanged();
        newMessageAdded = true;
    }

    public void MessageHint(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath, myPath.Count)) return;

        long ts = long.Parse(notice.params_[1].ToString());
        AddDateSplitLine(ts);
        AddAMessageHint(notice);
        newMessageAdded = true;
    }

    public void ReceiveFriendRequestAccepted(Notice notice)
    {
        if (!Tools.IsSamePath(notice.dst_, myPath, myPath.Count)) return;

        long ts = long.Parse(notice.params_[2].ToString());
        AddDateSplitLine(ts);
        AddAMessageHint(notice);
        newMessageAdded = true;
    }

    void AddAMessage(Notice notice)
    {
        Transform trans = Instantiate(messageButtonPrefab, scrollviewContent);
        trans.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
    }

    void AddDateSplitLine(long ts)
    {
        string date = Tools.GetMonthAndDayEnFromTs(ts);
        if (currentDate.CompareTo(date) == 0) return;

        Transform trans = Instantiate(dateSplitButtonPrefab, scrollviewContent);
        trans.SendMessage("SetDate", ts, SendMessageOptions.DontRequireReceiver);

        currentDate = date;
    }

    void AddAMessageHint(Notice notice)
    {
        Transform trans = Instantiate(messageHintButtonPrefab, scrollviewContent);
        trans.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
    }

    void AddInvitation(Notice notice)
    {
        Transform trans = Instantiate(applyActionButtonPrefab, scrollviewContent);
        trans.SendMessage(notice.func_, notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendNoticeForBubbleChanged()
    {
        // Is background UI, then need broadcast bubble change
        if (context.IsCurrentPath(myPath) == false)
        {
            List<string> src = myPath;
            Notice notice = new Notice("HintBubbleChanged", src, null, 1);
            SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
        }
    }
}
