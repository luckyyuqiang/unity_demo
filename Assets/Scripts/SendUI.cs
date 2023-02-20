using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendUI : MonoBehaviour
{
    // Private script object
    private ChannelController cc;
    private WorldUI wUI;
    private FriendsUI fsUI;
    private FriendUI fUI;
    private Sdk sdkHandle;

    // UI elements section
    private Transform send_Panel_Transform;
    private Transform emoji_Image_Button_Transform;
    private Transform send_InputField_Transform;
    private Transform send_Button_Transform;

    private Button send_Button;
    private TMP_InputField send_InputField;

    private void Awake()
    {
        InitUIItems();
        InitScriptHandles();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void InitUIItems()
    {
        send_Panel_Transform = transform.Find("Main_Panel/Send_Panel").transform;
        emoji_Image_Button_Transform = transform.Find("Main_Panel/Send_Panel/Emoji_Image_Button").transform;
        send_InputField_Transform = transform.Find("Main_Panel/Send_Panel/Send_InputField").transform;
        send_Button_Transform = transform.Find("Main_Panel/Send_Panel/Send_Button").transform;

        Tools.CheckTransform(send_Panel_Transform, "Send_Panel");
        Tools.CheckTransform(emoji_Image_Button_Transform, "Emoji_Image_Button");
        Tools.CheckTransform(send_InputField_Transform, "Send_InputField");
        Tools.CheckTransform(send_Button_Transform, "Send_Button");


        send_Button = send_Button_Transform.GetComponent<Button>();
        send_Button.onClick.AddListener(SendButtonAction);

        send_InputField = send_InputField_Transform.GetComponent<TMP_InputField>();
    }

    private void InitScriptHandles()
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

        sdkHandle = GameObject.Find("Canvas").GetComponent<Sdk>();
        if (null == sdkHandle)
        {
            Debug.LogError("Cannot find Sdk object.");
        }
    }

    private void EmojiButtonAction()
    {
        // Pop emoji ui
    }

    private void SendButtonAction()
    {
        if (send_InputField.text.Length == 0) return;

        if (Channels.World == cc.currentChannel ||
            Channels.Guild == cc.currentChannel ||
            Channels.Party == cc.currentChannel)
        {
            // Send room message
            string receiver = Sdk.GetRoomId(cc.currentChannel);
            sdkHandle.SendTextMessage(receiver, send_InputField.text, AgoraChat.MessageType.Room);
        }
        else if (Channels.Friend == cc.currentChannel)
        {
            // Send single chat message
            string receiver = fUI.GetCurrentFriendName();
            sdkHandle.SendTextMessage(receiver, send_InputField.text, AgoraChat.MessageType.Chat);
        }
        else
        {
            Debug.LogError($"In channel:{cc.currentChannel} is NOT permit to send message.");
            return;
        }

        send_InputField.text = "";
    }

    public void EnableSendUI()
    {
        send_Panel_Transform.gameObject.SetActive(true);
    }

    public void DisableSendUI()
    {
        send_Panel_Transform.gameObject.SetActive(false);
    }
}
