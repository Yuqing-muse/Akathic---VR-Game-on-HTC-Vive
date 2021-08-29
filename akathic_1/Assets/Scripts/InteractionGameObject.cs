using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionGameObject : MonoBehaviour
{
	public GameObject player;
	public  VRInput vrinputsystem;
	public WeaponSystem ws;
	public bool isWeapon;
	public GameObject gameobjectID;
    private float timer;
    private bool trigger;
	// Use this for initialization
	void Start () 
	{
	//	player = GameObject.FindGameObjectWithTag ("player");
	//	vrinputsystem = GameObject.FindGameObjectWithTag ("VRInputSystem");
	}
	// Update is called once per frame
	void Update () 
	{
		this.transform.Rotate (new Vector3(0,1,0));
		if (Vector3.Distance (player.transform.position,this.transform.position)<8.0f)
		{
			if (vrinputsystem.TriggerDown_left&&!trigger) 
			{
                trigger = true;
				effect ();
			}
		}
        if (trigger) 
        {
            timer += Time.deltaTime;
            if (timer > 0.5f) 
            {
                timer = 0;
                trigger = false;
            }
        }
	}
	void effect()
	{
		if (isWeapon) 
		{
			ws.AddWeapon (gameobjectID);
		}
		else
		{
			gameobjectID.SetActive (true);
		}
        Debug.Log("effectInteraction");
		Destroy (this.gameObject);
	}
}