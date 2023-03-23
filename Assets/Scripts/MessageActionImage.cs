using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageActionImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D hoveredCursor;
    public Texture2D focusedCusor;

    private MyResources myResources;
    private Sdk sdk;

    private Transform addFriendButtonTransform;
    private Transform chatSwitchButtonTranform;

    private float imageHeight;

    private void Awake()
    {
        myResources                 = GameObject.Find("Canvas").GetComponent<MyResources>();
        sdk                         = GameObject.Find("Canvas").GetComponent<Sdk>();

        addFriendButtonTransform    = GameObject.Find("Canvas/AddFriendButton").transform;
        chatSwitchButtonTranform    = GameObject.Find("Canvas/ChatSwitchButton").transform;

        imageHeight                 = transform.GetComponent<RectTransform>().rect.height;

        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    string GetFriendName()
    {
        Transform messageButtonTransform = transform.parent.parent.parent;
        Transform nameText = messageButtonTransform.GetChild(1).GetChild(0).GetChild(0);
        return nameText.GetComponent<TextMeshProUGUI>().text;
    }

    // For message in Chat no need to ButtonClick
    void ButtonClick()
    {
        string friendName = GetFriendName();
        if (sdk.IsMyFriend(friendName))
        {
            chatSwitchButtonTranform.SendMessage("Show");
            chatSwitchButtonTranform.position = new Vector3(chatSwitchButtonTranform.position.x, transform.position.y - imageHeight);
            chatSwitchButtonTranform.SendMessage("SetFriendName", friendName);
        }
        else
        {
            addFriendButtonTransform.SendMessage("Show");
            addFriendButtonTransform.position = new Vector3(addFriendButtonTransform.position.x, transform.position.y - imageHeight);
            addFriendButtonTransform.SendMessage("SetFriendName", friendName);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetComponent<Image>().sprite = myResources.ellipsis1Sprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetComponent<Image>().sprite = myResources.ellipsisSprite;
    }

}
