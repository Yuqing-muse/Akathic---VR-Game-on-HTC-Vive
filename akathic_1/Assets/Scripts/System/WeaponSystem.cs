using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using WeaponJN;
public class WeaponSystem : MonoBehaviour
{
	#region WeaponSystem
	public GameObject[] WeaponModel;
	[HideInInspector]
	public WeaponJNSystem[] WeaponJNs;
	[HideInInspector]
	public AIDebuff[] WeaponBuff;
	[HideInInspector]
	public float[] AIDeBuffTime;
	[HideInInspector]
	public int[] attackPower;
	private int tempIndex=0;
	VRInput vrinputsystem;
	BattleSystem bs;
//	public GameObject sample;
//	public GameObject father;
	private GameObject temp;
	public database db;
    public BagUIControl BUIControl;
	#endregion

	#region SwordJN
	public GameObject SwordGentle;
    public GameObject SwordGentlePos;
	public GameObject SwordLight;
    public GameObject SwordLightPos;
	#endregion

	#region AxeJN
	public GameObject AxeGentle;
    public GameObject AxeGentlePos;
	public GameObject AxeLight;
    public GameObject AxeLightPos;
	#endregion

    public bool IsWeaponRun = true;
	// Use this for initialization
    private bool changeStop = true;

    public GameObject sample;
	void Start () 
	{
		if (db.fileExist)
			tempIndex = db.md.weaponIndex;
		vrinputsystem = GameObject.FindGameObjectWithTag ("VRInputSystem").GetComponent<VRInput>();
		bs = GameObject.FindGameObjectWithTag ("BattleSystem").GetComponent<BattleSystem>();
		int length = WeaponModel.Length+db.md.itemAll;
		GameObject[] WeaponModelNew = new GameObject[length];
		for (int i = 0; i < length; i++) 
		{
			if (i < WeaponModel.Length)
				WeaponModelNew [i] = WeaponModel [i];
			else
                WeaponModelNew[i] = BUIControl.FindGameObjectByID(db.md.items[i - WeaponModel.Length]);
		}
		WeaponJNs = new WeaponJNSystem[length];
		WeaponJNs [0] = new SwordJNSystem ();
		WeaponJNs [0].SetEffect (SwordGentle,SwordLight,SwordGentlePos,SwordLightPos);
		WeaponJNs [0].weapon = this.gameObject;
		WeaponJNs [0].coolingTime = 180.0f;
		WeaponJNs [0].maxPower = 100.0f;
		WeaponJNs [0].powerVelocity = 20.0f;
		WeaponJNs [1] = new AxeJNSystem ();
		WeaponJNs [1].SetEffect (AxeGentle,AxeLight,AxeGentlePos,AxeLightPos);
		WeaponJNs [1].weapon = this.gameObject;
		WeaponJNs [1].coolingTime = 180.0f;
		WeaponJNs [1].maxPower = 100.0f;
		WeaponJNs [1].powerVelocity = 20.0f;
		WeaponBuff = new AIDebuff[length];
		WeaponBuff [0] = AIDebuff.FIRE;
		WeaponBuff [1] = AIDebuff.ICE;
		AIDeBuffTime = new float[length];
		AIDeBuffTime [0] = 6;
		AIDeBuffTime [1] = 4;
		attackPower = new int[length];
		for(int i=0;i<length;i++)
		{
			attackPower [i] = 100;
		}
        WeaponModel[tempIndex].SetActive(true);
       // Debug.Log(WeaponModel[tempIndex].name);
	}

	public void AddWeapon(GameObject g)
	{
		db.changeitem (int.Parse(g.name));
		int length = WeaponModel.Length+db.md.itemAll;
		GameObject[] WeaponModelNew = new GameObject[length];
		for (int i = 0; i < length; i++) 
		{
			if (i < WeaponModel.Length)
				WeaponModelNew [i] = WeaponModel [i];
			else
                WeaponModelNew[i] = BUIControl.FindGameObjectByID(db.md.items[i - WeaponModel.Length]);
		}
        WeaponModel = WeaponModelNew;
		WeaponJNs = new WeaponJNSystem[length];
		WeaponJNs [0] = new SwordJNSystem ();
		WeaponJNs [0].SetEffect (SwordGentle,SwordLight,SwordGentlePos,SwordLightPos);
		WeaponJNs [0].weapon = this.gameObject;
		WeaponJNs [0].coolingTime = 180.0f;
		WeaponJNs [0].maxPower = 100.0f;
		WeaponJNs [0].powerVelocity = 20.0f;
		WeaponJNs [1] = new AxeJNSystem ();
		WeaponJNs [1].SetEffect (AxeGentle,AxeLight,AxeGentlePos,AxeLightPos);
		WeaponJNs [1].weapon = this.gameObject;
		WeaponJNs [1].coolingTime = 180.0f;
		WeaponJNs [1].maxPower = 100.0f;
		WeaponJNs [1].powerVelocity = 20.0f;
		WeaponBuff = new AIDebuff[length];
		WeaponBuff [0] = AIDebuff.FIRE;
		WeaponBuff [1] = AIDebuff.ICE;
		AIDeBuffTime = new float[length];
		AIDeBuffTime [0] = 6;
		AIDeBuffTime [1] = 4;
		attackPower = new int[length];
		for(int i=0;i<length;i++)
		{
			attackPower [i] = 100;
		}

	}
	// Update is called once per frame
	void Update () 
	{
  
        if (IsWeaponRun)
        {
            Vector3 fmd = sample.transform.position - this.transform.position;
            fmd.y=0;
            fmd.Normalize();
            if (tempIndex <= 1)
            {
                WeaponJNs[tempIndex].forward = fmd;
                WeaponJNs[tempIndex].update();
            }
            if (!WeaponModel[tempIndex].activeSelf)
                WeaponModel[tempIndex].SetActive(true);
            if (vrinputsystem.TriggerDown)
            {
                change();
            }

        }
        else 
        {
            if (WeaponModel[tempIndex].activeSelf)
                WeaponModel[tempIndex].SetActive(false);
        }
	}
	void change()
	{
        if (changeStop)
        {
            WeaponModel[tempIndex].SetActive(false);
            tempIndex = (tempIndex + 1) % WeaponModel.Length;
            WeaponModel[tempIndex].SetActive(true);
            db.changeweapon(tempIndex);
            changeStop = false;
            StartCoroutine(waitLittle());
        }
	}
	public void attack(GameObject boss)
	{
		int value = (int)vrinputsystem.rightVelocity.magnitude * attackPower [tempIndex];
		bs.attack ("player",boss,value);
		float rand = Random.Range (0,10);
		if (rand > 7) 
		{
            if (boss.name == "Dragon")
            {
                boss.GetComponent<AIRT_FY>().controlMachine.aiState.tempDebuff = WeaponBuff[tempIndex];
                boss.GetComponent<AIRT_FY>().controlMachine.aiState.DebuffTime = AIDeBuffTime[tempIndex];
            }
            else 
            {
                boss.GetComponent<AIRT>().controlMachine.aiState.tempDebuff = WeaponBuff[tempIndex];
                boss.GetComponent<AIRT>().controlMachine.aiState.DebuffTime = AIDeBuffTime[tempIndex];
            }
		}
	}
	public void AddBuff(AIDebuff aiD,float t)
	{
		WeaponBuff [tempIndex] = aiD;
		AIDeBuffTime [tempIndex] = t;
	}
    IEnumerator waitLittle() 
    {
        yield return new WaitForSeconds(0.5f);
        changeStop = true;
    }
}
