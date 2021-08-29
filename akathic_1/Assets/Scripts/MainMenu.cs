using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    public GameObject SettingPanel;
    public GameObject MainPanel;
    public static int isshow = 0;
    double speed = 3.0;
    double time1 = 0;
    double time2 = 0;
    private float alpha1 = 0.0f;
    private float alphaSpeed = 0.05f;

    private CanvasGroup cg;
    void Start()
    {
        List<string> btnsName = new List<string>();
        cg = this.transform.GetComponent<CanvasGroup>();
        btnsName.Add("NewGameButton");
        btnsName.Add("LoadButton");
        btnsName.Add("SettingButton");

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
            case "NewGameButton":
                break;
            case "LoadButton":
                break;
            case "SettingButton":
                isshow = 1;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame  
    void Update()
    {

        if (isshow==1)
        {
           
            if (time1 <= 10)
            {
                time1+=0.5;

                transform.Translate(-(float)(speed * time1 / 2), 0, 0);
                Hide();
                if (alpha1 != cg.alpha)
                {
                    cg.alpha = Mathf.Lerp(cg.alpha, alpha1, (float)(alphaSpeed * time1));
                    if (Mathf.Abs(alpha1 - cg.alpha) <= 0.01)
                    {
                        cg.alpha = alpha1;
                    }
                }

                                
            }
            if (time1 > 10)
            {
                Hide();
                SettingPanel.SetActive(true);
                isshow = 0;
                time1 = 0;
            }
        }
        else if (isshow == 2)
        {

            SettingPanel.SetActive(false);
            if (time2<=10)
            {
                time2 += 0.5;

                transform.Translate((float)(speed * time2 / 2), 0, 0);
                Show();
                if (alpha1 != cg.alpha)
                {
                    cg.alpha = Mathf.Lerp(cg.alpha, alpha1, (float)(alphaSpeed * time2));
                    if (Mathf.Abs(alpha1 - cg.alpha) <= 0.01)
                    {
                        cg.alpha = alpha1;
                    }
                }


            }
            if (time2 > 10)
            {
                 Show();
                SettingPanel.SetActive(false);
                isshow = 0;
                time2 = 0;
            }
        }
    }
    public void Show()
    {
        alpha1 = 1;

        cg.blocksRaycasts = true;//可以和该UI对象交互  
    }

    public void Hide()
    {
        alpha1 = 0;

        cg.blocksRaycasts = false;//不可以和该UI对象交互  
    }
}