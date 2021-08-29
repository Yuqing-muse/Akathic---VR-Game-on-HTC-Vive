using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour 
{

	public float time = 0.5f;
	// Use this for initialization
	void Start ()
	{
		Invoke ("DestroyThisGameObject",time);	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void DestroyThisGameObject()
	{
		Destroy (this.gameObject);
	}
}
