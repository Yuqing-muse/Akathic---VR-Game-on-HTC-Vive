using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeGentle : MonoBehaviour
{

	[HideInInspector]
	public int power;
	private GameObject bs;
	public float attackDistance;
	public GameObject boom;
	// Use this for initialization
	void Start () 
	{
		GameObject[] allBoss = GameObject.FindGameObjectsWithTag ("AI");
		bs = GameObject.FindGameObjectWithTag ("BattleSystem");
		for(int i=0;i<allBoss.Length;i++)
		{
			if (Vector3.Distance (this.gameObject.transform.position, allBoss [i].transform.position) <= attackDistance) 
			{
				Instantiate (boom,allBoss[i].transform.position,Quaternion.identity);
				bs.GetComponent<BattleSystem> ().attack ("player",allBoss[i],power);
			}
		}
	}
}
