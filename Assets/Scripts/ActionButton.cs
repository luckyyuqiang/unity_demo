using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MyResources myResources;

    private Transform exitButtonTransform;
    private Transform actionImageTransform;

    private void Awake()
    {
        myResources = GameObject.Find("Canvas").GetComponent<MyResources>();
        exitButtonTransform = GameObject.Find("Canvas/BackGroundPanel/ExitButton").transform;
        actionImageTransform = transform.GetChild(0);
        actionImageTransform.GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetComponent<Image>().color = new Color(0, 255, 240, 255);
        actionImageTransform.GetComponent<Image>().sprite = myResources.more2Sprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetComponent<Image>().color = new Color(0, 255, 240, 0);
        actionImageTransform.GetComponent<Image>().sprite = myResources.moreSprite;
    }

    void ButtonClick()
    {
        Switch.Show(exitButtonTransform);
    }
}
