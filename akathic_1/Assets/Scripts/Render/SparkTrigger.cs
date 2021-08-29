using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SparkTrigger : MonoBehaviour
{
	public GameObject Spark;
	[HideInInspector]
	public float velocity;
	public float maxVelocity;
	private float minVelocityRequired;
	public GameObject samplePoint;
	public float scale;
	// Use this for initialization
	void Start () 
	{
		velocity = maxVelocity;
	}
	void OnTriggerStay(Collider collision)
	{
		if(velocity>minVelocityRequired)
		{
			if (collision.transform.tag == "Metal") 
			{
				GameObject g = Instantiate (Spark, samplePoint.transform.position, Quaternion.identity) as GameObject;
				g.GetComponent<ParticleSystem> ().startSize *= velocity / maxVelocity*scale;
				g.transform.localScale *= scale;
			}
		}
	}
	// Update is called once per frame
	void Update ()
	{
		GetVelocity ();
	}
	void GetVelocity()
	{
		
	}	
}
