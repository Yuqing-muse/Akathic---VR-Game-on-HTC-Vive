using UnityEngine;
using System.Collections;

public class BloodTrigger : MonoBehaviour 
{
	public GameObject Blood;
	public string TagOfAttackPoint;
	public GameObject samplePoint;
	public float scale;
	// Use this for initialization
	void Start () 
	{
	
	}
	void OnTriggerEnter(Collider other)
	{
		
		if (other.transform.tag == TagOfAttackPoint) 
		{
            Vector3 v = other.transform.position;
			GameObject g = Instantiate (Blood,other.bounds.center+new Vector3(2.5f,0,0),Quaternion.identity) as GameObject;
		//	g.transform.localScale *= Vector3.SqrMagnitude(samplePoint.transform.localScale) * scale;
		}
	}
	// Update is called once per frame
	void Update () 
	{
	}
}
