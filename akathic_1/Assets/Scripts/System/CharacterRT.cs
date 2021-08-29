using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CharacterRT : MonoBehaviour 
{

	public int hp;

	public float fireRate;

	public int defence;

	private int temphp;

	private BloodBank bbank;

	public Camera maincamera;

	public float dyingtimer;

	private bool dying_;

	public database db;
	// Use this for initialization
	void Start() 
	{
		temphp = hp;
		if (db.fileExist)
		{
			this.gameObject.transform.position = db.md.pos;
			temphp = db.md.hp;
		}
		bbank = GameObject.FindGameObjectWithTag ("BloodBank").GetComponent<BloodBank>();
		dying_ = false;
	}


	// Update is called once per frame
	void Update () 
	{
		if (dying_)
		{
			dyingtimer += Time.deltaTime;
			if (dyingtimer >= 60.0f) 
			{
				dying_ = false;
				dyingtimer = 0.0f;
				maincamera.GetComponent<Bloom> ().enabled = false;
				maincamera.GetComponent<Grayscale> ().enabled = false;
				temphp = hp;
				db.changehp (temphp);
				bbank.close ();
			}
		}	
	}

	public void beattack(int value)
	{
		temphp -= value;
		bbank.BeTriggered ();
		if (temphp <= hp * 0.3f)
			bbank.alwaysShow ((hp*0.3f-temphp)/(hp*0.3f));
		if (temphp <= 0)
			dying ();
		db.changehp (temphp);
	}

	void dying()
	{
		maincamera.GetComponent<Bloom> ().enabled = true;
		maincamera.GetComponent<Grayscale> ().enabled = true;
		dying_ = true;
	}

}
