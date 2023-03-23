using System.Collections.Generic;
using UnityEngine;
using AgoraChat;
using AgoraChat.MessageBody;
using System;

public class Sdk : MonoBehaviour, IChatManagerDelegate, IContactManagerDelegate
{
    private readonly string configFile  = "sdk.text";

    // Items in config file
    private readonly string appKeyStr   = "AppKey";
    private readonly string useTokenStr = "UseToken";
    private readonly string userNameStr = "UserName";
    private readonly string passwordStr = "Password";
    private readonly string tokenStr    = "Token";
    private readonly string worldStr    = "World";
    private readonly string guildStr    = "Guild";
    private readonly string partyStr    = "Party";

    // Configrations
    private string appKey   = "";
    private string userName = "";
    private string password = "";
    private string token    = "";
    private string useToken = "false";

    private Dictionary<string, string> name2id;
    private Dictionary<string, string> id2name;

    private Context context;
    private UIManager uiManager;

    private List<string> contactList;

    private void Awake()
    {
        uiManager   = GameObject.Find("Canvas").GetComponent<UIManager>();
        context     = GameObject.Find("Canvas").GetComponent<Context>();
        context.initSDK = false;

        string filePath = Tools.GetCurrentDirectory2(configFile);
        Debug.Log($"Config file Path is: {filePath}");

        if (!InitFromConfigFile(filePath))
        {
            Debug.LogError($"Error in config file {configFile}, please check it.");
            context.initSDK = false;
            return;
        }
        
        if (InitEaseMobSDK() != 0)
        {
            context.initSDK = false;
            return;
        }

        context.initSDK = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        PrepareSDK();
        LoginToSDK();
    }

    bool InitFromConfigFile(string filePath)
    {
        // Parse config file
        Dictionary<string, string> cfgDict = FileParser.Parse(filePath);
        if (cfgDict.Count == 0) return false;

        // Set config items
        if (cfgDict.ContainsKey(appKeyStr)) appKey = cfgDict[appKeyStr];
        if (cfgDict.ContainsKey(useTokenStr)) useToken = cfgDict[useTokenStr];
        if (cfgDict.ContainsKey(userNameStr)) userName = cfgDict[userNameStr];
        if (cfgDict.ContainsKey(passwordStr)) password = cfgDict[passwordStr];
        if (cfgDict.ContainsKey(tokenStr)) token = cfgDict[tokenStr];

        name2id = new Dictionary<string, string>();
        id2name = new Dictionary<string, string>();
        if (cfgDict.ContainsKey(worldStr)) 
        {
            name2id.Add(worldStr, cfgDict[worldStr]);
            id2name.Add(cfgDict[worldStr], worldStr);
        }

        if (cfgDict.ContainsKey(guildStr))
        {
            name2id.Add(guildStr, cfgDict[guildStr]);
            id2name.Add(cfgDict[guildStr], guildStr);
        }

        if (cfgDict.ContainsKey(partyStr))
        {
            name2id.Add(partyStr, cfgDict[partyStr]);
            id2name.Add(cfgDict[partyStr], partyStr);
        }

        // Check config items
        if (appKey.Length == 0) return false;
        if (userName.Length == 0) return false;
        if (useToken.CompareTo("false") == 0 && password.Length == 0) return false;
        if (useToken.CompareTo("true") == 0 && token.Length == 0) return false;
        if (name2id.Count < 3) return false;
        if (!name2id.ContainsKey(worldStr) || name2id[worldStr].Length == 0) return false;
        if (!name2id.ContainsKey(guildStr) || name2id[guildStr].Length == 0) return false;
        if (!name2id.ContainsKey(partyStr) || name2id[partyStr].Length == 0) return false;

        return true;
    }

    public string GetAppkey()
    {
        return appKey;
    }

    public string GetUserName()
    {
        return userName;
    }

    public string GetPassWord()
    {
        return password;
    }

    public string GetToken()
    {
        return token;
    }

    public string GetTagNameById(string id)
    {
        if (id2name.ContainsKey(id) == true)
        {
            return id2name[id];
        }
        else
        {
            return "";
        }
    }

    public string GetIdByTagName(string tag)
    {
        if (name2id.ContainsKey(tag) == true)
        {
            return name2id[tag];
        }
        else
        {
            return "";
        }
    }

    public int InitEaseMobSDK()
    {
        Options options = new Options(appKey)
        {
            AutoLogin = false,
            UsingHttpsOnly = true,
            DebugMode = true
        };
        return SDKClient.Instance.InitWithOptions(options);
    }

    public void PrepareSDK()
    {
        // Add delegate for lisenter
        SDKClient.Instance.ChatManager.AddChatManagerDelegate(this);
        SDKClient.Instance.ContactManager.AddContactManagerDelegate(this);
    }

    public void LoginToSDK()
    {
        if(useToken.CompareTo("false") == 0)
        {
            LoginWithPassword(userName, password);
        }
        else if (useToken.CompareTo("true") == 0)
        {
            LoginWithAgoraTokenAction(userName, token);
        }
        else
        {
            LoginWithPassword(userName, password);
        }
    }

    public void LoginWithPassword(string user, string password)
    {
        SDKClient.Instance.Login(user, password,
            callback: new CallBack(

                onSuccess: () =>
                {
                    Debug.Log("login with password succeed");
                    LoadAllContacts();
                    CheckRooms();
                },

                onError: (code, desc) =>
                {
                    if (code == 200)
                    {
                        Debug.Log("Already logined");
                        LoadAllContacts();
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
                    LoadAllContacts();
                    CheckRooms();
                },

                onError: (code, desc) =>
                {
                    if (code == 200)
                    {
                        Debug.Log("Already logined");
                        LoadAllContacts();
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
        CheckRoom("World");
        CheckRoom("Guild");
        CheckRoom("Party");
    }

    public void CheckRoom(string tag)
    {
        string id = name2id[tag];

        SDKClient.Instance.RoomManager.FetchRoomInfoFromServer(id, new ValueCallBack<Room>(
                onSuccess: (room) => {
                    Debug.Log($"room:{id} exist already.");
                    JoinRoom(id);
                },
                onError: (code, desc) => {
                    Debug.Log($"room:{id} is NOT exist, create a new room.");
                }
        ));
    }    

    public void JoinRoom(string id)
    {
        string name = id2name[id];

        SDKClient.Instance.RoomManager.JoinRoom(id, new ValueCallBack<Room>(
                onSuccess: (room) => {
                    Debug.Log($"Join {name}:{id} successfully.");
                },
                onError: (code, desc) => {
                    Debug.LogError($"Failed to join {name}:{id}.");
                }
            ));
    }
    
    void SendNoticeForSendAMessage(string from, string to, string content, long ts)
    {
        List<string> dst = context.GetCurrentPath();
        Notice notice = new Notice("SendAMessage", null, dst, from, to, content, ts);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendNoticeForMessageHint(string hint)
    {
        List<string> dst = context.GetCurrentPath();
        long ts = Tools.ToTimeStamp(DateTime.Now);
        Notice notice = new Notice("MessageHint", null, dst, hint, ts);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    public void SendTextMessage(string receiver, string content)
    {
        string id = receiver;
        MessageType mtype = MessageType.Chat;

        if (name2id.ContainsKey(receiver) == true) 
        {
            id = name2id[receiver];
            mtype = MessageType.Room;
        }

        Message msg = Message.CreateTextSendMessage(id, content);
        msg.MessageType = mtype;

        SDKClient.Instance.ChatManager.SendMessage(ref msg, new CallBack(
            onSuccess: () =>
            {
                Debug.Log($"send message success, msgid:{msg.MsgId}");

                TextBody tb = (TextBody)msg.Body;
                SendNoticeForSendAMessage(msg.From, msg.To, tb.Text, msg.ServerTime/1000);
            },
            onProgress: (progress) =>
            {
                Debug.Log($"send message progress, progress:{progress.ToString()}");
            },
            onError: (code, desc) =>
            {
                Debug.Log($"send message failed, code:{code}, desc:{desc}");
                SendNoticeForMessageHint("send failed: " + desc);
            }
        ));
    }

    public void AddFriend(string username)
    {
        SDKClient.Instance.ContactManager.AddContact(username, "hello", new CallBack(
                onSuccess: () =>
                {
                    Debug.Log($"AddContact success.");
                    SendNoticeForMessageHint("Your friend request has been sent.");
                },
                onError: (code, desc) =>
                {
                    Debug.Log($"AddContact failed, code:{code}, desc:{desc}");
                }
        ));
    }

    void SendNoticeForEnableSend()
    {
        Notice notice = new Notice("EnableSend", null, null);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    public void AcceptFriend(string userName)
    {
        SDKClient.Instance.ContactManager.AcceptInvitation(userName, new CallBack(
            onSuccess: () =>
            {
                SendNoticeForEnableSend();
                SendNoticeForMessageHint($"You have added {userName}. Start chatting!");
                Debug.Log($"AcceptInvitation success from {userName}.");
            },
            onError: (code, desc) =>
            {
                Debug.Log($"AcceptInvitation failed from {userName}, code:{code}, desc:{desc}");
            }
        ));
    }

    void SendNoticeForDeclineFriend(string userName)
    {
        Notice notice = new Notice("DeclineFriend", null, null, userName);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    public void DeclineFriend(string userName)
    {
        SDKClient.Instance.ContactManager.DeclineInvitation(userName, new CallBack(
            onSuccess: () =>
            {
                SendNoticeForMessageHint($"You ignore the request.");
                SendNoticeForDeclineFriend(userName);
                Debug.Log($"DeclineInvitation success from {userName}.");
            },
            onError: (code, desc) =>
            {
                Debug.Log($"DeclineInvitation failed from {userName}, code:{code}, desc:{desc}");
            }
        ));
    }

    public void LoadAllContacts()
    {
        SDKClient.Instance.ContactManager.GetAllContactsFromServer(new ValueCallBack<List<string>>(
            onSuccess: (list) =>
            {
                contactList = list;
                Debug.Log($"GetAllContactsFromServer success with contact num: {contactList.Count}.");
                foreach (var it in contactList)
                {
                    LoadAContact(it);
                    Debug.Log($"contactor: {it}");
                }
            },
            onError: (code, desc) =>
            {
                Debug.Log($"GetAllContactsFromServer failed, code:{code}, desc:{desc}");
            }
            ));
    }

    public void LoadAContact(string contact)
    {
        if (null == contact || contact.Length == 0) return;

        Message msg = LoadLastMessage(contact);

        if (null != msg)
        {
            TextBody tb = (TextBody)msg.Body;
            SendNoticeForLoadAContactWithMsg(msg.From, msg.To, tb.Text, msg.ServerTime / 1000);
        }
        else
        {
            SendNoticeForLoadAContactWithoutMsg(contact);
        }
    }

    void SendNoticeForLoadAContactWithMsg(string from, string to, string content, long ts)
    {
        MessageDirection direction = (from.CompareTo(GetUserName()) == 0) ? MessageDirection.SEND : MessageDirection.RECEIVE;

        List<string> dst = MakeDst(direction, from, to);
        Notice notice = new Notice("LoadAContactWithMsg", null, dst, from, to, content, ts);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendNoticeForLoadAContactWithoutMsg(string contact)
    {
        List<string> dst = new List<string> { "Chat", contact };

        Notice notice = new Notice("LoadAContactWithoutMsg", null, dst);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    public Message LoadLastMessage(string convId)
    {
        Conversation conv = SDKClient.Instance.ChatManager.GetConversation(convId, ConversationType.Chat);

        Message msg = conv.LastMessage;

        return msg;
    }

    public List<string> GetContactList()
    {
        return contactList;
    }

    public bool IsMyFriend(string name)
    {
        List<string> list = SDKClient.Instance.ContactManager.GetAllContactsFromDB();

        if (null == list) return false;

        if (list.Contains(name) == true) return true;

        return false;
    }

    public List<string> LoadAllChatConversations()
    {
        List<Conversation> list = SDKClient.Instance.ChatManager.LoadAllConversations();
        List<string> ret = new List<string>();

        Debug.Log($"Load conversation num:{list.Count}");

        foreach (var conv in list)
        {
            if (ConversationType.Chat == conv.Type) ret.Add(conv.Id);
        }

        return ret;
    }

    List<string> MakeDst(MessageDirection direction, string from, string to)
    {
        List<string> dst;

        // Room message
        if (null != to && id2name.ContainsKey(to) == true)
        {
            dst = new List<string> { id2name[to] };
        }
        // Chat message
        else
        {
            dst = new List<string> { "Chat" };
            if (MessageDirection.SEND == direction)
            {
                dst.Add(to);
            }
            else // MessageDirection.RECEIVE
            {
                dst.Add(from);
            }
        }

        return dst;
    }

    void SendNoticeForReceiveAMessage(string from, string to, string content, long ts)
    {
        List<string> dst = MakeDst(MessageDirection.RECEIVE, from, to);
        Notice notice = new Notice("ReceiveAMessage", null, dst, from, to, content, ts);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void IChatManagerDelegate.OnMessagesReceived(List<Message> messages)
    {
        if (null == messages || messages.Count == 0) return;

        foreach (var it in messages)
        {
            MessageType mtype = it.MessageType;
            MessageBodyType bType = it.Body.Type;

            if (MessageBodyType.TXT != bType || MessageType.Group == mtype)
            {
                Debug.Log($"Receive a message Not belong to text or room message, msgId:{it.MsgId}, discard it.");
                continue;
            }

            TextBody tb = (TextBody)it.Body;

            SendNoticeForReceiveAMessage(it.From, it.To, tb.Text, it.ServerTime/1000);
        }
    }

    void IChatManagerDelegate.MessageReactionDidChange(List<MessageReactionChange> list)
    {
        //TODO: Add or update reaction to UI
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
    void SendNoticeForOnContactInvited(string userId, string reason)
    {
        List<string> dst = MakeDst(MessageDirection.RECEIVE, userId, null);
        DateTime t = DateTime.Now;
        long ts = Tools.ToTimeStamp(t);
        Notice notice = new Notice("ReceiveContactInvited", null, dst, userId, reason, ts);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void IContactManagerDelegate.OnContactInvited(string userId, string reason)
    {
        Debug.Log("OnContactInvited");
        SendNoticeForOnContactInvited(userId, reason);
    }

    void SendNoticeForOnFriendRequestAccepted(string userId)
    {
        List<string> dst = MakeDst(MessageDirection.RECEIVE, userId, null);
        DateTime t = DateTime.Now;
        long ts = Tools.ToTimeStamp(t);
        string hint = userId + " accepted your request. Start chatting!";
        Notice notice = new Notice("ReceiveFriendRequestAccepted", null, dst, userId, hint, ts);
        uiManager.SendMessage("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void IContactManagerDelegate.OnFriendRequestAccepted(string userId)
    {
        Debug.Log("OnFriendRequestAccepted");
        SendNoticeForOnFriendRequestAccepted(userId);
    }

    void IContactManagerDelegate.OnFriendRequestDeclined(string userId)
    {

    }
}
