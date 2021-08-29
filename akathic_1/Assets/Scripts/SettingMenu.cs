using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SettingMenu : MonoBehaviour {

    // Use this for initialization 
    public GameObject SettingPanel;
    public GameObject MainPanel;
    void Start()
    {
        List<string> btnsName = new List<string>();
        btnsName.Add("ComfirmButton");
        btnsName.Add("ReturnButton");

        foreach (string btnName in btnsName)
        {
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                this.OnClick(btnObj);
            });
        }
    }

    public void OnClick(GameObject sender)
    {
        switch (sender.name)
        {
            case "ComfirmButton":
                break;
            case "ReturnButton":
                MainMenu.isshow = 2;
                break;
            default:
                break;
        }
    }
}
