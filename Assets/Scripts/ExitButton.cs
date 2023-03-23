using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MyResources myResources;
    private Color myColor;
    private Transform exitImageTransform;
    private Transform exitTextTransform;

    private void Awake()
    {
        myResources = GameObject.Find("Canvas").GetComponent<MyResources>();
        exitImageTransform = transform.GetChild(0);
        exitTextTransform = transform.GetChild(1);
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        ShowOrNot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        myColor = exitTextTransform.GetComponent<TextMeshProUGUI>().color;
        exitTextTransform.GetComponent<TextMeshProUGUI>().color = new Color(0, 255, 240);
        exitImageTransform.GetComponent<Image>().sprite = myResources.exitSelectSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exitTextTransform.GetComponent<TextMeshProUGUI>().color = myColor;
        exitImageTransform.GetComponent<Image>().sprite = myResources.exitSprite;
    }

    void ButtonClick()
    {
        transform.gameObject.SetActive(false);
        ExitGame();
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowOrNot()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            if (EventSystem.current.currentSelectedGameObject.transform == transform)
            {
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
    }
}
