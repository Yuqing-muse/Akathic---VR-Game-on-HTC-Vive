using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGameObject : MonoBehaviour 
{

	// Use this for initialization
    private bool isOnhand=true;
    public VRInput vrinputsystem;
    public GameObject sample;
	void Start () 
    {
        Invoke("DestroyThis",30.0f);
	}
    void DestroyThis() 
    {
        Destroy(this.gameObject);
    }
	// Update is called once per frame
	void Update () 
    {
        if (isOnhand) 
        {
            if (!vrinputsystem.TriggerDown_left) 
            {
                isOnhand = false;
                GameObject g=Instantiate(this.gameObject) as GameObject;
                Destroy(this.gameObject);
                Vector3 v = vrinputsystem.leftVelocity;
                v.y = 0;
                v.Normalize();
               g.GetComponent<Rigidbody>().velocity = v*3.0f;
                g.GetComponent<Rigidbody>().useGravity = true;
                //this.transform.SetParent(null);
            }
        }
	}
}
