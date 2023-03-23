using UnityEngine;
using UnityEngine.UI;

public class SingleEmojiButton : MonoBehaviour
{
    public Texture2D hoveredCursor;
    public Texture2D defaultCursor;

    private string emojiName;

    private void Awake()
    {
        Image image = transform.GetComponent<Image>();
        emojiName   = transform.GetComponent<Image>().sprite.name;
        Button btn  = transform.GetComponent<Button>();
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    void ButtonClick()
    {
        SendNoticeForAddEmoji();
        SendNoticeForHideEmojiPanel();
    }

    void SendNoticeForAddEmoji()
    {
        Notice notice = new Notice("AddEmoji", null, null, emojiName);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void SendNoticeForHideEmojiPanel()
    {
        Notice notice = new Notice("HideEmojiPanel", null, null);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }
}
