using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class test1 : MonoBehaviour {

	// Use this for initialization
	MyData _md;
	DirectStrategy<MyData> dstrategy;
	XMLDataTranslate<MyData> datatranslate;
	DataManager<MyData> _dm;
	void Start () 
	{
		_md = new MyData ();
		dstrategy = new DirectStrategy<MyData> ();
		datatranslate = new XMLDataTranslate<MyData> (MyData.StringtoData,MyData.dataToString);
		_dm = new DataManager<MyData> (dstrategy,datatranslate,Application.dataPath+"/1.xml");
		/*
		_md.exp = 100;
		_md.favorRate = 20;
		_md.rank = 5;
		_md.modelstate = 2;
		_md.modelMessage = 1;
		_md.items=new int[]{0,1,2};
		_dm.Write (1,_md);
		*/
		_md=_dm.Read (1);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

}
