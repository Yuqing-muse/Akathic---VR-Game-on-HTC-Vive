using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using VRTK.Examples;
public class BagUIControl : MonoBehaviour {

	// Use this for initialization

    public VRInput vrinputsystem;
    private int counter=0;
    public WeaponSystem weaponsystem;
    public GameObject[] ui_all;
    public VRTK_StraightPointerRenderer vrtk_spr;
    public VRTK_ControllerUIPointerEvents_ListenerExample vrtk_cuele;
    public VRTK_Pointer vrtk_pt;
    public VRTK_UIPointer vrtk_uip;
    public VRTK_InteractTouch vrtk_it;
    public GameObject[] allGameObject;
    public Sprite[] allTexture;
    public string[] allString;
    public GameObject[] allButton;
    public Image MainImage;
    public Text MainString;
    public database db;
	void Start () 
    {
		
	}
    void openUI()
    {
        weaponsystem.IsWeaponRun = false;
        vrtk_spr.enabled = true;
        vrtk_cuele.enabled = true;
        vrtk_pt.enabled = true;
        vrtk_uip.enabled = true;
        vrtk_it.enabled = true;
        for (int i = 0; i < ui_all.Length;i++ ) 
        {
            ui_all[i].SetActive(true);
        }
        for (int i = 0; i < db.md.itemAll+2;i++ )
        {
            if (i < 2)
                allButton[i].GetComponent<Image>().sprite = allTexture[i];
            else
                allButton[i].GetComponent<Image>().sprite = allTexture[db.md.items[i-2]];
        }
    }

    public  GameObject FindGameObjectByID(int id) 
    {
        return allGameObject[id];
    }
    void closeUI() 
    {
        weaponsystem.IsWeaponRun = true;
        for (int i = 0; i < ui_all.Length; i++)
        {
            ui_all[i].SetActive(false);
        }
        vrtk_spr.enabled = false;
        vrtk_cuele.enabled = false;
        vrtk_pt.enabled = false;
        vrtk_uip.enabled = false;
        vrtk_it.enabled = false;

        for (int i = 0; i < db.md.itemAll+2; i++)
        {
            allButton[i].GetComponent<Image>().sprite = null;
        }
    }
	// Update is called once per frame
	void Update () 
    {
	    if(vrinputsystem.gripDown)
        {
            if (counter % 2 == 0)
            {
                openUI();
            }
            else 
            {
                closeUI();
            }
            counter++;
        }
	}
    public void select(int id) 
    {
        int realID;
        if (id < 2) 
        {
           realID = id;
        }
        else
        {
            realID = db.md.items[id - 2];
        }
        MainImage.sprite = allTexture[realID];
        MainString.text = allString[realID];
    }
}
