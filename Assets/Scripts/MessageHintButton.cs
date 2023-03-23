using TMPro;
using UnityEngine;

public class MessageHintButton : MonoBehaviour
{
    private Transform hintText;
    private Transform timeText;

    private void Awake()
    {
        hintText = transform.GetChild(0).GetChild(0);
        timeText = transform.GetChild(1).GetChild(0);
    }

    public void MessageHint(Notice notice)
    {
        string hint = notice.params_[0].ToString();
        long ts = long.Parse(notice.params_[1].ToString());

        hintText.GetComponent<TextMeshProUGUI>().text = hint;
        timeText.GetComponent<TextMeshProUGUI>().text = Tools.GetTimeHHMMFromTs(ts);
    }

    public void ReceiveFriendRequestAccepted(Notice notice)
    {
        string hint = notice.params_[1].ToString();
        long ts = long.Parse(notice.params_[2].ToString());

        hintText.GetComponent<TextMeshProUGUI>().text = hint;
        timeText.GetComponent<TextMeshProUGUI>().text = Tools.GetTimeHHMMFromTs(ts);
    }
}
