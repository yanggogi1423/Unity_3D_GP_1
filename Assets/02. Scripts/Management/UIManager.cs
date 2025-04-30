using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    public GameObject backGround;
    public GameObject buttonPanel;

    private UnityAction action;

    private void Start()
    {
        //   With Unity Action
        action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);
        
        //  With Anonymous Function
        optionButton.onClick.AddListener(delegate { OnButtonClick(optionButton.name); });
        
        //  With Lambda-Expression
        shopButton.onClick.AddListener(()=>OnButtonClick(shopButton.name));
    }

    private void OnButtonClick(string str)
    {
        Debug.Log($"Click Button : {str}");

        if (str == "Start Button")
        {
            backGround.SetActive(false);
            buttonPanel.SetActive(false);
        }
    }
}
