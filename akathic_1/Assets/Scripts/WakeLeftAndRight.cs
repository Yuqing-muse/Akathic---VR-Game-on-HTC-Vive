using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeLeftAndRight : MonoBehaviour {

	// Use this for initialization
    public GameObject left;
    public GameObject right;
	void Start () {
        left.SetActive(true);
        right.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
