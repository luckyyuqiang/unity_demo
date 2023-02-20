using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReactionData
{
    public string reactionId;
    public string messageId;
    public int hintNum;
    public Transform reaction_Button_Transform;
};

// This class also include codes of Guild and Party
public class WorldUI : MonoBehaviour
{
    // Prefab section
    public RectTransform reaction_Button_Prefab_Transform;
    public RectTransform reaction_Grid_Button_Prefab_Transform;
    public RectTransform send_Message_Hint_Button_Prefab_Transform;
    public RectTransform world_Message_Button_Prefab_Transform;

    // Data section
    private Dictionary<Transform, string> dictOfAvatar2UserName;  // Avatar_Button -> UserName
    private Dictionary<Transform, string> dictOfContent2MessageId; // Name_Content_Button -> MessageId
    private Dictionary<Channels, int> dictOfChannel2BubbleNum; // Channel -> Bubble hint num
    private Dictionary<string, Transform> dictOfMessageId2ReactionGrid; // MessageId -> Reaction_Grid_Button
    private Dictionary<string, ReactionData> dictOfReactionId2ReactionData; // ReactionId -> ReactionData
    private Dictionary<Transform, ReactionData> dictOfReactionObj2ReactionData; // Reaction_Button -> ReactionData

    // UI elements section
    // Tabs
    private Transform world_Button_Transform;
    private Transform guild_Button_Transform;
    private Transform party_Button_Transform;

    // Scroll views, contents and other
    private Transform world_Scroll_View_Transform;
    private Transform guild_Scroll_View_Transform;
    private Transform party_Scroll_View_Transform;
    private Transform world_Scroll_View_Content_Transform;
    private Transform guild_Scroll_View_Content_Transform;
    private Transform party_Scroll_View_Content_Transform;

    private Transform add_Button_Transform;

    // Bubbles
    private Transform world_Bubble_Transform;
    private Transform guild_Bubble_Transform;
    private Transform party_Bubble_Transform;

    private Button add_Button;

    // Script objects
    private ChannelController cc;
    private FriendsUI fsUI;
    private FriendUI fUI;
    private SendUI sendUI;
    private Sdk sdkHandle;

    // Private variables
    private Transform currentTabTransform;
    private Transform currentScrollViewTransform;
    private Transform currentScrollViewContentTransform;
    private Scrollbar currentScrollBar;
    private bool newMessageAdded;
    private string selectedUser;
    private string selectedMsgId;

    private void Awake()
    {
        InitUIItems();
        InitScriptHandles();
        InitData();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine("AdjustScrollBar");
    }

    private void InitUIItems()
    {
        world_Button_Transform = transform.Find("Main_Panel/Tab_Panel/World_Button").transform;
        guild_Button_Transform = transform.Find("Main_Panel/Tab_Panel/Guild_Button").transform;
        party_Button_Transform = transform.Find("Main_Panel/Tab_Panel/Party_Button").transform;

        world_Bubble_Transform = transform.Find("Main_Panel/Tab_Panel/World_Button").GetChild(1).transform;
        guild_Bubble_Transform = transform.Find("Main_Panel/Tab_Panel/Guild_Button").GetChild(1).transform;
        party_Bubble_Transform = transform.Find("Main_Panel/Tab_Panel/Party_Button").GetChild(1).transform;

        add_Button_Transform = transform.Find("Main_Panel/Add_Button").transform;

        world_Scroll_View_Transform = transform.Find("Main_Panel/World_Scroll_View").transform;
        world_Scroll_View_Content_Transform = transform.Find("Main_Panel/World_Scroll_View/Viewport/Content").transform;

        guild_Scroll_View_Transform = transform.Find("Main_Panel/Guild_Scroll_View").transform;
        guild_Scroll_View_Content_Transform = transform.Find("Main_Panel/Guild_Scroll_View/Viewport/Content").transform;

        party_Scroll_View_Transform = transform.Find("Main_Panel/Party_Scroll_View").transform;
        party_Scroll_View_Content_Transform = transform.Find("Main_Panel/Party_Scroll_View/Viewport/Content").transform;

        Tools.CheckTransform(world_Button_Transform, "World_Button");
        Tools.CheckTransform(guild_Button_Transform, "Guild_Button");
        Tools.CheckTransform(party_Button_Transform, "Party_Button");
        Tools.CheckTransform(world_Bubble_Transform, "World_Bubble_Button");
        Tools.CheckTransform(guild_Bubble_Transform, "Guild_Bubble_Button");
        Tools.CheckTransform(party_Bubble_Transform, "Party_Bubble_Button");
        Tools.CheckTransform(add_Button_Transform, "Add_Button");
        Tools.CheckTransform(world_Scroll_View_Transform, "World_Scroll_View");
        Tools.CheckTransform(world_Scroll_View_Content_Transform, "World_Scroll_View_Content");
        Tools.CheckTransform(guild_Scroll_View_Transform, "Guild_Scroll_View");
        Tools.CheckTransform(guild_Scroll_View_Content_Transform, "Guild_Scroll_View_Content");
        Tools.CheckTransform(party_Scroll_View_Transform, "Party_Scroll");
        Tools.CheckTransform(party_Scroll_View_Content_Transform, "Party_Scroll_View_Content");

        // Add click action for tab button
        world_Button_Transform.GetComponent<Button>().onClick.AddListener(WorldButtonAction);
        guild_Button_Transform.GetComponent<Button>().onClick.AddListener(GuildButtonAction);
        party_Button_Transform.GetComponent<Button>().onClick.AddListener(PartyButtonAction);

        // disable all bubble buttons
        world_Bubble_Transform.gameObject.SetActive(false);
        guild_Bubble_Transform.gameObject.SetActive(false);
        party_Bubble_Transform.gameObject.SetActive(false);

        add_Button = add_Button_Transform.GetComponent<Button>();

        Button friend_Add_Button = add_Button_Transform.GetChild(1).GetComponent<Button>();
        friend_Add_Button.onClick.AddListener(FriendAddButtonAction);


        InitInternalVariables();
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

        fUI = GameObject.Find("Canvas").GetComponent<FriendUI>();
        if (null == fsUI)
        {
            Debug.LogError("Cannot find FriendUI object.");
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
        dictOfAvatar2UserName = new Dictionary<Transform, string>();
        dictOfContent2MessageId = new Dictionary<Transform, string>();
        dictOfChannel2BubbleNum = new Dictionary<Channels, int>();
        dictOfMessageId2ReactionGrid = new Dictionary<string, Transform>();
        dictOfReactionId2ReactionData = new Dictionary<string, ReactionData>();
        dictOfReactionObj2ReactionData = new Dictionary<Transform, ReactionData>();

        dictOfChannel2BubbleNum[Channels.World] = 0;
        dictOfChannel2BubbleNum[Channels.Guild] = 0;
        dictOfChannel2BubbleNum[Channels.Party] = 0;
    }

    private void InitInternalVariables()
    {
        // Default panel
        currentTabTransform = world_Button_Transform;
        currentScrollViewTransform = world_Scroll_View_Transform;
        currentScrollViewContentTransform = world_Scroll_View_Content_Transform;
        currentScrollBar = currentScrollViewTransform.GetChild(1).GetComponent<Scrollbar>();

        newMessageAdded = false;
        selectedUser = "";
        selectedMsgId = "";
    }

    private void SetCurrentUIHandles(Channels channel)
    {
        if (Channels.World == channel)
        {
            currentTabTransform = world_Button_Transform;
            currentScrollViewTransform = world_Scroll_View_Transform;
            currentScrollViewContentTransform = world_Scroll_View_Content_Transform;
        } 
        else if (Channels.Guild == channel)
        {
            currentTabTransform = guild_Button_Transform;
            currentScrollViewTransform = guild_Scroll_View_Transform;
            currentScrollViewContentTransform = guild_Scroll_View_Content_Transform;
        }
        else if (Channels.Party == channel)
        {
            currentTabTransform = party_Button_Transform;
            currentScrollViewTransform = party_Scroll_View_Transform;
            currentScrollViewContentTransform = party_Scroll_View_Content_Transform;
        }
        else
        {
            currentTabTransform = null;
            currentScrollViewTransform = null;
            currentScrollViewContentTransform = null;
        }
        if(null != currentScrollViewTransform)
        {
            currentScrollBar = currentScrollViewTransform.GetChild(1).GetComponent<Scrollbar>();
        }
        else
        {
            currentScrollBar = null;
        }
    }

    private void TabButtonAction(Channels channel)
    {
        // Disable last tab
        if (Channels.Friends == cc.currentChannel)
        {
            fsUI.DisableTab(cc.currentChannel);
        }
        else if (Channels.Friend == cc.currentChannel)
        {
            fUI.DisableFriendPanel(cc.currentChannel);

            // Switch from Friend_Panel to World_Panel also need to disable Friends_Tab
            fsUI.DisableTab(cc.currentChannel);
        }
        else
        {
            DisableTab(cc.currentChannel);
        }

        cc.currentChannel = channel;

        // Enable current tab
        EnableTab(cc.currentChannel);
    }

    private void WorldButtonAction()
    {
        TabButtonAction(Channels.World);
    }

    private void GuildButtonAction()
    {
        TabButtonAction(Channels.Guild);
    }

    private void PartyButtonAction()
    {
        TabButtonAction(Channels.Party);
    }

    private void Reaction_Add_Button_Click()
    {
        // pop reaction ui
    }

    private IEnumerator AdjustScrollBar()
    {
        yield return new WaitForEndOfFrame();
        
        if (newMessageAdded)
        {
            newMessageAdded = false;

            // Set currentScrollBar.value with Non-Zero
            // Then at next frame, currentScrollBar.value can be successfully to set with zero!
            currentScrollBar.value = 0.1F;

            yield return new WaitForFixedUpdate();
            currentScrollBar.value = 0;
        }
    }

    private void AvatarImageAction()
    {
        Transform trans = EventSystem.current.currentSelectedGameObject.transform;
        selectedUser = dictOfAvatar2UserName[trans];

        // selected user is current user ,then no need to pop button of Friend_Add_Button
        if (Sdk.CurrentUserName().CompareTo(selectedUser) == 0) return;

        add_Button_Transform.gameObject.SetActive(true);
        Debug.Log($"selected user is: {selectedUser}");
    }

    private void MessageContentButtonAction()
    {
        Debug.Log("World message text is clicked.");
    }


    private void Add_Reaction_Button()
    {
        // Add an empty reaction placeholder in panel
        // 
    }

    private void FriendAddButtonAction()
    {
        sdkHandle.AddFriend(selectedUser);
        add_Button_Transform.gameObject.SetActive(false);
    }

    public void DisableTab(Channels channel)
    {       
        // Tab
        TextMeshProUGUI text = currentTabTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.color = Color.black;

        // Scroll view
        currentScrollViewTransform.gameObject.SetActive(false);

        SetCurrentUIHandles(channel);
    }

    public void EnableTab(Channels channel)
    {
        // Must set current UI handles first
        SetCurrentUIHandles(channel);

        // Tab
        TextMeshProUGUI text = currentTabTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.color = Color.white;

        // Scroll view
        currentScrollViewTransform.gameObject.SetActive(true);

        // Update bubble hint
        dictOfChannel2BubbleNum[channel] = 0;
        UpdateHintBubble();

        // Enable SendUI
        sendUI.EnableSendUI();
    }
    

    public void AddMessageToUI(string msgId, string sender, string content, DateTime time)
    {
        // -------------------------------------------------------------------------------
        // Add an UI message
        Transform aMessageTransform = Instantiate(world_Message_Button_Prefab_Transform).transform;

        // Using userName to get avatar
        string avatar = Tools.GetAvatar(sender);
        avatar = "Avatar/" + avatar;
        Sprite sprite = Resources.Load(avatar, typeof(Sprite)) as Sprite;

        // Get current time
        string currentTime = Tools.GetTimeHHMM(time);

        Transform timeTextTransform = aMessageTransform.GetChild(0).transform;

        // Set time in UI message
        TextMeshProUGUI timeText = timeTextTransform.GetComponent<TextMeshProUGUI>();
        timeText.text = currentTime;

        Transform avatarImageTransform = aMessageTransform.GetChild(1).GetChild(0).transform;

        // Set avatar in UI message
        Image image = avatarImageTransform.GetComponent<Image>();        
        image.sprite = sprite;

        // Set avatar image click action
        Button imageButton = avatarImageTransform.gameObject.AddComponent<Button>();
        imageButton.onClick.AddListener(AvatarImageAction);

        Transform nameContentTransform = aMessageTransform.GetChild(2).transform;

        // Set content in UI message
        TextMeshProUGUI contentText = nameContentTransform.GetComponent<TextMeshProUGUI>();
        contentText.text = sender + ": " + content;

        // Set message content button click action
        Button contentButton = nameContentTransform.GetComponent<Button>();
        contentButton.onClick.AddListener(MessageContentButtonAction);

        // Add the new UI message into current scroll view
        aMessageTransform.SetParent(currentScrollViewContentTransform);

        // -------------------------------------------------------------------------------
        // Add a Reaction_Grid_Button for occupy the position in scroll view
        Transform reactionGridTransform = Instantiate(reaction_Grid_Button_Prefab_Transform);

        // Hide the occupying reaction button
        GameObject reactionButton = reactionGridTransform.GetChild(0).gameObject;
        reactionButton.SetActive(false);

        // Add the new occupying Reaction_Grid_Button in to current scroll view
        reactionGridTransform.SetParent(currentScrollViewContentTransform);

        newMessageAdded = true;

        // -------------------------------------------------------------------------------
        // Save internal data
        dictOfAvatar2UserName.Add(avatarImageTransform, sender);
        dictOfContent2MessageId.Add(nameContentTransform, msgId);
        dictOfMessageId2ReactionGrid.Add(msgId, reactionGridTransform);  
    }

    public void AddHintToUI(string content)
    {
        Transform hintTransform = Instantiate(send_Message_Hint_Button_Prefab_Transform).transform;
        TextMeshProUGUI text = hintTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = content;

        hintTransform.SetParent(currentScrollViewContentTransform);
    }

    public void ProcessRecvingMessageOnUI(Channels targetChannel, string msgId, string sender, string content)
    {
        DateTime t = DateTime.Now;

        // Add the related message in corresponding panel
        AddMessageToTargetUI(targetChannel, msgId, sender, content, t);

        // Update hint bubble num in internal data
        if (cc.currentChannel != targetChannel)
        {
            dictOfChannel2BubbleNum[targetChannel]++;
        }

        // Update hint bubble on UI
        UpdateHintBubble();
    }

    private void AddMessageToTargetUI(Channels targetChannel, string msgId,  string sender, string content, DateTime time)
    {
        Transform saveTransform = currentScrollViewContentTransform;
        if (Channels.World == targetChannel)
        {
            currentScrollViewContentTransform = world_Scroll_View_Content_Transform;
        }
        else if (Channels.Guild == targetChannel)
        {
            currentScrollViewContentTransform = guild_Scroll_View_Content_Transform;
        }
        else if (Channels.Party == targetChannel)
        {
            currentScrollViewContentTransform = party_Scroll_View_Content_Transform;
        }
        else
        {
            Debug.LogError($"Wrong target channel:{targetChannel} for  AddMessageToTargetUI.");
        }
        
        AddMessageToUI(msgId, sender, content, time);
        currentScrollViewContentTransform = saveTransform;
    }

    public void UpdateHintBubble()
    {
        if (dictOfChannel2BubbleNum[Channels.World] > 0)
        {
            TextMeshProUGUI text = world_Bubble_Transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = dictOfChannel2BubbleNum[Channels.World].ToString();
            world_Bubble_Transform.gameObject.SetActive(true);
        }
        else
        {
            world_Bubble_Transform.gameObject.SetActive(false);
        }

        if (dictOfChannel2BubbleNum[Channels.Guild] > 0)
        {
            TextMeshProUGUI text = guild_Bubble_Transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = dictOfChannel2BubbleNum[Channels.Guild].ToString();
            guild_Bubble_Transform.gameObject.SetActive(true);
        }
        else
        {
            guild_Bubble_Transform.gameObject.SetActive(false);
        }

        if (dictOfChannel2BubbleNum[Channels.Party] > 0)
        {
            TextMeshProUGUI text = party_Bubble_Transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = dictOfChannel2BubbleNum[Channels.Party].ToString();
            party_Bubble_Transform.gameObject.SetActive(true);
        }
        else
        {
            party_Bubble_Transform.gameObject.SetActive(false);
        }
    }

    private void UpdateData(string msgId, string sender, string content)
    {

    }
}
