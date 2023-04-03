using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MyResources myResources;
    private Sdk sdk;
    private Color myColor;
    private Transform exitImageTransform;
    private Transform exitTextTransform;

    private bool needExit = false;
    private float timer = 0f;

    private void Awake()
    {
        myResources = GameObject.Find("Canvas").GetComponent<MyResources>();
        sdk = GameObject.Find("Canvas").GetComponent<Sdk>();
        exitImageTransform = transform.GetChild(0);
        exitTextTransform = transform.GetChild(1);
        transform.GetComponent<Button>().onClick.AddListener(ButtonClick);
        Switch.Hide(transform);
    }

    // Update is called once per frame
    void Update()
    {
        ShowOrNot();
        CheckExit();
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
        Switch.Hide(transform);
        sdk.LeaveRooms();
        needExit = true;
    }

    void CheckExit()
    {
        // Check can exit or not
        if (needExit)
        {
            timer += Time.deltaTime;
            if (timer >= 0.05f)
            {
                ExitGame();
                timer = 0f;
            }
        }
    }

    void ExitGame()
    {
        // Wait sdk exit complete
        if (!sdk.SdkExitComplete()) return;
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
                Switch.Hide(transform);
                return;
            }
            if  (EventSystem.current.currentSelectedGameObject.transform.name.CompareTo("ActionButton") == 0 ||
                 EventSystem.current.currentSelectedGameObject.transform.name.CompareTo("ActionImage") == 0)
            {
                Switch.Show(transform);
            }
            else
            {
                Switch.Hide(transform);
            }
        }
    }
}
