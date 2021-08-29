using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xiyiAnimTest : MonoBehaviour {

	private xiyiAnimation xiyi_Anim;
	private Animator anim;
	bool notchange=false;
	float timer;
	// Use this for initialization
	void Start () 
	{
		xiyi_Anim = new xiyiAnimation ();
		anim = GetComponent<Animator> ();
		xiyi_Anim.animator = anim;
		xiyi_Anim.Init ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.Log (xiyi_Anim.id);
		if (!notchange) {
			
			xiyi_Anim.play ("Sleep");
			if (Input.GetKey (KeyCode.A)) {
				xiyi_Anim.play ("Walk");
				Debug.Log ("walk");
			}


			if (Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.W))
				xiyi_Anim.play ("Run");
			if (Input.GetKeyDown (KeyCode.Q)) 
			{
				xiyi_Anim.play ("underAttack");
				notchange = true;
			}
			if (Input.GetKeyDown (KeyCode.E)) 
			{
				xiyi_Anim.play ("Attack");
				notchange = true;
			}
		} else {
			timer += Time.deltaTime;
			if (timer > 1.0f) {
				timer = 0.0f;
				notchange = false;
			}
		}
	}
}
