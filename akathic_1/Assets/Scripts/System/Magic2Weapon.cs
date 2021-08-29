using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.VR;
public class Magic2Weapon : MonoBehaviour 
{
    public VRInput vrinputsystem;

    private int counter = 0;
    #region MagicSystem
    public VRGestureRig vrgesture;
    public GameObject[] allMagic;
    public Animator animator;
    #endregion

    #region WeaponSystem
    public WeaponSystem weaponsystem;
    #endregion
    // Use this for initialization

	void Start () 
    {
		
	}
    private void Magic2WeaponSystem() 
    {
        for (int i = 0; i < allMagic.Length; i++) 
        {
            allMagic[i].SetActive(false);
        }
        vrgesture.enabled = false;
        weaponsystem.IsWeaponRun = true;
        animator.SetBool("IsOpen",false);
    }
    private void Weapon2MagicSystem() 
    {
        animator.SetBool("IsOpen",true);
        for (int i = 0; i < allMagic.Length; i++)
        {
            allMagic[i].SetActive(true);
        }
        vrgesture.enabled = true;
        weaponsystem.IsWeaponRun = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (vrinputsystem.gripDown_left) 
        {
            if (counter % 2 == 0)
            {
                Weapon2MagicSystem();
                counter++;
            }
            else 
            {
                Magic2WeaponSystem();
                counter++;
            }
        }
	}
}
