using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour {
    public GameObject MoveGameObject;
    public Transform Terget;
    public float speed = 10;
    int time = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        time++;
        if (time > 10) time--;
        if (time < -10) time++;
        transform.Rotate(Vector3.up, (float)(time * 0.01), Space.World);
	}
}
