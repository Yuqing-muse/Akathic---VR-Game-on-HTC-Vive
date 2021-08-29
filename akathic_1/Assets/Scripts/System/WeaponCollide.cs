using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollide : MonoBehaviour {

    public bool isAxe;
    public BattleSystem battlesystem;
    public WeaponSystem weaponsystem;
	// Use this for initialization
	void Start () 
    {
		
	}
    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "AI") 
        {
            weaponsystem.attack(other.gameObject);

      
        }
        if (isAxe) 
        {
            if (other.tag == "Plane") 
            {
        //        weaponsystem.WeaponJNs[1].TriggerWithPlane(other.transform.position.y);
            }
        }
    }
	// Update is called once per frame
	void Update ()
    {
		
	}
}
