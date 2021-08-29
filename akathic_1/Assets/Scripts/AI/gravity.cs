using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravity : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//RaycastHit hit;
		if(!Physics.Raycast(this.transform.position,new Vector3(0,-1,0),0.2f))
		{
			GetComponent<CharacterController> ().Move (new Vector3(0,-1,0)*0.5f*Time.deltaTime);
		}
	}
}
