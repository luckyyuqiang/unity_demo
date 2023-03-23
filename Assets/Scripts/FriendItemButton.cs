using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FriendItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform friendSplitButtonPrefab;

    private Image friendItemImage;
    private Transform avatarImage;
    private Transform nameText;
    private Transform contentText;
    private Transform timeText;
    private Transform bubbleButton;
    private Transform toChatImage;
    private Transform friendSplitButtonTransform;

    private HintBubbleButton hintBubble;
    private MyResources myResources;

    private void Awake()
    {
        myResources     = GameObject.Find("Canvas").GetComponent<MyResources>();

        friendItemImage = transform.GetComponent<Image>();
        avatarImage     = transform.GetChild(0).GetChild(0);
        nameText        = transform.GetChild(1).GetChild(0).GetChild(0);
        contentText     = transform.GetChild(1).GetChild(1).GetChild(0);
        timeText        = transform.GetChild(2).GetChild(0);
        bubbleButton    = transform.GetChild(2).GetChild(1);
        toChatImage     = transform.GetChild(2).GetChild(2);
        hintBubble      = bubbleButton.GetComponent<HintBubbleButton>();

        friendSplitButtonTransform = Instantiate(friendSplitButtonPrefab, transform.parent);

        AddButtonClickForAll();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        friendItemImage.color = new Color(friendItemImage.color.r, friendItemImage.color.g, friendItemImage.color.b, 255);
        toChatImage.gameObject.SetActive(true);
        bubbleButton.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        friendItemImage.color = new Color(friendItemImage.color.r, friendItemImage.color.g, friendItemImage.color.b, 0);
        toChatImage.gameObject.SetActive(false);
        hintBubble.ShowHide();
    }

    void SendNoticeForSwitch()
    {
        string name = nameText.GetComponent<TextMeshProUGUI>().text;
        List<string> dst = new List<string> { "Chat", name };
        Notice notice = new Notice("SwitchTo", null, dst);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendNoticeForBubbleChanged()
    {
        int hintNum = hintBubble.HintNum();
        if (hintNum <= 0) return;
        string name = nameText.GetComponent<TextMeshProUGUI>().text;
        List<string> src = new List<string> { "Chat", name };
        Notice notice = new Notice("HintBubbleChanged", src, null, -hintNum);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    // TODO: Using raycast maybe better?
    void AddButtonClickForAll()
    {
        Transform[] all = GetComponentsInChildren<Transform>();
        foreach(var child in all)
        {
            Button btn = child.GetComponent<Button>();
            if (null != btn)
            {
                btn.onClick.AddListener(ButtonClick);
            }
        }
    }

    void ButtonClick()
    {
        SendNoticeForSwitch();
        SendNoticeForBubbleChanged();
    }

    void SetAsFirstSibling()
    {
        friendSplitButtonTransform.SetAsFirstSibling();
        transform.SetAsFirstSibling();
    }

    void ProcessMessage(Notice notice)
    {
        string from = notice.params_[0].ToString();
        string to = notice.params_[1].ToString();
        string content = notice.params_[2].ToString();
        long ts = long.Parse(notice.params_[3].ToString());

        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(from));
        nameText.GetComponent<TextMeshProUGUI>().text = from;
        contentText.GetComponent<TextMeshProUGUI>().text = content;
        timeText.GetComponent<TextMeshProUGUI>().text = Tools.GetMonthAndDayEnAndHHMMFromTs(ts);
        // Bubble will be udpated in HintBubbleChanged
    }

    public void ReceiveAMessage(Notice notice)
    {
        ProcessMessage(notice);
        SetAsFirstSibling();
    }

    public void SendAMessage(Notice notice)
    {
        ProcessMessage(notice);

        string to = notice.params_[1].ToString();
        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(to));
        nameText.GetComponent<TextMeshProUGUI>().text = to;

        SetAsFirstSibling();
    }

    public void LoadAContactWithMsg(Notice notice)
    {
        ProcessMessage(notice);

        string friend = notice.dst_[1];
        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(friend));
        nameText.GetComponent<TextMeshProUGUI>().text = friend;
    }

    public void LoadAContactWithoutMsg(Notice notice)
    {
        string contact = notice.dst_[1].ToString();

        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(contact));
        nameText.GetComponent<TextMeshProUGUI>().text = contact;
        contentText.GetComponent<TextMeshProUGUI>().text = "";
        timeText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void ReceiveContactInvited(Notice notice)
    {
        string from = notice.params_[0].ToString();
        long ts = long.Parse(notice.params_[2].ToString());

        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(from));
        nameText.GetComponent<TextMeshProUGUI>().text = from;
        contentText.GetComponent<TextMeshProUGUI>().text = "/Friend Requtest/";
        contentText.GetComponent<TextMeshProUGUI>().color = new Color(0, 255, 240);
        timeText.GetComponent<TextMeshProUGUI>().text = Tools.GetMonthAndDayEnAndHHMMFromTs(ts);
        // Bubble will be updated in HintBubbleChanged

        SetAsFirstSibling();
    }

    public void HintBubbleChanged(Notice notice)
    {
        int hintNum = int.Parse(notice.params_[0].ToString());
        hintBubble.Add(hintNum);
    }

    public void ReceiveFriendRequestAccepted(Notice notice)
    {
        string from = notice.params_[0].ToString();

        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(from));
        nameText.GetComponent<TextMeshProUGUI>().text = from;
        contentText.GetComponent<TextMeshProUGUI>().text = "";
        timeText.GetComponent<TextMeshProUGUI>().text = "";
    }
}
