using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvokeButton : MonoBehaviour
{
    public Texture2D hoveredCusor;

    private HintBubbleButton hintBubble;
    private MyResources myResource;
    private Context context;
    private Image invokeImage;

    private List<string> myPath;

    private void Awake()
    {
        context     = GameObject.Find("Canvas").GetComponent<Context>();
        myResource  = GameObject.Find("Canvas").GetComponent<MyResources>();
        invokeImage = transform.GetChild(0).GetComponent<Image>();
        hintBubble  = transform.GetChild(1).GetComponent<HintBubbleButton>();

        myPath = new List<string> { transform.tag };
    }

    // Start is called before the first frame update
    void Start()
    {
        if (false == context.initSDK)
        {
            // Init sdk failed, then just show button with failed icon
            invokeImage.sprite = myResource.invokeFailSprite;
        }
        else
        {
            // Init sdk success, then permit to switch to main panel
            transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ButtonClick);
        }
        Switch.Show(transform);
        hintBubble.ShowHide();
    }

    void ButtonClick()
    {
        SendNoticeForSwitch();
        Switch.Hide(transform);
    }

    void SendNoticeForSwitch()
    {
        List<string> dst = new List<string> { "World" };
        Notice notice = new Notice("SwitchTo", null, dst);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    public void SwitchTo(Notice notice)
    {
        Switch.SwitchTo(notice.dst_, myPath, transform);
    }

    public void HintBubbleChanged(Notice notice)
    {
        int hintNum = int.Parse(notice.params_[0].ToString());
        hintBubble.Add(hintNum);
    }
}
