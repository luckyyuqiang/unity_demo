using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MyResources myResources;
    private Sdk sdk;

    private Image messageButtonImage;
    private Transform avatarImage;
    private Transform nameText;
    private Transform timeText;
    private Transform contentText;
    private Transform actionButton;

    private bool isRoomMessage;
    private bool isSendMessage;

    private void Awake()
    {
        myResources         = GameObject.Find("Canvas").GetComponent<MyResources>();
        sdk                 = GameObject.Find("Canvas").GetComponent<Sdk>();

        messageButtonImage  = transform.GetComponent<Image>();

        avatarImage         = transform.GetChild(0).GetChild(0);
        nameText            = transform.GetChild(1).GetChild(0).GetChild(0);
        timeText            = transform.GetChild(1).GetChild(0).GetChild(1);
        contentText         = transform.GetChild(1).GetChild(1).GetChild(0);

        actionButton        = transform.GetChild(2);

        isRoomMessage       = false;
        isSendMessage       = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        messageButtonImage.color = new Color(messageButtonImage.color.r, messageButtonImage.color.g, messageButtonImage.color.b, 255);

        if (ShowActionButton())
        {
            actionButton.gameObject.SetActive(true);
        }
        else
        {
            actionButton.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        messageButtonImage.color = new Color(messageButtonImage.color.r, messageButtonImage.color.g, messageButtonImage.color.b, 0);
        actionButton.gameObject.SetActive(false);
    }

    bool ShowActionButton()
    {
        return (true == isRoomMessage && false == isSendMessage);
    }

    public void ReceiveAMessage(Notice notice)
    {
        List<string> dst = notice.dst_;

        string from = notice.params_[0].ToString();
        string to = notice.params_[1].ToString();
        string content = notice.params_[2].ToString();
        long ts = long.Parse(notice.params_[3].ToString());

        avatarImage.GetComponent<Image>().sprite = myResources.GetAvatarSprite(Tools.GetAvatarIndex(from));
        nameText.GetComponent<TextMeshProUGUI>().text = from;
        timeText.GetComponent<TextMeshProUGUI>().text = Tools.GetTimeHHMMFromTs(ts);
        contentText.GetComponent<TextMeshProUGUI>().text = content;

        if (dst[0].CompareTo("Chat") != 0) isRoomMessage = true;
        if (from.CompareTo(sdk.GetUserName()) == 0)
        {
            isSendMessage = true;
        }
        else
        {
            isSendMessage = false;
        }
    }

    public void SendAMessage(Notice notice)
    {
        ReceiveAMessage(notice);
        isSendMessage = true;
    }

    public void LoadAContactWithMsg(Notice notice)
    {
        ReceiveAMessage(notice);

        string from = notice.params_[0].ToString();
        if (from.CompareTo(sdk.GetUserName()) == 0)
        {
            isSendMessage = true;
        }
        else
        {
            isSendMessage = false;
        }
    }
}
