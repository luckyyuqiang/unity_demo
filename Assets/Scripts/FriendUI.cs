using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FriendMessageData
{
    public Transform messageTranform;
    public DateTime time;
}

public class FriendUI : MonoBehaviour
{
    // Prefab section
    public RectTransform A_Friend_Panel_Prefab_Transform;
    public RectTransform Friend_Action_Button_Prefab_Transform;
    public RectTransform Friend_Hint_Button_Prefab_Transform;
    public RectTransform Friend_Message_Button_Prefab_Transform;
    public RectTransform Send_Message_Hint_Button_Prefab_Transform;

    // Data section
    private Dictionary<string, Transform> dictOfUserName2FriendPanel;  // UserName -> A_Friend_Panel
    private Dictionary<string, List<FriendMessageData>> dictOfUserName2MessageList; // UserName -> Message List

    // UI element
    private Transform friend_Container_Panel_Transform;

    // Script objects
    private ChannelController cc;
    private FriendsUI fsUI;
    private SendUI sendUI;
    private Sdk sdkHandle;

    // Private variables
    private string currentFriendName;
    private Transform currentFriendPanelTransform;
    private List<FriendMessageData> currentMessageList;
    private FriendData currentFriendData;
    private Scrollbar currentScrollBar;
    private bool newMessageAdded;

    private void Awake()
    {
        InitUIItems();
        InitScriptHandles();
        InitData();
        InitInternalVariables();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine("AdjustAFriendScrollBar");
    }

    private void InitUIItems()
    {
        friend_Container_Panel_Transform = transform.Find("Main_Panel/Friend_Container_Panel").transform;
        Tools.CheckTransform(friend_Container_Panel_Transform, "Friend_Container_Panel");
    }

    private void InitScriptHandles()
    {
        cc = GameObject.Find("Canvas").GetComponent<ChannelController>();
        if (null == cc)
        {
            Debug.LogError("Cannot find ChannelController object.");
        }

        fsUI = GameObject.Find("Canvas").GetComponent<FriendsUI>();
        if (null == fsUI)
        {
            Debug.LogError("Cannot find FriendsUI object.");
        }

        sendUI = GameObject.Find("Canvas").GetComponent<SendUI>();
        if (null == sendUI)
        {
            Debug.LogError("Cannot find SendUI object.");
        }

        sdkHandle = GameObject.Find("Canvas").GetComponent<Sdk>();
        if (null == sdkHandle)
        {
            Debug.LogError("Cannot find Sdk object.");
        }
    }

    private void InitData()
    {
        dictOfUserName2FriendPanel = new Dictionary<string, Transform>();
        dictOfUserName2MessageList = new Dictionary<string, List<FriendMessageData>>();
    }

    private void InitInternalVariables()
    {
        currentFriendName = "";
        currentFriendPanelTransform = null;
        currentMessageList = null;
        currentFriendData = null;
        currentScrollBar = null;
        newMessageAdded = false;
    }

    private void BackButtonAction()
    {
        // Disable current Friend panel
        DisableFriendPanel(cc.currentChannel);

        // Rmove A_Friend_Panel if the user didn't passed application
        if (FriendType.NotPassedApplication == currentFriendData.friendType)
        {
            DelAFriendPanel(currentFriendName);
        }
        
        fsUI.EnableFriendsPanel();
    }

    private void DelAFriendPanel(string userName)
    {
        if (dictOfUserName2FriendPanel.ContainsKey(userName) == true)
        {
            dictOfUserName2FriendPanel.Remove(userName);
        }

        if (dictOfUserName2MessageList.ContainsKey(userName) == true)
        {
            dictOfUserName2MessageList.Remove(userName);
        }
    }

    public string GetCurrentFriendName()
    {
        return currentFriendName;
    }

    public void EnableFriendPanel(string userName, FriendData fData)
    {
        cc.currentChannel = Channels.Friend;
        currentFriendName = userName;
        currentFriendData = fData;

        // Show Friend_Container_Panel
        friend_Container_Panel_Transform.gameObject.SetActive(true);

        // Look for A_Friend_Panel basing on userName
        if (dictOfUserName2FriendPanel.ContainsKey(userName) != true)
        {
            Debug.LogError($"Cannot find FriendPanel for user:{userName}");
            return;
        }

        currentFriendPanelTransform = dictOfUserName2FriendPanel[userName];
        currentMessageList = dictOfUserName2MessageList[userName];

        currentFriendPanelTransform.gameObject.SetActive(true);
        currentScrollBar = currentFriendPanelTransform.GetChild(2).GetChild(1).GetComponent<Scrollbar>();

        // Enable SendUI
        sendUI.EnableSendUI();
    }

    public void DisableFriendPanel(Channels channel)
    {
        //currentFriendData = null;
        //currentScrollBar = null;

        // Disable Friend container panel
        friend_Container_Panel_Transform.gameObject.SetActive(false);

        // Disable current Friend panel
        if (null != currentFriendPanelTransform) currentFriendPanelTransform.gameObject.SetActive(false);
    }

    public void NewAFriendPanel(string friendName, FriendData fData)
    {
        // Create a new A_Friend_Panel
        Transform aFriendPanelTransform = Instantiate(A_Friend_Panel_Prefab_Transform).transform;

        // Save A_Friend_Panel into data
        dictOfUserName2FriendPanel.Add(friendName, aFriendPanelTransform);

        // New message list
        List<FriendMessageData> list = new List<FriendMessageData>();
        dictOfUserName2MessageList.Add(friendName, list);

        // Set user name to Friend_Name_Text in A_Friend_Panel
        Transform friend_Name_Text_Transform = aFriendPanelTransform.GetChild(1).transform;
        friend_Name_Text_Transform.GetComponent<TextMeshProUGUI>().text = friendName;

        // Set click action for back button
        Button backButton = aFriendPanelTransform.GetChild(0).GetComponent<Button>();
        backButton.onClick.AddListener(BackButtonAction);

        // Add latest message to A_Friend_Panel
        if(Sdk.CurrentUserName().CompareTo(fData.sender) == 0) // Last message is a sent message from current user
        {
            AddSendingMessageToUI(fData.sender, fData.receiver, fData.lastMessageContent, fData.lastMessageContentType, fData.lastTime);
        }
        else // Last message is received message from others
        {
            AddRecvingMessageToUI(fData.sender, fData.receiver, fData.lastMessageContent, fData.lastMessageContentType, fData.lastTime);
        }

        // Add A_Friend_Panel to UI
        aFriendPanelTransform.SetParent(friend_Container_Panel_Transform);
        aFriendPanelTransform.gameObject.SetActive(false);

        // Just new A_Friend_Panel, no need to show Friend_Container_Panel
        friend_Container_Panel_Transform.gameObject.SetActive(false);
    }

    public void AddSendingMessageToUI(string sender, string receiver, string content, ContentType contentType, DateTime time)
    {
        AddMessageToUI(receiver, sender, content, contentType, time);
    }

    public void AddRecvingMessageToUI(string sender, string receiver, string content, ContentType contentType, DateTime time)
    {
        AddMessageToUI(sender, sender, content, contentType, time);
    }

    private void AddMessageToUI(string userName, string avatarName,string content, ContentType contentType, DateTime time)
    {
        if (content.Length == 0) return;

        if (dictOfUserName2FriendPanel.ContainsKey(userName) != true)
        {
            Debug.Log($"Cannot find the Friend_Panel basing on userName:{userName}");
            return;
        }

        Transform targetFriendPanelTransform = dictOfUserName2FriendPanel[userName];
        List<FriendMessageData> targetMsgList = dictOfUserName2MessageList[userName];

        // Get content transform: A_Friend_Panel/A_Friend_Scroll_View/Content
        Transform contentTrans = targetFriendPanelTransform.GetChild(2).GetChild(0).GetChild(0).transform;

        // Using userName to get avatar
        string avatar = Tools.GetAvatar(avatarName);
        avatar = "Avatar/" + avatar;
        Sprite sprite = Resources.Load(avatar, typeof(Sprite)) as Sprite;

        if (ContentType.Message == contentType)
        {
            // New Friend_Message_Button instance
            Transform friend_Message_Button_Transform = Instantiate(Friend_Message_Button_Prefab_Transform).transform;

            // Set avatar image
            Image image = friend_Message_Button_Transform.GetChild(0).GetChild(0).GetComponent<Image>();
            image.sprite = sprite;

            // Message content
            TextMeshProUGUI msgText = friend_Message_Button_Transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            msgText.text = content;

            // Set time
            TextMeshProUGUI timeText = friend_Message_Button_Transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            timeText.text = Tools.GetTimeHHMM(time);

            // Add to message to UI
            friend_Message_Button_Transform.SetParent(contentTrans);

            FriendMessageData fmData = new FriendMessageData();
            fmData.messageTranform = friend_Message_Button_Transform;
            fmData.time = time;

            // Save message data into list
            targetMsgList.Add(fmData);

            newMessageAdded = true;

            return;
        }

        if (ContentType.Notice == contentType)
        {
            // New Friend_Action_Button instance
            Transform friend_Action_Button_Transform = Instantiate(Friend_Action_Button_Prefab_Transform).transform;

            // Set avatar
            Image image = friend_Action_Button_Transform.GetChild(0).GetChild(0).GetComponent<Image>();
            image.sprite = sprite;

            // Set click action for buttons
            Button ignoreButton = friend_Action_Button_Transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Button>();
            ignoreButton.onClick.AddListener(IgnoreButtonAction);

            Button acceptButton = friend_Action_Button_Transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Button>();
            acceptButton.onClick.AddListener(AcceptButtonAction);

            // Add Friend_Action_Button instance to UI
            friend_Action_Button_Transform.SetParent(contentTrans);
            friend_Action_Button_Transform.SetAsFirstSibling();

            newMessageAdded = true;

            return;
        }
    }
    
    public void AddHintToUI(string content)
    {
        Transform hintTransform = Instantiate(Friend_Hint_Button_Prefab_Transform).transform;
        TextMeshProUGUI hintText = hintTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
        hintText.text = content;

        // Get content transform: A_Friend_Panel/A_Friend_Scroll_View/Content
        Transform contentTrans = currentFriendPanelTransform.GetChild(2).GetChild(0).GetChild(0).transform;

        hintTransform.SetParent(contentTrans);
        hintTransform.SetAsFirstSibling();

        newMessageAdded = true;
    }

    private void IgnoreButtonAction()
    {
        Transform trans = EventSystem.current.currentSelectedGameObject.transform;
        Transform friendActionButtonTranform = trans.parent.parent.parent;

        sdkHandle.DeclineFriend(currentFriendName);

        currentFriendData.friendType = FriendType.NotPassedApplication;

        friendActionButtonTranform.gameObject.SetActive(false);

        AddHintToUI($"You decline the friend application.");
    }

    private void AcceptButtonAction()
    {
        Transform trans = EventSystem.current.currentSelectedGameObject.transform;
        Transform friendActionButtonTranform = trans.parent.parent.parent;

        sdkHandle.AcceptFriend(currentFriendName);

        currentFriendData.friendType = FriendType.PassedApplication;

        friendActionButtonTranform.gameObject.SetActive(false);

        AddHintToUI($"You both are friends now.");
    }

    private IEnumerator AdjustAFriendScrollBar()
    {
        yield return new WaitForEndOfFrame();
        if (newMessageAdded)
        {
            newMessageAdded = false;
            if (null != currentScrollBar)
            {
                currentScrollBar.value = 0.1F;
            }

            yield return new WaitForFixedUpdate();
            if (null != currentScrollBar)
            {
                currentScrollBar.value = 0;
            }
        }
    }

    public void ProcessRecvingMessageOnUI(string msgId, string sender, string receiver, string content, DateTime time)
    {
        AddMessageToUI(sender, sender, content, ContentType.Message, time);
    }

    public void ProcessSendingMessageOnUI(string msgId, string sender, string receiver, string content, DateTime time)
    {
        AddSendingMessageToUI(sender, receiver, content, ContentType.Message, time);
    }

    private void UpdateMessageItemsTime()
    {

    }
}
