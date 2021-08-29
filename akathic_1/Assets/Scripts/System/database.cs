using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class database : MonoBehaviour
{
	[HideInInspector]
	public MyData md;
	DirectStrategy<MyData> dstrategy;
	XMLDataTranslate<MyData> datatranslate;
	DataManager<MyData> dm;
	[HideInInspector]
	public bool fileExist=false;
	public GameObject player;
	private float gameTime;
	// Use this for initialization
	void Start () 
	{
	//	player = GameObject.FindGameObjectWithTag ("player");
		md = new MyData ();
		dstrategy = new DirectStrategy<MyData> ();
		datatranslate = new XMLDataTranslate<MyData> (MyData.StringtoData,MyData.dataToString);
		dm = new DataManager<MyData> (dstrategy,datatranslate,Application.dataPath+"/1.xml");
		if (File.Exists(Application.dataPath+"/1.xml")) 
		{
			fileExist = true;
			md = dm.Read (1);
            player.transform.position = md.pos;
		}
		gameTime = (float)md.gameTime;
		//selftest ();
	}
	void selftest()
	{
		md.gameTime = 1000;
		md.hp = 400;
		md.weaponIndex = 2;
		md.pos = new Vector3 (2.2f,3.1f,123.9f);
		md.bossDied [0] = true;
		md.itemAll = 1;
		md.items [0] = 4;
		Exit ();
	}
	public void Exit()
	{
		md.gameTime = (int)gameTime;
		dm.Write (1, md);
	}
	public void changehp(int hp)
	{
		md.hp = hp;
	}
	public void changeweapon(int weapon)
	{
		md.weaponIndex = weapon;
	}
	public void changeBoss(int bossid)
	{
		md.bossDied [bossid] = true;
	}
	public void changeitem(int id)
	{
		md.items [md.itemAll] = id;
		md.itemAll++;
	}
//	public void 
	// Update is called once per frame
	void Update ()
	{
		gameTime += Time.deltaTime;
		md.pos = player.transform.position;
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Exit();
        }
	}
}
