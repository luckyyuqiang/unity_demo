using System.Collections.Generic;
using UnityEngine;
using AgoraChat;
using AgoraChat.MessageBody;
using System;

public class UIHandle
{
    // Private object script
    private SendUI sendUI;
    private FriendsUI fsUI;
    private FriendUI fUI;
    private WorldUI wUI;
    private ChannelController cc;

    public UIHandle()
    {
        InitScriptHandles();
    }

    ~UIHandle() { }

    public void InitScriptHandles()
    {
        cc = GameObject.Find("Canvas").GetComponent<ChannelController>();
        if (null == cc)
        {
            Debug.LogError("Cannot find ChannelController object.");
        }

        wUI = GameObject.Find("Canvas").GetComponent<WorldUI>();
        if (null == wUI)
        {
            Debug.LogError("Cannot find WorldUI object.");
        }

        fsUI = GameObject.Find("Canvas").GetComponent<FriendsUI>();
        if (null == fsUI)
        {
            Debug.LogError("Cannot find FriendsUI object.");
        }

        fUI = GameObject.Find("Canvas").GetComponent<FriendUI>();
        if (null == fUI)
        {
            Debug.LogError("Cannot find FriendUI object.");
        }

        sendUI = GameObject.Find("Canvas").GetComponent<SendUI>();
        if (null == sendUI)
        {
            Debug.LogError("Cannot find SendUI object.");
        }
    }

    public void ProcessSendingMessageOnUI(string msgId, MessageDirection direction, string sender, string receiver, string content)
    {
        DateTime t = DateTime.Now;

        if (Channels.World == cc.currentChannel ||
            Channels.Guild == cc.currentChannel ||
            Channels.Party == cc.currentChannel)
        {
            wUI.AddMessageToUI(msgId, sender, content, t);
            return;
        }

        if (Channels.Friend == cc.currentChannel)
        {
            fUI.AddSendingMessageToUI(sender, receiver, content, ContentType.Message,t);
            return;
        }
    }

    public void ProcessSendingHintOnUI(string content)
    {
        if (Channels.World == cc.currentChannel ||
            Channels.Guild == cc.currentChannel ||
            Channels.Party == cc.currentChannel)
        {
            wUI.AddHintToUI(content);
            return;
        }

        if (Channels.Friend == cc.currentChannel)
        {
            fUI.AddHintToUI(content);
            return;
        }
    }

    public void ProcessRecvingMessageOnUI(string msgId, Channels targetChannel, string sender, string receiver, string content)
    {
        if (Channels.Friend == targetChannel || Channels.Friends == targetChannel)
        {
            fsUI.ProcessRecvingMessageOnUI(msgId, sender, receiver, content);            
            return;
        }
        else if (Channels.World == targetChannel || Channels.Guild == targetChannel || Channels.Party == targetChannel)
        {
            wUI.ProcessRecvingMessageOnUI(targetChannel, msgId, sender, content);
            return;
        }
        else
        {
            return;
        }
    }

    public void ProcessOnContactInvited(string inviter, string reason)
    {

    }
};

public class Sdk : MonoBehaviour, IChatManagerDelegate, IContactManagerDelegate
{
    // Configrations
    private static string appKey = "";
    private static string worldRoomId = "";
    private static string guildRoomId = "";
    private static string partyRoomId = "";
    private static string userName = "";
    private static string password = "";
    private static string token = "";

    // Private object script
    private SendUI sendUI;
    private UIHandle uiHandle;

    private void Awake()
    {
        uiHandle = new UIHandle();

        GetSDKConfig();
        InitEaseMobSDK();
    }

    private void Start()
    {
        PrepareSDK();
        LoginToSDK();
    }

    private void Update()
    {

    }

    public static void GetSDKConfig()
    {
        SetAppKey("easemob-demo#unitytest");
        SetUserAndPassword("yqtest", "yqtest");
        //SetUserAndToken();

        // Please using http console to create rooms and then set room ids here
        SetWorldRoomId("205360469180417");
        SetGuildRoomId("205360500637703");
        SetPartyRoomId("205360528949251");
    }


    public static void SetAppKey(string key)
    {
        appKey = key;
    }

    public static void SetWorldRoomId(string id)
    {
        worldRoomId = id;
    }

    public static void SetGuildRoomId(string id)
    {
        guildRoomId = id;
    }

    public static void SetPartyRoomId(string id)
    {
        partyRoomId = id;
    }

    public static void SetUserAndPassword(string user, string passwd)
    {
        userName = user;
        password = passwd;
    }

    public static void SetUserAndToken(string user, string tk)
    {
        userName = user;
        token = tk;
    }

    public static string CurrentUserName()
    {
        return userName;
    }

    public static string GetRoomId(Channels channel)
    {
        if (Channels.World == channel) return worldRoomId;
        if (Channels.Guild == channel) return guildRoomId;
        if (Channels.Party == channel) return partyRoomId;
        return "";
    }

    public void InitEaseMobSDK()
    {
        Options options = new Options(appKey);
        options.AutoLogin = false;
        options.UsingHttpsOnly = true;
        options.DebugMode = true;
        SDKClient.Instance.InitWithOptions(options);
    }

    public void PrepareSDK()
    {
        // Add delegate for lisenter
        SDKClient.Instance.ChatManager.AddChatManagerDelegate(this);
    }

    public void LoginToSDK()
    {
        // Which login type is decided by customer
        LoginWithPassword(userName, password);
        //LoginWithAgoraTokenAction(uesrName, token);
    }

    public void LoginWithPassword(string user, string password)
    {
        SDKClient.Instance.Login(user, password,
            callback: new CallBack(

                onSuccess: () =>
                {
                    Debug.Log("login with password succeed");
                    CheckRooms();
                },

                onError: (code, desc) =>
                {
                    if (code == 200)
                    {
                        Debug.Log("Already logined");
                        CheckRooms();
                    }
                    else
                    {
                        Debug.Log($"Failed to login， code:{code}, desc:{desc}");
                    }
                }
            )
        );
    }

    public void LoginWithAgoraTokenAction(string user, string token)
    {
        SDKClient.Instance.LoginWithAgoraToken(user, token,
            callback: new CallBack(

                onSuccess: () =>
                {
                    Debug.Log("login with agora token succeed");
                    CheckRooms();
                },

                onError: (code, desc) =>
                {
                    if (code == 200)
                    {
                        Debug.Log("Already logined");
                        CheckRooms();
                    }
                    else
                    {
                        Debug.Log($"Failed to login with agora token， code:{code}, desc:{desc}");
                    }
                }
            )
        );
    }

    public void CheckRooms()
    {
        CheckRoom(worldRoomId, Channels.World);
        CheckRoom(guildRoomId, Channels.World);
        CheckRoom(partyRoomId, Channels.World);
    }

    public void CheckRoom(string roomId, Channels channel)
    {
        SDKClient.Instance.RoomManager.FetchRoomInfoFromServer(roomId, new ValueCallBack<Room>(
                onSuccess: (room) => {
                    Debug.Log($"room:{roomId} exist already.");
                    JoinRoom(roomId, channel);
                },
                onError: (code, desc) => {
                    Debug.Log($"room:{roomId} is NOT exist, create a new room.");
                    CreateRoom(channel);
                }
        ));
    }

    public void CreateRoom(Channels channel)
    {
        string roomDesc = "DemoRoom";
        if (Channels.World == channel) roomDesc = "WorldRoom";
        if (Channels.Guild == channel) roomDesc = "GuildRoom";
        if (Channels.Party == channel) roomDesc = "PartyRoom";

        SDKClient.Instance.RoomManager.CreateRoom(name, roomDesc, "", 300, null, new ValueCallBack<Room>(
                onSuccess: (room) => {

                    if (Channels.World == channel) worldRoomId = room.RoomId;
                    if (Channels.Guild == channel) guildRoomId = room.RoomId;
                    if (Channels.Party == channel) partyRoomId = room.RoomId;

                    Debug.Log($"Create {roomDesc} with id {room.RoomId} successfully.");

                    JoinRoom(room.RoomId, channel);
                },
                onError: (code, desc) => {
                    Debug.LogError($"Failed to create room of {roomDesc}.");
                }
            ));
    }

    public void JoinRoom(string roomId, Channels channel)
    {
        string roomDesc = "";
        if (Channels.World == channel) roomDesc = "WorldRoom";
        if (Channels.Guild == channel) roomDesc = "GuildRoom";
        if (Channels.Party == channel) roomDesc = "PartyRoom";

        SDKClient.Instance.RoomManager.JoinRoom(roomId, new ValueCallBack<Room>(
                onSuccess: (room) => {
                    Debug.Log($"Join {roomDesc}:{roomId} successfully.");
                },
                onError: (code, desc) => {
                    Debug.LogError($"Failed to join {roomDesc}:{roomId}.");
                }
            ));
    }

    public void SendTextMessage(string receiver, string content, MessageType type)
    {
        Message msg = Message.CreateTextSendMessage(receiver, content);
        msg.MessageType = type; // Chat or Room
        SDKClient.Instance.ChatManager.SendMessage(ref msg, new CallBack(
            onSuccess: () =>
            {
                Debug.Log($"send message success, msgid:{msg.MsgId}");

                // Show this message on UI
                TextBody tb = (TextBody)msg.Body;
                uiHandle.ProcessSendingMessageOnUI(msg.MsgId, msg.Direction, msg.From, msg.To, tb.Text);
            },
            onProgress: (progress) =>
            {
                Debug.Log($"send message progress, progress:{progress.ToString()}");
            },
            onError: (code, desc) =>
            {
                Debug.Log($"send message failed, code:{code}, desc:{desc}");

                // Show send failure reason on UI
                uiHandle.ProcessSendingHintOnUI("send failed due to: " + desc);
            }
        ));
    }

    public void AddFriend(string username)
    {
        SDKClient.Instance.ContactManager.AddContact(username, "hello", new CallBack(
                onSuccess: () =>
                {
                    Debug.Log($"AddContact success.");
                },
                onError: (code, desc) =>
                {
                    Debug.Log($"AddContact failed, code:{code}, desc:{desc}");
                }
        ));
    }

    public void AcceptFriend(string userName)
    {
        SDKClient.Instance.ContactManager.AcceptInvitation(userName, new CallBack(
            onSuccess: () =>
            {
                Console.WriteLine($"AcceptInvitation success from {userName}.");
            },
            onError: (code, desc) =>
            {
                Console.WriteLine($"AcceptInvitation failed from {userName}, code:{code}, desc:{desc}");
            }
        ));
    }

    public void DeclineFriend(string userName)
    {
        SDKClient.Instance.ContactManager.DeclineInvitation(userName, new CallBack(
            onSuccess: () =>
            {
                Debug.Log($"DeclineInvitation success from {userName}.");
            },
            onError: (code, desc) =>
            {
                Debug.Log($"DeclineInvitation failed from {userName}, code:{code}, desc:{desc}");
            }
        ));
    }


    public List<string> LoadAllConversations()
    {
        List<Conversation> list = SDKClient.Instance.ChatManager.LoadAllConversations();
        List<string> ret = new List<string>();

        Debug.Log($"Load conversation num:{list.Count}");

        foreach (var conv in list)
        {
            ret.Add(conv.Id);
        }

        return ret;
    }

    public void LastMessageFromFriend(string convId, out string sender, out string receiver, out string content, out DateTime time)
    {
        Conversation conv = SDKClient.Instance.ChatManager.GetConversation(convId, ConversationType.Chat);

        Message msg = conv.LastMessage;

        Debug.Log($"Loat last message id:{msg.MsgId} for conversation: {convId}");

        if (null != msg)
        {
            sender = msg.From;
            receiver = msg.To;
            TextBody tb = (TextBody)msg.Body;
            content = tb.Text;
            time = Tools.GetTimeFromTS(msg.ServerTime);
        }
        else
        {
            sender = convId;
            receiver = "";
            content = "";
            time = DateTime.Now;
        }
    }

    public void AddRection()
    {

    }

    void IChatManagerDelegate.OnMessagesReceived(List<Message> messages)
    {
        // Add the related message in corresponding panel
        // Update hint bubble on tabs
        // Update hint bubble on tab Friends
        // Update hint bubble on the friend in friend list; update message content and time
        // Add the related message in the friend talking panel

        if (null == messages || messages.Count == 0) return;

        foreach (var it in messages)
        {
            MessageType mtype = it.MessageType;
            MessageBodyType bType = it.Body.Type;

            // Only process text message here
            if (MessageBodyType.TXT != bType)
            {
                Debug.Log($"Receive a None Text message, msgId:{it.MsgId}, discard it.");
                continue;
            }

            Channels targetChannel = Channels.World;

            TextBody tb = (TextBody)it.Body;
            if (MessageType.Chat == mtype)
            {
                targetChannel = Channels.Friend;
                uiHandle.ProcessRecvingMessageOnUI(it.MsgId, targetChannel, it.From, it.To, tb.Text);
                continue;
            }
            if (MessageType.Room == mtype)
            {
                if (it.To.CompareTo(worldRoomId) == 0) targetChannel = Channels.World;
                if (it.To.CompareTo(guildRoomId) == 0) targetChannel = Channels.Guild;
                if (it.To.CompareTo(partyRoomId) == 0) targetChannel = Channels.Party;

                uiHandle.ProcessRecvingMessageOnUI(it.MsgId, targetChannel, it.From, it.To,tb.Text);
                continue;
            }
            if (MessageType.Group == mtype)
            {
                Debug.Log($"Received group message, msgid:{it.MsgId}, discard it.");
                continue;
            }
        }
    }

    void IChatManagerDelegate.MessageReactionDidChange(List<MessageReactionChange> list)
    {
        
    }

    void IChatManagerDelegate.OnCmdMessagesReceived(List<Message> messages)
    {
        
    }

    void IChatManagerDelegate.OnConversationRead(string from, string to)
    {
        
    }

    void IChatManagerDelegate.OnConversationsUpdate()
    {        
    }

    void IChatManagerDelegate.OnGroupMessageRead(List<GroupReadAck> list)
    {
        
    }

    void IChatManagerDelegate.OnMessagesDelivered(List<Message> messages)
    {
        
    }

    void IChatManagerDelegate.OnMessagesRead(List<Message> messages)
    {
        
    }

    void IChatManagerDelegate.OnMessagesRecalled(List<Message> messages)
    {
        
    }

    void IChatManagerDelegate.OnReadAckForGroupMessageUpdated()
    {
        
    }

    void IContactManagerDelegate.OnContactAdded(string userId)
    {
        Debug.Log("OnContactAdded");
    }

    void IContactManagerDelegate.OnContactDeleted(string userId)
    {
        
    }

    void IContactManagerDelegate.OnContactInvited(string userId, string reason)
    {
        Debug.Log("OnContactInvited");

    }

    void IContactManagerDelegate.OnFriendRequestAccepted(string userId)
    {
        
    }

    void IContactManagerDelegate.OnFriendRequestDeclined(string userId)
    {
        
    }
}
