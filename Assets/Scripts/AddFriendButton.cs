using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddFriendButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Sdk sdk;
    private string friendName;
    private Transform textTranform;
    private Color myColor;

    private void Awake()
    {
        sdk = GameObject.Find("Canvas").GetComponent<Sdk>();
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
        textTranform = transform.GetChild(0);

        Switch.Hide(transform);
    }

    // Update is called once per frame
    void Update()
    {
        CheckMeClicked();
    }

    void ButtonClick()
    {
        sdk.AddFriend(friendName);
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
                EventSystem.current.currentSelectedGameObject.transform != transform )
            {
                Switch.Hide(transform);
            }
        }
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

    public void Show()
    {
        Switch.Show(transform);
    }

    public void SetFriendName(string name)
    {
        friendName = name;
    }
}
