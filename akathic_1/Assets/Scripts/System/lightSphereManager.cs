using UnityEngine;
using System.Collections;

public class lightSphereManager : MonoBehaviour {

	[HideInInspector]
	public int index=0;
	public GameObject notbroken;
	public GameObject broken;
	[HideInInspector]
	public qte_trigger qteManager;
	private VRInput VRInputManager;
	// Use this for initialization
	void Start () 
	{
		notbroken.SetActive (true);
		broken.SetActive (false);
		VRInputManager = GameObject.FindGameObjectWithTag ("VRInputSystem").GetComponent<VRInput>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (!qteManager.is_broken [index])
		{
			if (other.transform.tag == "Weapon") 
			{
				qteManager.attack (index, Vector3.SqrMagnitude(VRInputManager.rightVelocity*15.0f));
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (!qteManager.is_broken [index])
		{
			notbroken.SetActive (true);
			broken.SetActive (false);
		}
		else
		{
			notbroken.SetActive (false);
			broken.SetActive (true);
		}
	}
}
