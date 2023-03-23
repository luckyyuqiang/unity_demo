using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintBubbleButton : MonoBehaviour
{
    private int hintNum;

    private static string bubble1 = "Icons/bubble1_3x";
    private static string bubble2 = "Icons/bubble2_3x";
    private static string bubble3 = "Icons/bubble3_3x";

    private Sprite sprite1;
    private Sprite sprite2;
    private Sprite sprite3;

    private void Awake()
    {
        hintNum = 0;

        sprite1 = Resources.Load(bubble1, typeof(Sprite)) as Sprite;
        sprite2 = Resources.Load(bubble2, typeof(Sprite)) as Sprite;
        sprite3 = Resources.Load(bubble3, typeof(Sprite)) as Sprite;

        ShowHide();
    }

    public int HintNum()
    {
        return hintNum;
    }

    public void Add(int n)
    {
        hintNum += n;
        ShowHide();
    }

    public void Sub(int n)
    {
        hintNum -= n;
        if (hintNum < 0) hintNum = 0;
        ShowHide();
    }

    public void Set(int n)
    {
        hintNum = n;
        ShowHide();
    }

    public void ShowHide()
    {
        if (hintNum <= 0)
        {
            transform.gameObject.SetActive(false);
            return;
        }

        if (hintNum > 0 && hintNum <= 9)
        {
            transform.gameObject.SetActive(true);
            transform.GetComponent<Image>().sprite = sprite1;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hintNum.ToString();
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);
            return;
        }

        if (hintNum > 9 && hintNum <= 99)
        {
            transform.gameObject.SetActive(true);
            transform.GetComponent<Image>().sprite = sprite2;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hintNum.ToString();
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(22, 16);
            return;
        }

        if (hintNum > 99)
        {
            transform.gameObject.SetActive(true);
            transform.GetComponent<Image>().sprite = sprite3;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "99+";
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(29, 16);
            return;
        }
    }
}
