using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testQTE : MonoBehaviour
{
	qte_trigger qt;
	int i=0;
	// Use this for initialization
	void Start ()
	{
		qt = GetComponent<qte_trigger> ();
		//qt.qtetrigger ();
		Invoke("call",5.0f);
	}
	void call()
	{
		qt.qtetrigger ();
	}
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			qt.attack (i, 100);
			i++;
		}
	}
}
