using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatSwitchButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string friendName;
    private Transform textTranform;
    private Color myColor;

    private void Awake()
    {
        textTranform = transform.GetChild(0);
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);

        Switch.Hide(transform);
    }

    // Update is called once per frame
    void Update()
    {
        CheckMeClicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        myColor = textTranform.GetComponent<TextMeshProUGUI>().color;
        textTranform.GetComponent<TextMeshProUGUI>().color = new Color(0, 255, 240);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textTranform.GetComponent<TextMeshProUGUI>().color = myColor;
    }

    void SendNoticeForSwitch()
    {
        List<string> dst = new List<string> { "Chat", friendName };
        Notice notice = new Notice("SwitchTo", null, dst);
        SendMessageUpwards("UIManagerNotice", notice, SendMessageOptions.DontRequireReceiver);
    }

    void ButtonClick()
    {
        SendNoticeForSwitch();
        Switch.Hide(transform);
    }

    void CheckMeClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                Switch.Hide(transform);
                return;
            }

            if (EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.transform != transform)
            {
                Switch.Hide(transform);
            }
        }
    }

    public void Show()
    {
        Switch.Show(transform);
    }

    public void SetFriendName(string name)
    {
        friendName = name;
    }
}
