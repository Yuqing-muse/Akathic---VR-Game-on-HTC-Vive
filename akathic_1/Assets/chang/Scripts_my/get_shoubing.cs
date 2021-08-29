using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class get_shoubing : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;
    private Transform begin;
    private Vector3 v;
   
    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
       

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void FixedUpdate()
    {
        //用来获取手柄的移动速度，用来对应不同的攻击伤害量
        device.GetPose();
        v = device.velocity;
        Debug.Log(v);
    }
}
