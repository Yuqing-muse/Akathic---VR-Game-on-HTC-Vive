using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
public class xiyiAnimation : AIAnimationControl 
{
	string[] animationName;
	public  int id;
	public override void Init()
	{
		animationName = new string[5];
		animationName [0] = "Sleep";
		animationName [1] = "Attack";
		animationName[2]="Run";
		animationName[3]="Walk";
		animationName [4] = "underAttack";
		id = 0;
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

	public override void play (int id)
	{
			this.id = id;
			switch (id) 
			{
			case 0:
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",false);
				break;
			case 1:
				animator.SetBool ("IsAttack", true);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",false);
				break;
			case 2:
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", true);
				animator.SetBool ("IsWalk",false);
				break;
			case 3:
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",true);
				break;
			case 4:
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",true);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",false);
				break;
			}
	}

	public override void play (string name)
	{
		for (int i = 0; i < 5; i++) 
		{
			if (animationName [i] == name)
				this.id = i;
		}
			switch (name) 
			{
			case "Sleep":
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",false);
				break;
			case "Attack":
				animator.SetBool ("IsAttack", true);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",false);
				break;
			case "Run":
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", true);
				animator.SetBool ("IsWalk",false);
				break;
			case "Walk":
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",false);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",true);
				break;
			case "underAttack":
				animator.SetBool ("IsAttack", false);
				animator.SetBool ("IsBeAttack",true);
				animator.SetBool ("IsRun", false);
				animator.SetBool ("IsWalk",false);
				break;
			}
	}
	protected override void LockAnimation (float time)
	{
		belock = true;
		allTime = time;
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
}
