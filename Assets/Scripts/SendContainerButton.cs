using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendContainerButton : MonoBehaviour
{
    private Transform emojiButtonTransform;
    private Transform inputFieldTransform;
    private Transform sendButtonTransform;
    private Transform maskButtonTransform;

    private TMP_InputField inputField;

    private Sdk sdk;

    private string receiver;

    private void Awake()
    {
        sdk                     = GameObject.Find("Canvas").GetComponent<Sdk>();

        emojiButtonTransform    = transform.GetChild(0);
        inputFieldTransform     = transform.GetChild(1);
        sendButtonTransform     = transform.GetChild(2);
        maskButtonTransform     = transform.GetChild(3);

        inputField              = inputFieldTransform.GetComponent<TMP_InputField>();

        sendButtonTransform.GetComponent<Button>().onClick.AddListener(SendClick);
        emojiButtonTransform.GetComponent<Button>().onClick.AddListener(EmojiClick);
    }

    void EmojiClick()
    {
        Notice notice = new Notice("ShowEmojiPanel", null, null);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendClick()
    {
        if (inputField.text.Length == 0) return;
        sdk.SendTextMessage(receiver, inputField.text);
        inputField.text = "";
    }

    public void EnableSend()
    {
        maskButtonTransform.gameObject.SetActive(false);
    }

    public void DisableSend()
    {
        maskButtonTransform.gameObject.SetActive(true);        
    }

    void EnableOrDisableSend(string friend)
    {
        DisableSend();

        if (null == friend || friend.Length == 0) return;        

        if (sdk.IsMyFriend(friend) == true)
        {
            EnableSend();
        }
    }

    void SetReceiver(List<string> dst)
    {
        receiver = "";

        if (dst.Count == 1)
        {
            receiver = dst[0];
        }
        else if (dst.Count == 2)
        {
            receiver = dst[1];
        }
    }

    public void SwitchTo(Notice notice)
    {
        List<string> dst = notice.dst_;

        if (dst[0].CompareTo("World") == 0
         || dst[0].CompareTo("Guild") == 0
         || dst[0].CompareTo("Party") == 0)
        {
            Switch.Show(transform);
            SetReceiver(dst);
        }
        else if (dst[0].CompareTo("Chat") == 0)
        {
            Switch.Show(transform);
            SetReceiver(dst);
            EnableOrDisableSend(dst[1]);
        }
        else
        {
            Switch.Hide(transform);
        }
    }

    public void AddEmoji(Notice notice)
    {
        string emojiName = notice.params_[0].ToString();

        int position = Tools.GetRealCaretPosition(inputField.text, inputField.caretPosition);
        Debug.Log($"pos: {position}");

        string pre = inputField.text.Substring(0, position);
        string post = inputField.text.Substring(position, inputField.text.Length - position);

        string unicode = Tools.GetEmojiUnicode(emojiName);
        inputField.text = pre + unicode + post;
        inputField.caretPosition++;
    }
}
