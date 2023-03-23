using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmojiPanel : MonoBehaviour
{
    public  Texture2D defaultCursor;

    private Transform scrollBarTransform;
    private Transform viewPortTransform;
    private Transform contentTransform;

    private void Awake()
    {
        scrollBarTransform = transform.GetChild(1).GetChild(1);
        viewPortTransform  = transform.GetChild(1).GetChild(0);
        viewPortTransform.GetComponent<Button>().onClick.AddListener(ButtonClick);
        contentTransform = transform.GetChild(1).GetChild(0).GetChild(0);
        Switch.Hide(transform);
    }

    // Update is called once per frame
    void Update()
    {
        ShowOrNot();
    }

    void ButtonClick()
    {
        Debug.Log("viewport is clicked!!!");
    }

    public void ShowEmojiPanel()
    {
        Switch.Show(transform);
    }

    public void HideEmojiPanel()
    {
        Switch.Hide(transform);
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

            if (EventSystem.current.currentSelectedGameObject.transform == transform ||
                EventSystem.current.currentSelectedGameObject.transform == viewPortTransform ||
                EventSystem.current.currentSelectedGameObject.transform == scrollBarTransform)
            {
                Switch.Show(transform);
            }
            else
            {
                // Click SingleEmojiButton will trigger to hide EmojiPanel too.
                // So here no need to hide EmojiPanel when clicked object is SingleEmojiButton
                if (EventSystem.current.currentSelectedGameObject.transform.tag.CompareTo("SingleEmoji") != 0)
                {
                    Switch.Hide(transform);
                }
            }
        }
    }
}
