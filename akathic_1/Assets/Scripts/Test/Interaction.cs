using UnityEngine;
using System.Collections;

public class Interaction : MonoBehaviour 
{
	public GameObject sward;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		sward.transform.Rotate (new Vector3(Time.deltaTime*120,0.0f,0.0f));
		if(Input.GetKey(KeyCode.W))
		{
			sward.transform.Translate (sward.transform.forward*Time.deltaTime*5);
		}
		if(Input.GetKey(KeyCode.A))
		{
			sward.transform.Translate (-sward.transform.right*Time.deltaTime*5);
		}
		if(Input.GetKey(KeyCode.S))
		{
			sward.transform.Translate (-sward.transform.forward*Time.deltaTime*5);
		}
		if(Input.GetKey(KeyCode.D))
		{
			sward.transform.Translate (sward.transform.right*Time.deltaTime*5);
		}
	}
}
