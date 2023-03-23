using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplyActionButton : MonoBehaviour
{
    private Transform avatarImage;
    private Transform nameText;
    private Transform timeText;
    private Transform contentText;
    private Transform ignoreButton;
    private Transform acceptButton;
    private Transform ignoreText;
    private Transform acceptText;
    private Transform ignoreImage;
    private Transform acceptImage;

    private MyResources myResources;
    private Sdk sdk;

    private string friend;

    private void Awake()
    {
        myResources = GameObject.Find("Canvas").GetComponent<MyResources>();
        sdk = GameObject.Find("Canvas").GetComponent<Sdk>();

        avatarImage     = transform.GetChild(0).GetChild(0);
        nameText        = transform.GetChild(1).GetChild(0).GetChild(0);
        timeText        = transform.GetChild(1).GetChild(0).GetChild(1);
        contentText     = transform.GetChild(1).GetChild(1).GetChild(0);
        ignoreButton    = transform.GetChild(2).GetChild(0);
        acceptButton    = transform.GetChild(2).GetChild(1);
        ignoreText      = ignoreButton.GetChild(0);
        acceptText      = acceptButton.GetChild(0);
        ignoreImage     = ignoreButton.GetChild(1);
        acceptImage     = acceptButton.GetChild(1);

        contentText.GetComponent<TextMeshProUGUI>().text = "Send a friend request.";

        ignoreButton.GetComponent<Button>().onClick.AddListener(IgnoreButtonClick);
        acceptButton.GetComponent<Button>().onClick.AddListener(AcceptButtonClick);
    }

    void IgnoreButtonClick()
    {
        ignoreButton.GetComponent<Button>().interactable = false;
        ignoreText.GetComponent<TextMeshProUGUI>().text = "";
        ignoreImage.gameObject.SetActive(true);

        acceptButton.GetComponent<Button>().interactable = false;

        sdk.DeclineFriend(friend);
    }

    void AcceptButtonClick()
    {
        ignoreButton.GetComponent<Button>().interactable = false;
        
        acceptButton.GetComponent<Button>().interactable = false;
        acceptText.GetComponent<TextMeshProUGUI>().text = "";
        acceptImage.gameObject.SetActive(true);

        sdk.AcceptFriend(friend);
    }

    public void ReceiveContactInvited(Notice notice)
    {
        string user = notice.params_[0].ToString();
        long ts = long.Parse(notice.params_[2].ToString());

        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(user));
        nameText.GetComponent<TextMeshProUGUI>().text = user;
        timeText.GetComponent<TextMeshProUGUI>().text = Tools.GetTimeHHMMFromTs(ts);

        friend = user;
    }
}
