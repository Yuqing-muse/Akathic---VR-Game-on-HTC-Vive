using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alwaysForwardCamera : MonoBehaviour 
{
	public Camera camera_this;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.gameObject.transform.up = -(camera_this.transform.forward);
	}
}
