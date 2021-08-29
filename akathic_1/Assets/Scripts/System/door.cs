using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour 
{
    public GameObject key;
    public float maxDis;
    public float openspeed;
    private HingeJoint hj;
    public GameObject sample;
	// Use this for initialization
	void Start () 
    {
        hj = GetComponent<HingeJoint>();
	}

    void open() 
    {
        JointMotor jm = new JointMotor();
        jm.targetVelocity = openspeed;
        jm.force = 5;
        hj.motor = jm;
        hj.useMotor = true;
        Destroy(this);
    }
	// Update is called once per frame
	void Update () 
    {
        float realDis = Vector3.Distance(key.transform.position, sample.gameObject.transform.position);
        Debug.Log(realDis);
	    if(realDis<maxDis&&key.activeSelf)
        {
            open();
        }
	}
}
