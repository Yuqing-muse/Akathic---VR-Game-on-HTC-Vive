using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGentle : MonoBehaviour 
{
	[HideInInspector]
	public float flyvelocity;
	[HideInInspector]
	public int power;
	private GameObject target;

	public GameObject boom;
	private GameObject battlesystem;
    private float timer;
    GameObject player;
	void Start ()
	{
		battlesystem = GameObject.FindGameObjectWithTag ("BattleSystem");
		GameObject[] allBoss = GameObject.FindGameObjectsWithTag ("AI");
        player = GameObject.FindGameObjectWithTag("player");
		float MaxTargetDisance=-1.0f;
		int Index = -1;
		for(int i=0;i<allBoss.Length;i++)
		{
			if (Vector3.Distance (this.gameObject.transform.position, allBoss [i].transform.position) > MaxTargetDisance) 
			{
				MaxTargetDisance = Vector3.Distance (this.gameObject.transform.position, allBoss [i].transform.position);
				Index = i;
			}
		}
        if(Index!=-1)
		    target = allBoss [Index];
	}
    void Destroythis() 
    {
        Destroy(this.gameObject);
    }
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == target) 
		{
			battlesystem.GetComponent<BattleSystem> ().attack ("player",target,power);
			Instantiate (boom,this.transform.position,Quaternion.identity);
			Destroy (this.gameObject);
		}
	}
	// Update is called once per frame
	void Update () 
	{
     //   Debug.Log(flyvelocity);
        Vector3 vdir = player.transform.forward;
        vdir.y=0;
        this.transform.Translate(flyvelocity * Time.deltaTime * vdir * 2.0f);

        timer += Time.deltaTime;
        if (timer > 30.0f)
            Destroythis();
	}
}
