using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using simpleExampleForAI;


// My  memorandum:
// 1. If we must allocate an AI to a monster,then most of them shouldn't use too much memeory if player is not nearby.(sleep)
// 2. I use animation_control.play(String s), we can later change it to play(int i) after we decide the animation sequece.
// 3. If a monster is asleep , then he can not notice player nearby
// 4. how to initiate animation controller 
// 5. According to Ye,we get a weight value table for different attacking skills
// 6. ChaseSimple play()  Vector3.rotateAround   角度or弧度？


namespace AI
{
	public enum AIDebuff
	{
		FIRE=0,
		LIGHT=1,
		DARK=2,
		ICE=3,
		NULL=4
	}
	public interface AIBehavior
	{
		void Play(GameObject AI, AIControl controlMachine, GameObject player);
	};
	public struct AIState
	{
		public Vector3 vDir;
		public bool isDied;
		public int attack;
		public int defence;
		public float velocity;
		public AIDebuff tempDebuff;
		public float DebuffTime;
		public Vector3 home;
		public bool lost;       //标识怪物打架打的忘记回家了(lost)还是结束战斗回家(not lost)
		public int hp;
		public int stateEnum;
		public int LeavePower;
		public int Level;
		public int sleepy;
		public int sleepCount;
		public float angerDistance;
		public float attackDistance;
		public float eyeSightLength;    //if player is far than this length, ai can't see and attack player
		public float height;
		public int halfAttackAngle;     //计算AI在多少面对玩家时多少角度误差内可以发动攻击动作。注意是怪物的面向向量和位置差向量的夹角。
		public float halfAttackProduct; //上一项halfAttackAngle的余弦值
		public bool infight;        //标识是否在战斗状态（影响接下来的动作策略）
		public bool beingAttack;    //标识当前的被攻击动画是否锁定。
		public bool weakState;          //可以被QTE。
		//当前的攻击动画的锁定由AttackSimple内自主实现。
	}
	public struct PlayerState
	{
		public float distanceToAI;
		public int Level;
		public int hp;
		public int stateEnum;
		public float height;
	}
	public abstract class AIAnimationControl
	{   
		//animation choice: Run,Walk,Sleep,Attack
		public abstract void Init();
		public abstract void play(string name);
		public abstract void play(int id);
		public abstract int getIDByName(string name);
		protected abstract void LockAnimation (float time);
		protected abstract void lockUpdate ();
		public Animator animator;
		public bool belock;
		public float allTime;
		public float locktimer;
	}
	public abstract class AIControl
	{
        public  int AttackCounter=0;
		protected database database_;
		public  BattleSystem battlesystem;
		public float beingAttackTime; 
		public CollideCheck CollideChecker;
		public bool skipCheck = false;
		public GameObject ai_this;
		public GameObject player_this;
		public float visibleDistance;   //how far can a monster be seen clearly 
		public AIAnimationControl animation_control;
		public AIBehavior[] allBehavior;
		public int behaviorcount = 0;
		public AIState aiState;
		protected float debufftimer = 0;
		public PlayerState playerState;
		protected int tempBehaviourIndex;
		protected int IforFire=0;
		protected bool bforIce=true;
		public abstract void Init();
		public abstract void RegisterBehavior(AIBehavior ab);
		public abstract void BehaviorCheck();  //this function is called every frame to check if the state can switch to each other
		public abstract bool VisibleCheck();
		public abstract void Update(); //called every frame. Must call BehaviourSchedule() and update aiState and playerState  
		public abstract void DebuffUpdate();
	};
};
namespace simpleExampleForAI
{
	public class CollideCheck
	{
		public int healthLoss;
		public simpleControl sc;
		public void OnTriggerEnter(Collider c)
		{
			if (c.gameObject.tag == "weapon") 
			{
				sc.aiState.beingAttack = true;
				sc.beingAttackTime = 0;
				healthLoss = 7;
				sc.aiState.hp -= healthLoss;
				if (sc.aiState.hp < 100)           
					sc.aiState.weakState = true;
				if (sc.aiState.hp <= 0)
					sc.aiState.isDied = true;
			}
		}
	}
	public class PathBarrier:AIBehavior
	{
		public float MaxBarrier=3.8f;
		public void Play(GameObject AI, AIControl controlMachine, GameObject player)
		{
			RaycastHit hit;
			if (Physics.Raycast (AI.transform.position, controlMachine.aiState.vDir, out hit, MaxBarrier, 1 << LayerMask.NameToLayer ("Barrier"))) 
			{
				Vector3 derserve;
				derserve = AI.transform.position - hit.collider.transform.position;
				derserve = new Vector3 (derserve.x, 0, derserve.y); 
				AI.GetComponent<CharacterController> ().Move (derserve.normalized*Time.deltaTime*2.0f);
			}
		}
	}
	public class Chasesimple : AIBehavior
	{
		public int turnCount;
		private bool rest;
		private float resttime;
		private float brakeDistance;  //if AI and player are too far apart , AI will chase faster
		public Chasesimple() 
		{
			turnCount = 0;
			rest = false;
			resttime = 0;
			//defaultVelocity
			brakeDistance = 10.0f;
		}
		public void Play(GameObject AI, AIControl controlMachine, GameObject player)
		{
			Vector2 a = new Vector2(AI.transform.forward.x, AI.transform.forward.z).normalized;    //monster face
			Vector3 diff = AI.transform.position - player.transform.position;   //怪物为起点 ,玩家为终点    
			Vector2 b = new Vector2(diff.x,diff.z).normalized;                         
			if (Vector2.Dot(a, -b) < controlMachine.aiState.halfAttackProduct) 
			{ //没转过身来，让他继续转，不改变状态
		//		controlMachine.skipCheck = true;
				AI.transform.forward = Vector3.Lerp(AI.transform.forward,-diff,Time.deltaTime*0.5f);
				controlMachine.animation_control.play("Walk");
			//	return;
			}
			//转过身来,判断距离是否追还是发动攻击，需要checkBehavior
		//	controlMachine.skipCheck = false;
			float distance = Vector3.Magnitude(diff);
			Vector3 moving = (player.transform.position - AI.transform.position);
			moving.y = 0;
			moving.Normalize();
			if (distance > brakeDistance)
			{
				
				AI.GetComponent<CharacterController>().Move(Time.deltaTime * moving * controlMachine.aiState.velocity);
				controlMachine.aiState.vDir = moving.normalized;
			}
			else
			{
				AI.GetComponent<CharacterController>().Move(Time.deltaTime * moving * controlMachine.aiState.velocity * distance / brakeDistance);
				controlMachine.aiState.vDir = moving.normalized;
			}
			controlMachine.animation_control.play("Run");
			controlMachine.aiState.lost = true;
		}
	}
	public class Roundsimple : AIBehavior
	{
		private float riaus; 
		private float changeTime;
		private float wakeTime;
		private float timer = 0.0f;
		private int counter =0 ;
		private int turnStep = 0;
		private bool turnBack = false;
		private Vector3 TempDir;
		public Roundsimple()
		{
			riaus = 8.0f;
			changeTime = 5.0f;
		//	AI.transform.forward = AI.transform.forward.normalized;
//			TempDir = new Vector3(Random.Range(0.0f, 1.0f), 0.0f, Random.Range(0.0f, 1.0f));
			//wakeTime 
			//TempDir
			//veloctiy
		}
		public void Play(GameObject AI, AIControl controlMachine, GameObject player)
		{
			controlMachine.aiState.vDir = AI.transform.forward.normalized;
			if (counter == 0)
			{
				AI.transform.forward.Normalize ();
				counter++;
			}
			if (turnBack && turnStep < 5)
			{
				turnStep++;
				AI.GetComponent<CharacterController>().Move(Time.deltaTime * AI.transform.forward * controlMachine.aiState.velocity);
				return;
			}
			turnBack = false;
			if (timer < changeTime)
			{
				timer += Time.deltaTime;
				//if AI steps out of circle, he turns back immediately
				if (Vector3.Distance(AI.transform.position, controlMachine.aiState.home) >= riaus)
				{
					AI.transform.forward = -AI.transform.forward;
					turnBack = true;
					turnStep = 0;
				}

				AI.GetComponent<CharacterController>().Move(Time.deltaTime * AI.transform.forward * controlMachine.aiState.velocity);
				controlMachine.animation_control.play("Walk");
			}
			else
			{
				AI.transform.forward = new Vector3(Random.Range(0.0f, 1.0f), 0.0f, Random.Range(0.0f, 1.0f));
				AI.transform.forward.Normalize();
				timer = 0.0f;
		//		Debug.Log ( AI.transform.forward.x+" "+AI.transform.forward.z);
				controlMachine.aiState.sleepy++;
			}
		}
	}
	public class Sleepsimple : AIBehavior
	{
		private float timer = 0.0f;
		private float wakeTime = 4.0f;
		public void Play(GameObject AI, AIControl controlMachine, GameObject player)
		{
			timer += Time.deltaTime;
			if (timer >= wakeTime)
			{
				timer = 0.0f;
				controlMachine.aiState.sleepy = 0;
			}
			else
			{
				controlMachine.animation_control.play("Sleep");
			}
		}
	}
	public class Backsimple : AIBehavior
	{
		public Vector3 homeDirection;
		public Backsimple()
		{
			homeDirection = Vector3.zero;
		}
		public void Play(GameObject AI, AIControl controlMachine, GameObject player)
		{
			if(homeDirection==Vector3.zero)
				homeDirection = (AI.transform.position - controlMachine.aiState.home).normalized;
			if (controlMachine.aiState.lost)
			{
				homeDirection = (AI.transform.position - controlMachine.aiState.home).normalized;
				controlMachine.aiState.lost = false;
			} 
			controlMachine.aiState.vDir = homeDirection.normalized;
			AI.GetComponent<CharacterController>().Move(Time.deltaTime * homeDirection * controlMachine.aiState.velocity);
			controlMachine.animation_control.play("Walk");
		}
	}
	public class Attacksimple : AIBehavior
	{
		public float animationTime;     //记录从攻击判定生效开始，播放动画的时间. 初设为0.5秒解锁。
		public AIControl controller;
		public Attacksimple() 
		{
			animationTime = 0;
		}
		public void Play(GameObject AI, AIControl controlMachine, GameObject player) 
		{
			if (controller == null)
				controller = controlMachine;
			controlMachine.animation_control.play("Attack");
			animationTime += Time.deltaTime;
			if (animationTime <= 2.0)
				controller.skipCheck = true;
			else {
				controller.playerState.hp -= controller.aiState.Level * 10 + controller.aiState.LeavePower * 5;
				//判断：如果player.hp < 0 进入战败的 UI
				controlMachine.battlesystem.attack("AI",player,controller.aiState.Level * 10 + controller.aiState.LeavePower * 5);
				controller.aiState.LeavePower -= 5;
				controller.skipCheck = false;
				animationTime = 0;
			}
		}

	}
	public class BeingAttacksimple : AIBehavior
	{
		public void Play(GameObject AI, AIControl controlMachine, GameObject player)
		{
			controlMachine.animation_control.play("UnderAttack");
		}
	}

	public class simpleControl : AIControl
	{
	  //beingAttack的动画锁定时间

		public override void RegisterBehavior(AIBehavior ab)
		{
			allBehavior[behaviorcount] = ab;
			behaviorcount++;
		}
		public void Init_aiState() 
		{
            aiState.tempDebuff = AIDebuff.NULL;
			aiState.attack = 0;
			aiState.defence = 0;
			aiState.velocity = 2.0f;
			//        public Vector3 home;
			aiState.home = ai_this.transform.position;
			//        public bool lost;
			aiState.lost = false;
			//        public int hp;
			aiState.hp = 1000;
			//        public int stateEnum;
			aiState.stateEnum = 0;
			//        public int LeavePower;
			aiState.LeavePower = 30;
			//        public int Level;
			aiState.Level = 1;
			//        public int sleepy;
			aiState.sleepy=0;
			//        public int sleepCount;
			aiState.sleepCount = 3;
			//        public float angerDistance;
			aiState.angerDistance = 15.0f;
			//        public float attackDistance;
			aiState.attackDistance = 3.0f;
			//        public float eyeSightLength;    //if player is far than this length, ai can't see and attack player
			aiState.eyeSightLength = 250.0f;
			//        public float height;
			aiState.height = ai_this.GetComponent<BoxCollider>().bounds.size.y;
			//        public int halfAttackAngle;     //计算AI在多少面对玩家时多少角度误差内可以发动攻击动作。注意是怪物的面向向量和位置差向量的夹角。
			aiState.halfAttackAngle = (int)Mathf.Acos(0.85f);
			//        public float halfAttackProduct; //上一项halfAttackAngle的余弦值
			aiState.halfAttackProduct = 0.85f;
			//        public bool infight;        //标识是否在战斗状态（影响接下来的动作策略）
			//       public bool beingAttack;    //标识当前的被攻击动画是否锁定。
			//        public bool Attacking;      //标识当前的攻击动画是否锁定。
			aiState.beingAttack = false;
			aiState.infight = false;
			aiState.halfAttackProduct = Mathf.Cos(aiState.halfAttackAngle);
			aiState.weakState = false;
		}
		public void Init_playerState() {
			//        public float distanceToAI;
			//        public int Level;
			playerState.Level = 2;
			//        public int hp;
			playerState.hp = 300;
			//        public int stateEnum;
			playerState.stateEnum = 0;
			playerState.height = player_this.GetComponent<Collider> ().bounds.size.y;
			//        public float height;
			playerState.distanceToAI = Vector3.Distance(ai_this.transform.position, player_this.transform.position);
		}
		public override void Init()
		{
			visibleDistance = 300.0f;
			battlesystem = GameObject.FindGameObjectWithTag ("BattleSystem").GetComponent<BattleSystem>();
			database_ = GameObject.FindGameObjectWithTag ("database").GetComponent<database>();
			allBehavior = new AIBehavior[10];
			AIBehavior chase = new Chasesimple();       //0.chase
			AIBehavior rounds = new Roundsimple();      //1.round
			AIBehavior sleep = new Sleepsimple();       //2.sleep
			AIBehavior back = new Backsimple();         //3.come back home
			AIBehavior attack = new Attacksimple();     //4.attack
			AIBehavior beingAttack = new BeingAttacksimple(); //5.being attacked
			AIBehavior pathbarrier = new PathBarrier();

			RegisterBehavior(chase);
			RegisterBehavior(rounds);
			RegisterBehavior(sleep);
			RegisterBehavior(back);
			RegisterBehavior(attack);
			RegisterBehavior(beingAttack);
			RegisterBehavior (pathbarrier);
			tempBehaviourIndex = 1; //init state to rounding
			beingAttackTime = 0;
			CollideChecker = new CollideCheck();
			CollideChecker.sc = this;
			// initiation of ai_state and player_state and Animation Controller
			Init_aiState();
			Init_playerState();
			animation_control.Init();
		}
		public override bool VisibleCheck()
		{   //if  the monster can see the player
			if (tempBehaviourIndex == 2) return false;  //sleeping ,can't see anything
			if (playerState.distanceToAI > aiState.eyeSightLength) return false;
			Vector3 ai_forward = ai_this.transform.forward;
			Vector3 pos_diff = ai_this.transform.position - player_this.transform.position;
			Vector2 ai_face = new Vector2(ai_forward.x, ai_forward.z).normalized;
			Vector2 pos_diff_2d = new Vector2(pos_diff.x, pos_diff.z).normalized;
			if (Vector2.Dot(ai_face, pos_diff_2d) < 0.5) return false;
			// if eyesight blocked by obstacles,return false;
			Vector3 ai_eye = new Vector3(ai_forward.x, (float)(0.75 * aiState.height), ai_forward.z);
			Vector3 player_pos = new Vector3(player_this.transform.position.x, (float)(0.5 * playerState.height), player_this.transform.position.z);
			Vector3 eyesight = ai_eye - player_pos;
			if (Physics.Raycast(ai_eye, eyesight, eyesight.magnitude,1<<LayerMask.NameToLayer("Barrier"))) return false;
			return true;
		}
		public override void DebuffUpdate ()
		{
			if (aiState.tempDebuff != AIDebuff.NULL) 
			{
				debufftimer += Time.deltaTime;
				if (debufftimer >= aiState.DebuffTime) 
				{
					if (aiState.tempDebuff == AIDebuff.FIRE) 
					{
						IforFire = 0;
					}
					if (aiState.tempDebuff == AIDebuff.ICE) 
					{
						bforIce = true;
						aiState.velocity = aiState.velocity / 0.7f;
					}
					debufftimer = 0;
					aiState.tempDebuff = AIDebuff.NULL;
				}
				switch (aiState.tempDebuff) 
				{
				case AIDebuff.FIRE:
					int temp = (int)(debufftimer - IforFire);
					if (temp == 1) 
					{
						IforFire++;
						aiState.hp = (int)((float)(aiState.hp) * 0.95f);
					}
					break;
				case AIDebuff.ICE:
					if(bforIce)
					{
						bforIce = false;
						aiState.velocity = aiState.velocity*0.7f;
					}
					break;
				case AIDebuff.DARK:
					break;
				case AIDebuff.LIGHT:
					break;
				case AIDebuff.NULL:
					break;
				default :
					break;
				}
			}
		}
		public override void BehaviorCheck()    //change tempBehaviourIndex to current behaviour index
		{
			if (skipCheck) return;
			switch (tempBehaviourIndex)
			{
			case 0: //now AI is chasing at player.If player runs out of range, AI comes back to its nest
				if (playerState.distanceToAI > 3 * aiState.angerDistance)
				{
					tempBehaviourIndex = 3;
					aiState.infight = false;
					break;
				}
				if (playerState.distanceToAI <= aiState.attackDistance)
				{   
					tempBehaviourIndex = 4;
				}
				break;
			case 1: //now AI is rounding . He falls into a short sleep every 3 times he changes direction
				//if AI sees player( in sight && not behind barrier),he begins to chase for fighting
				if (aiState.infight) tempBehaviourIndex = 0;
				//非战斗状态下
				else {
					if (!VisibleCheck()) tempBehaviourIndex = 1;         
					else if (playerState.distanceToAI < aiState.angerDistance)  //玩家闯进了怪物的领地                        
						tempBehaviourIndex = 0;
					else
						if (aiState.sleepy >= aiState.sleepCount) tempBehaviourIndex = 2;
				}
				break;
			case 2: // now AI is sleeping. He can't notice any movement of player except being attacked.
				if (aiState.sleepy < aiState.sleepCount) {  //if awake,continue to round 
					tempBehaviourIndex = 1;
				}
				//if being attacked ,switch to chase the player
				if (aiState.infight) tempBehaviourIndex = 0;
				break;
			case 3: // now AI is coming back home. If he comes back, he starts to round; 
				if ((ai_this.transform.position - this.aiState.home).magnitude <= 1.0)
					tempBehaviourIndex = 1;
				// if being attacked,he chases player again
				if (aiState.infight) tempBehaviourIndex = 0;
				break;
			case 4:
				tempBehaviourIndex = 0; //调整一下姿态和位置（可能是一帧，也可能很多帧），然后继续或结束战斗
				break;
			}
		}


		public override void Update()
		{
			if (!aiState.isDied) 
			{
                if (AttackCounter >= 8&&!ai_this.GetComponent<qte_trigger>().is_trigger) 
                {
                    ai_this.GetComponent<qte_trigger>().qtetrigger();
                    AttackCounter = 0;
                }
				allBehavior [6].Play (ai_this, this, player_this);
				DebuffUpdate ();
				//1.update the distance .If too far away,don't update
				playerState.distanceToAI = Vector3.Distance (ai_this.transform.position, player_this.transform.position);
			//	Debug.Log (playerState.distanceToAI+" "+visibleDistance);
				if (playerState.distanceToAI > visibleDistance)
					return;

				//2.now player can see AI. Change the state.           
				if (aiState.beingAttack) 
				{
					beingAttackTime += Time.deltaTime;
					if (beingAttackTime > 0.5) 
					{
						aiState.beingAttack = false;
						tempBehaviourIndex = 0;
					} 
					else 
					{
						allBehavior [5].Play (ai_this, this, player_this);
					}
				} 
				else 
				{
					BehaviorCheck ();
					allBehavior [tempBehaviourIndex].Play (ai_this, this, player_this);
				}
			}
		}

	}
}
