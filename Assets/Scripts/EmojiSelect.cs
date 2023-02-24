using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmojiSelect : MonoBehaviour
{

    // Private object script
    private SendUI sendUI;

    private static Dictionary<string, string> dictOfFileName2Unicode = new Dictionary<string, string>() {
            { "ee_1","\U0001F60A" },{ "ee_2","\U0001F604" },{ "ee_3","\U0001F609" },
            { "ee_4","\U0001F626" },{ "ee_5","\U0001F60B" },{ "ee_6","\U0001F60E" },
            { "ee_7","\U0001F621" },{ "ee_8","\U0001F615" },{ "ee_9","\U0001F60C" },
            { "ee_10","\U0001F610" }
    };

    private void Awake()
    {
        InitScriptHandles();

        transform.GetComponent<Button>().onClick.AddListener(ButtonAction);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitScriptHandles()
    {
        sendUI = GameObject.Find("Canvas").GetComponent<SendUI>();
        if (null == sendUI)
        {
            Debug.LogError("Cannot find SendUI object.");
        }
    }

    private void ButtonAction()
    {
        string imageName = transform.GetComponent<Image>().sprite.name;

        sendUI.AddEmojiInText(dictOfFileName2Unicode[imageName]);
    }
}
