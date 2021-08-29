using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
public class DragonAnim:AIAnimationControl 
{
	string[] animationName;
	public  int id;
    int fir;
	public override void Init ()
	{
		animationName = new string[5];
		animationName [0] = "Sleep";
		animationName [1] = "Attack";
		animationName[2]="Run";
		animationName[3]="Walk";
		animationName [4] = "underAttack";
		id = 0;
        fir = 0;
	}

	public override void play (string name)
	{

        int thisid = -1;
		for (int i = 0; i < 5; i++) 
		{
			if (animationName [i] == name)
				thisid = i;
		}
        if (id == 2&&thisid!=2&&fir!=0)
        {
            animator.SetBool("IsRunning",false);
            fir = 0;
        }
        id = thisid;
		switch (name) 
		{
		case "Sleep":
			animator.SetBool ("IsIdle", true);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case "Attack":
			if (Random.Range (0.0f, 1.0f) > 0.5f) 
			{
				animator.SetBool ("IsAttackRight", true);
				animator.SetBool ("IsAttackForward", false);
			}
			else
			{
				animator.SetBool ("IsAttackForward", true);
				animator.SetBool ("IsAttackRight", false);
			}
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case "Run":
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", true);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",false);
            if (fir != 0)
                animator.SetBool("IsRunning", true);
            if (fir == 0) { fir++; }
			break;
		case "Walk":
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", true);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case "underAttack":
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",true);
			break;
		}
	}
	public override void play (int id)
	{
		this.id = id;
		switch (id) 
		{
		case 0:
			animator.SetBool ("IsIdle", true);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case 1:
			if (Random.Range (0.0f, 1.0f) > 0.5f) 
			{
				animator.SetBool ("IsAttackRight", true);
				animator.SetBool ("IsAttackForward", false);
			}
			else
			{
				animator.SetBool ("IsAttackForward", true);
				animator.SetBool ("IsAttackRight", false);
			}
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case 2:
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", true);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case 3:
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", true);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",false);
			break;
		case 4:
			animator.SetBool ("IsIdle", false);
			animator.SetBool ("IsWalk", false);
			animator.SetBool ("IsRun", false);
			animator.SetBool ("IsAttackRight", false);
			animator.SetBool ("IsAttackForward", false);
			animator.SetBool ("IsBeattack",true);
			break;
		}
	}

	public override int getIDByName (string name)
	{
		for(int i=0;i<animationName.Length;i++)
		{
			if (animationName [i] == name) 
			{
				return i;
			}
		}
		return -1;
	}

	protected override void lockUpdate ()
	{
		if (belock) 
		{
			locktimer += Time.deltaTime;
			if (locktimer >= allTime) 
			{
				locktimer = 0.0f;
				belock = false;
			}
		} 
	}
	protected override void LockAnimation (float time)
	{
		belock = true;
		allTime = time;
	}
}
