using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReactionSelect : MonoBehaviour
{
    // Script objects
    private WorldUI wUI;

    private void Awake()
    {
        InitScriptHandles();

        transform.GetComponent<Button>().onClick.AddListener(ReactionSelectButtonAction);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitScriptHandles()
    {
        wUI = GameObject.Find("Canvas").GetComponent<WorldUI>();
        if (null == wUI)
        {
            Debug.LogError("Cannot find WorldUI object.");
        }
    }

    public void ReactionSelectButtonAction()
    {
        string imageName = transform.GetComponent<Image>().sprite.name;

        wUI.AddReaction(imageName);

        Transform trans = EventSystem.current.currentSelectedGameObject.transform;
        trans.parent.gameObject.SetActive(false);
    }
}
