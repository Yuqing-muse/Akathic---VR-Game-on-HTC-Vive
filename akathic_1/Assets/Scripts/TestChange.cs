using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChange : MonoBehaviour 
{
    public GameObject[] fir;
    public GameObject[] sec;
	// Use this for initialization
	void Start ()
    {
        Invoke("change",10.0f);
	}
    void change() 
    {
        for (int i = 0; i < fir.Length; i++) 
        {
            fir[i].SetActive(false);
        }
        for (int i = 0; i < sec.Length; i++) 
        {
            sec[i].SetActive(true);
        }
    }
	// Update is called once per frame
	void Update () 
    {
		
	}
}
