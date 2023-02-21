using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ContentType
{
    Notice = 0,
    Message
}

public enum FriendType
{
    WaitForApplication = 0,
    NotPassedApplication = -1,
    PassedApplication = 1,
}

public class FriendData
{
    public string sender;
    public string receiver;
    public string lastMessageContent;
    public ContentType lastMessageContentType;
    public int hintNum;
    public DateTime lastTime;
    public FriendType friendType; // -1: not passed; 0: wait to pass; 1: passed; 
    public Transform friend_Button;
    public Transform hint_Bubble;
};

public class FriendsUI : MonoBehaviour
{
    // Prefab section
    public RectTransform friend_Button_Prefab_Transform;

    // Data section
    private int friendsBubbleNum;
    private Dictionary<Transform, string> dictOfFriend2UserName; // Friend_Button -> UserName
    private Dictionary<string, FriendData> dictOfUserName2FriendData; // UserName -> FriendData    

    // UI elements
    private Transform friends_Button_Transform;
    private Transform friends_Scroll_View_Transform;
    private Transform friends_Scroll_View_Cotent_Transform;
    private Transform friends_Hint_Bubble_Transform;
    private Scrollbar friendsScrollBar;

    // Script objects
    private ChannelController cc;
    private FriendUI fUI;
    private WorldUI wUI;
    private SendUI sendUI;
    private Sdk sdkHandle;

    // Private variables
    private bool newFriendItemAdded;
    private bool initializedFriendsList;

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
        StartCoroutine("AdjustFriendsScrollBar");
    }

    private void InitUIItems()
    {
        friends_Button_Transform = transform.Find("Main_Panel/Tab_Panel/Friends_Button").transform;
        friends_Scroll_View_Transform = transform.Find("Main_Panel/Friends_Scroll_View").transform;
        friends_Scroll_View_Cotent_Transform = transform.Find("Main_Panel/Friends_Scroll_View/Viewport/Content").transform;
        friends_Hint_Bubble_Transform = friends_Button_Transform.GetChild(1).transform;
        friendsScrollBar = friends_Scroll_View_Transform.GetChild(1).GetComponent<Scrollbar>();

        Tools.CheckTransform(friends_Button_Transform, "Friends_Button");
        Tools.CheckTransform(friends_Scroll_View_Transform, "Friends_Scroll_View");
        Tools.CheckTransform(friends_Scroll_View_Cotent_Transform, "Friends_Scroll_View_Content");

        friends_Button_Transform.GetComponent<Button>().onClick.AddListener(FriendsButtonAction);

        friends_Hint_Bubble_Transform.gameObject.SetActive(false);
    }

    private void InitScriptHandles()
    {
        cc = GameObject.Find("Canvas").GetComponent<ChannelController>();
        if (null == cc)
        {
            Debug.LogError("Cannot find ChannelController object.");
        }

        fUI = GameObject.Find("Canvas").GetComponent<FriendUI>();
        if (null == fUI)
        {
            Debug.LogError("Cannot find FriendUI object.");
        }

        wUI = GameObject.Find("Canvas").GetComponent<WorldUI>();
        if (null == wUI)
        {
            Debug.LogError("Cannot find WorldUI object.");
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
        dictOfFriend2UserName = new Dictionary<Transform, string>();
        dictOfUserName2FriendData = new Dictionary<string, FriendData>();
        friendsBubbleNum = 0;
    }

    private void InitInternalVariables()
    {
        newFriendItemAdded = false;
        initializedFriendsList = false;
    }

    private void FriendsButtonAction()
    {
        TabButtonAction(Channels.Friends);
    }

    private void TabButtonAction(Channels channel)
    {
        // Disable last tab
        if (Channels.World == cc.currentChannel || 
            Channels.Guild == cc.currentChannel || 
            Channels.Party == cc.currentChannel)
        {
            wUI.DisableTab(cc.currentChannel);
        }
        else if (Channels.Friend == cc.currentChannel)
        {
            fUI.DisableFriendPanel(cc.currentChannel);
        }

        cc.currentChannel = channel;

        // Enable current tab
        EnableTab(cc.currentChannel);
    }

    public void LoadAllFriends()
    {
        List<string> list = sdkHandle.GetContactList();

        if (null == list) return;

        foreach(var conv in list)
        {
            sdkHandle.LastMessageFromFriend(conv, out string sender, out string receiver,out string content, out DateTime time);
            AddFriendItemToUI(conv, sender, receiver, content, ContentType.Message, time, false);
        }
    }

    public void DisableTab(Channels channel)
    {
        // Tab
        TextMeshProUGUI text = friends_Button_Transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.color = Color.black;

        // Scroll view
        friends_Scroll_View_Transform.gameObject.SetActive(false);
    }

    public void EnableTab(Channels channel)
    {
        // Initialize friend list first
        if (!initializedFriendsList)
        {
            LoadAllFriends();
            initializedFriendsList = true;
        }

        // Tab
        TextMeshProUGUI text = friends_Button_Transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.color = Color.white;

        // Scroll view
        friends_Scroll_View_Transform.gameObject.SetActive(true);

        // Update all items in Friends panel here
        UpdateAllFriendItems();

        // Disable SendUI
        sendUI.DisableSendUI();
    }

    public void EnableFriendsPanel()
    {
        cc.currentChannel = Channels.Friends;

        // Scroll view
        friends_Scroll_View_Transform.gameObject.SetActive(true);

        // Update all items in Friends panel here
        UpdateAllFriendItems();

        // Disable SendUI
        sendUI.DisableSendUI();
    }

    public void DisableFriendsPanel()
    {
        // Scroll view
        friends_Scroll_View_Transform.gameObject.SetActive(false);
    }

    public void AddFriendItemToUI(string friendName, string sender, string receiver, string content, ContentType contentType, DateTime time, bool newMsg)
    {
        if (dictOfUserName2FriendData.ContainsKey(friendName) == true) return;

        Transform aFriendButtonTransform = Instantiate(friend_Button_Prefab_Transform);
        aFriendButtonTransform.GetComponent<Button>().onClick.AddListener(FriendButtonAction);

        // Using userName to get avatar
        string avatar = Tools.GetAvatar(friendName);
        avatar = "Avatar/" + avatar;
        Sprite sprite = Resources.Load(avatar, typeof(Sprite)) as Sprite;

        // Set avatar
        Transform avatarImageTransform = aFriendButtonTransform.GetChild(0).GetChild(0).transform;
        avatarImageTransform.GetComponent<Image>().sprite = sprite;
        avatarImageTransform.GetComponent<Button>().onClick.AddListener(FriendButtonChildClickAction);

        // Set user name
        Transform name_Button_Transform = aFriendButtonTransform.GetChild(1).GetChild(0).transform;
        name_Button_Transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = friendName;
        name_Button_Transform.GetComponent<Button>().onClick.AddListener(FriendButtonChildClickAction);

        // Set message content
        Transform message_Button_Transform = aFriendButtonTransform.GetChild(1).GetChild(1).transform;
        message_Button_Transform.GetComponent<Button>().onClick.AddListener(FriendButtonChildClickAction);

        Transform message_Text_Transform = message_Button_Transform.GetChild(0).transform;
        message_Text_Transform.GetComponent<TextMeshProUGUI>().text = content;

        // Set message color
        message_Text_Transform.GetComponent<TextMeshProUGUI>().color = Color.white;
        if (ContentType.Notice == contentType)
        {
            message_Text_Transform.GetComponent<TextMeshProUGUI>().color = Color.red;
        }

        // Set time text
        Transform time_Text_Transform = aFriendButtonTransform.GetChild(2).GetChild(1).transform;
        if (content.Length == 0)
        {
            // Content is empty, then no need to set time
            time_Text_Transform.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            time_Text_Transform.GetComponent<TextMeshProUGUI>().text = Tools.GetTimeHHMM(time);
        }

        // Set hint bubble
        Transform hint_Bubble_Transform = aFriendButtonTransform.GetChild(2).GetChild(0).transform;        
        if (content.Length > 0 && newMsg == true) // If message has content and it is also a new message, then set hint
        {
            hint_Bubble_Transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
            hint_Bubble_Transform.gameObject.SetActive(true);
        }
        else // If message has content, then no need to set hint
        {
            hint_Bubble_Transform.gameObject.SetActive(false);
        }

        // Set UI parent
        aFriendButtonTransform.SetParent(friends_Scroll_View_Cotent_Transform);

        // Adjust new item to the top of UI
        aFriendButtonTransform.SetAsFirstSibling();

        // Save internal data
        FriendData fData = new FriendData();

        fData.sender = sender;
        fData.receiver = receiver;
        fData.lastMessageContent = content;
        fData.lastMessageContentType = contentType;

        fData.hintNum = 0;
        if (content.Length > 0 && newMsg == true) fData.hintNum = 1;

        fData.lastTime = time;

        fData.friendType = FriendType.PassedApplication;
        if (ContentType.Notice == contentType) fData.friendType = FriendType.WaitForApplication;

        fData.friend_Button = aFriendButtonTransform;
        fData.hint_Bubble = hint_Bubble_Transform;

        dictOfUserName2FriendData[friendName] = fData;
        dictOfFriend2UserName[aFriendButtonTransform] = friendName;

        // New A_Friend_Panel relate to userName
        fUI.NewAFriendPanel(friendName, fData);

        newFriendItemAdded = true;
    }

    public void UpdateFriendItemToUI(string userName, string content, ContentType contentType, DateTime time)
    {
        if (dictOfUserName2FriendData.ContainsKey(userName) == false) return;

        FriendData fData = dictOfUserName2FriendData[userName];

        Transform aFriendButtonTransform = fData.friend_Button;

        // Set message content
        Transform message_Text_Transform = aFriendButtonTransform.GetChild(1).GetChild(1).GetChild(0).transform;
        message_Text_Transform.GetComponent<TextMeshProUGUI>().text = content;

        // Set message color
        message_Text_Transform.GetComponent<TextMeshProUGUI>().color = Color.white;
        if (ContentType.Notice == contentType && content.Length > 0)
        {
            message_Text_Transform.GetComponent<TextMeshProUGUI>().color = Color.red;
        }

        // Update content for new message
        if (content.Length > 0)
        {
            // Set time text
            Transform time_Text_Transform = aFriendButtonTransform.GetChild(2).GetChild(1).transform;
            time_Text_Transform.GetComponent<TextMeshProUGUI>().text = Tools.GetTimeHHMM(time);

            if (cc.currentChannel == Channels.Friend && fUI.GetCurrentFriendName().CompareTo(userName) == 0)
            {
                // No need to update hint bubble, since the message will be add into current Friend UI
            }
            else
            {
                // Update hint bubble
                fData.hintNum++; // Add 1 for new coming message
                fData.hint_Bubble.GetChild(0).GetComponent<TextMeshProUGUI>().text = fData.hintNum.ToString();
                fData.hint_Bubble.gameObject.SetActive(true);
            }

            // Adjust new item to the top of Friend list
            aFriendButtonTransform.SetAsFirstSibling();
        }
        // Clear content for useless message
        else
        {
            Transform time_Text_Transform = aFriendButtonTransform.GetChild(2).GetChild(1).transform;
            time_Text_Transform.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void FriendButtonAction()
    {
        Transform trans = EventSystem.current.currentSelectedGameObject.transform;
        
        if (dictOfFriend2UserName.ContainsKey(trans) == false)
        {
            Debug.LogError("Cannot find related usename with selectd Friend_Button.");
            return;
        }

        string userName = dictOfFriend2UserName[trans];
        SwtichToAFriendPanel(userName);
    }

    private void FriendButtonChildClickAction()
    {
        Transform trans = EventSystem.current.currentSelectedGameObject.transform;

        if (dictOfFriend2UserName.ContainsKey(trans.parent.parent) == false)
        {
            Debug.LogError("Cannot find related usename with selectd Friend_Button.");
            return;
        }

        string userName = dictOfFriend2UserName[trans.parent.parent];
        SwtichToAFriendPanel(userName);
    }

    private void SwtichToAFriendPanel(string userName)
    {
        FriendData fData = dictOfUserName2FriendData[userName];
        if (null == fData)
        {
            Debug.LogError($"Cannot find related FriendData with usename:{userName}.");
            return;
        }

        // After click the Friend_Button,
        // the hint_bubble on the Friend_Button will be set inactive
        fData.hintNum = 0;
        fData.hint_Bubble.gameObject.SetActive(false);

        // Update hint bubble on the Friends_Button
        UpdateHintBubble();

        DisableFriendsPanel();
        fUI.EnableFriendPanel(userName, fData);
    }

    private IEnumerator AdjustFriendsScrollBar()
    {
        yield return new WaitForEndOfFrame();
        if (newFriendItemAdded)
        {
            newFriendItemAdded = false;
            friendsScrollBar.value = 0.1F;            

            yield return new WaitForFixedUpdate();
            friendsScrollBar.value = 0;
        }
    }

    private void UpdateHintBubble()
    {
        friendsBubbleNum = 0;

        foreach (var it in dictOfUserName2FriendData)
        {
            friendsBubbleNum += it.Value.hintNum;
        }

        if (friendsBubbleNum > 0)
        {
            TextMeshProUGUI hintBubbleText = friends_Hint_Bubble_Transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            hintBubbleText.text = friendsBubbleNum.ToString();
            friends_Hint_Bubble_Transform.gameObject.SetActive(true);
        }
        else
        {
            friends_Hint_Bubble_Transform.gameObject.SetActive(false);
        }
    }

    private void UpdateAllFriendItems()
    {
        List<string> removeList = new List<string>();

        foreach(var it in dictOfUserName2FriendData)
        {
            FriendData fData = it.Value;

            // For the friend which is not passed application, need to be removed from UI
            if (FriendType.NotPassedApplication == fData.friendType)
            {
                fData.friend_Button.gameObject.SetActive(false);
                removeList.Add(it.Key);
            }
            // Clear the Friend Request hint on Friend item UI
            else if (FriendType.PassedApplication == fData.friendType)
            {
                if (ContentType.Notice == fData.lastMessageContentType)
                {
                    UpdateFriendItemToUI(it.Key, "", ContentType.Notice, DateTime.Now);
                }
            }

            // Update friend item time
        }

        // Remove declined friends from internal data
        foreach(var it in removeList)
        {
            DelAFriendItem(it);
        }
    }

    private void DelAFriendItem(string userName)
    {
        if (dictOfUserName2FriendData.ContainsKey(userName) == true)
        {
            FriendData fData = dictOfUserName2FriendData[userName];

            if (dictOfFriend2UserName.ContainsKey(fData.friend_Button) == true)
            {
                dictOfFriend2UserName.Remove(fData.friend_Button);
            }

            dictOfUserName2FriendData.Remove(userName);
        }
    }


    public void ProcessRecvingMessageOnUI(string msgId, string sender, string receiver, string content, DateTime time)
    {
        
        if (dictOfUserName2FriendData.ContainsKey(sender) == false)
        {
            // Cannot find the Friend_Button basing on sender, then create one
            AddFriendItemToUI(sender, sender, receiver, content, ContentType.Message, time, true);
        }
        else
        {
            // Update the FriendItem on Friends UI
            UpdateFriendItemToUI(sender, content, ContentType.Message, time);

            // Add message to Friend UI
            fUI.ProcessRecvingMessageOnUI(msgId, sender, receiver, content, time);
        }

        // Update hint bubble on the Friends_Button
        UpdateHintBubble();
    }

    public void ProcessSendingMessageOnUI(string msgId, string sender, string receiver, string content, DateTime time)
    {
        // Update the FriendItem on Friends UI
        UpdateFriendItemToUI(receiver, content, ContentType.Message, time);

        // Add message to Friend UI
        fUI.ProcessSendingMessageOnUI(msgId, sender, receiver, content, time);
    }

    public void ProcessRecvInvitationOnUI(string sender, string receiver, string reason, DateTime time)
    {

        if (dictOfUserName2FriendData.ContainsKey(sender) == false)
        {
            // Cannot find the Friend_Button basing on sender, then create one
            AddFriendItemToUI(sender, sender, receiver, "[Friend Request]", ContentType.Notice, time, true);

            // Update hint bubble on the Friends_Button
            UpdateHintBubble();
        }
    }
}
