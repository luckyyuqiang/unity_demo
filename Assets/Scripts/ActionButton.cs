using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private Transform exitButtonTransform;

    private void Awake()
    {
        exitButtonTransform = GameObject.Find("Canvas/BackGroundPanel/ExitButton").transform;
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ButtonClick()
    {
        exitButtonTransform.gameObject.SetActive(true);
    }
}
