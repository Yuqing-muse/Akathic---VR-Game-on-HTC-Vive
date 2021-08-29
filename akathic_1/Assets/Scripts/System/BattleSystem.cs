                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class BattleSystem : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	void Start () 
	{
		//player = GameObject.FindGameObjectWithTag ("player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void attack(string tag_atk,GameObject tag_beatk,int value)
	{
		if (tag_atk == "AI") 
		{
			int def = tag_beatk.GetComponent<CharacterRT> ().defence;
			int realvalue = value - def;
			tag_beatk.GetComponent<CharacterRT> ().beattack (realvalue);
		}
		else if(tag_atk=="player")
		{
			float fireRate = player.GetComponent<CharacterRT> ().fireRate;
			float rand = Random.Range (0.0f, 1.0f);
			if(rand<fireRate)
			{
				value *= 2;
			}
            if (tag_beatk.name == "Dragon") 
            {
                int def = tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.defence;
                AIDebuff aid = tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.tempDebuff;
                int realValue;
                switch (aid)
                {
                    case AIDebuff.DARK:
                        realValue = (int)((value - def) * 1.15f);
                        break;
                    case AIDebuff.LIGHT:
                        float rand1 = Random.Range(0.0f, 1.0f);
                        if (rand1 > 0.5f)
                            realValue = (int)(value - (int)(def * 0.6f));
                        else
                            realValue = value - def;
                        break;
                    default:
                        realValue = value - def;
                        break;
                }
                if (!tag_beatk.GetComponent<qte_trigger>().is_trigger)
                {
                    realValue = 50;
                    tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.hp -= realValue;
                    tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.beingAttack = true;
                    tag_beatk.GetComponent<AIRT_FY>().controlMachine.beingAttackTime = 0;
                    tag_beatk.GetComponent<AIRT_FY>().controlMachine.AttackCounter++;
                    if (tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.hp < 100)
                        tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.weakState = true;
                    if (tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.hp <= 0)
                        tag_beatk.GetComponent<AIRT_FY>().controlMachine.aiState.isDied = true;
                }
            }
            else
            {
                int def = tag_beatk.GetComponent<AIRT>().controlMachine.aiState.defence;
                AIDebuff aid = tag_beatk.GetComponent<AIRT>().controlMachine.aiState.tempDebuff;
                int realValue;
                switch (aid)
                {
                    case AIDebuff.DARK:
                        realValue = (int)((value - def) * 1.15f);
                        break;
                    case AIDebuff.LIGHT:
                        float rand1 = Random.Range(0.0f, 1.0f);
                        if (rand1 > 0.5f)
                            realValue = (int)(value - (int)(def * 0.6f));
                        else
                            realValue = value - def;
                        break;
                    default:
                        realValue = value - def;
                        break;
                }
                if (!tag_beatk.GetComponent<qte_trigger>().is_trigger)
                {
                    realValue = 50;
                    tag_beatk.GetComponent<AIRT>().controlMachine.aiState.hp -= realValue;
                    tag_beatk.GetComponent<AIRT>().controlMachine.aiState.beingAttack = true;
                    tag_beatk.GetComponent<AIRT>().controlMachine.beingAttackTime = 0;
                    tag_beatk.GetComponent<AIRT>().controlMachine.AttackCounter++;
                    if (tag_beatk.GetComponent<AIRT>().controlMachine.aiState.hp < 100)
                        tag_beatk.GetComponent<AIRT>().controlMachine.aiState.weakState = true;
                    if (tag_beatk.GetComponent<AIRT>().controlMachine.aiState.hp <= 0)
                        tag_beatk.GetComponent<AIRT>().controlMachine.aiState.isDied = true;
                }
            }
		}
	}
}
