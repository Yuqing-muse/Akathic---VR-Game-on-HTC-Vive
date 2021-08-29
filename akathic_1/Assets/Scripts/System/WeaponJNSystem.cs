using UnityEngine;
using System.Collections;

namespace WeaponJN
{
	public class WeaponJNSystem
	{
		protected VRInput vrinputSystem;
		public GameObject weapon;
		protected bool IsTrigger;
		public float coolingTime;
		protected float timer_forCooling;
		public bool canBeTrigger = true;
		public float maxPower;
		public float tempPower;
		public float powerVelocity;
		protected Vector3 sampleStart;
		protected Vector3 sampleEnd;
        public Vector3 forward;
		public WeaponJNSystem()
		{

		}
		public virtual void SetEffect(GameObject g1,GameObject g2,GameObject gPos1,GameObject gPos2)
		{
			
		}
        protected virtual void effect()
		{
         
			IsTrigger = false;
			sampleEnd = weapon.transform.position;
		}
        protected virtual void TriggerShow()
		{
			
		}
        protected virtual void TriggerJN()
		{
			if (canBeTrigger) 
			{
				tempPower = 0.0f;
				IsTrigger = true;
				canBeTrigger = false;
				sampleStart = weapon.transform.position;
			}
		}
        public virtual void update()
		{
			if (!canBeTrigger) 
			{
				timer_forCooling += Time.deltaTime;
				if (timer_forCooling > coolingTime) 
				{
					timer_forCooling = 0.0f;
					canBeTrigger = true;
				}
			}
			if (vrinputSystem.TouchPanelDown) 
			{
				TriggerJN ();
			}
			if (IsTrigger) 
			{
				TriggerShow ();
				tempPower += Time.deltaTime*powerVelocity;
				if (!vrinputSystem.TouchPanelDown) 
				{
					effect ();
				}
			}
		}
        public virtual void TriggerWithPlane(float y) 
        {

        }
	};	
	public class SwordJNSystem:WeaponJNSystem
	{
		public GameObject SwordLightEffect;
		public GameObject SwordGentleEffect;
        public GameObject SwordLightPos;
        public GameObject SwordGentlePos;
		private GameObject SwordLightEffectInstance;
		public SwordJNSystem()
		{
			vrinputSystem = GameObject.FindGameObjectWithTag ("VRInputSystem").GetComponent<VRInput>();
		}
		public override void SetEffect(GameObject g1,GameObject g2,GameObject gPos1,GameObject gPos2)
		{
			SwordGentleEffect = g1;
            SwordGentlePos = gPos1;
			SwordLightEffect = g2;
            SwordLightPos = gPos2;
		}
		protected override void effect()
		{
			base.effect ();
			float velocity = vrinputSystem.rightVelocity.magnitude;
            GameObject g = GameObject.Instantiate(SwordGentleEffect, (sampleStart + sampleEnd) / 2 + forward * 1.5f + new Vector3(0.0f, 0.3f, 0.0f), Quaternion.identity) as GameObject;
			g.GetComponent<SwordGentle> ().power = (int)(velocity*5.0f+10.0f*tempPower);
       //     Debug.Log(velocity+"a");
        //    Debug.Log(tempPower+"b");
			g.GetComponent<SwordGentle> ().flyvelocity = tempPower / 50.0f;
			GameObject.Destroy (SwordLightEffectInstance);
		}
		protected override void TriggerShow()
		{
		//	Color c = SwordLightEffectInstance.GetComponent<MeshRenderer> ().material.color;
		//	c.a += Time.deltaTime*0.3f;
		//	c.a = Mathf.Min (c.a,1.0f);
		//	SwordLightEffectInstance.GetComponent<MeshRenderer> ().material.color = c;
			base.TriggerShow ();
		}
		protected override void TriggerJN()
		{
            if (canBeTrigger)
            {
                SwordLightEffectInstance = GameObject.Instantiate(SwordLightEffect, SwordLightPos.transform.position, SwordLightPos.transform.rotation) as GameObject;
                SwordLightEffectInstance.transform.SetParent(SwordLightPos.transform.parent);
            }
			base.TriggerJN ();
			
		}
		public override void update()
		{
			base.update ();
		}
        public override void TriggerWithPlane(float y)
        {
            base.TriggerWithPlane(y);
        }
	};
	public class AxeJNSystem:WeaponJNSystem
	{
		public GameObject AxeLightEffect;
		public GameObject AxeGentleEffect;
        public GameObject AxeLightPos;
        public GameObject AxeGentlePos;
		private GameObject AxeLightEffectInstance;
		private bool IsTriggerWithPlane;
		private float PlaneHight;
		public AxeJNSystem()
		{
			vrinputSystem = GameObject.FindGameObjectWithTag ("VRInputSystem").GetComponent<VRInput>();
		}
        public override void SetEffect(GameObject g1, GameObject g2,GameObject gPos1,GameObject gPos2)
		{
			AxeGentleEffect = g1;
            AxeGentlePos = gPos1;
			AxeLightEffect = g2;
            AxeLightPos = gPos2;
		}
        protected override void effect()
		{
			base.effect ();
			float velocity = vrinputSystem.rightVelocity.magnitude;
			GameObject.Destroy (AxeLightEffectInstance);
            Vector3 v = (sampleStart + sampleEnd) / 2 + forward * 2.5f + new Vector3(0.0f, 1.1f, 0.0f);
            v.y -= 1.0f;
		   GameObject g = GameObject.Instantiate (AxeGentleEffect,v,Quaternion.identity) as GameObject;
				g.GetComponent<AxeGentle> ().power = (int)(tempPower * 20.0f + velocity);

		}
        protected override void TriggerShow()
		{
			base.TriggerShow ();
		//	Color c = AxeLightEffectInstance.GetComponent<MeshRenderer> ().material.color;
		//	c.a += Time.deltaTime*0.3f;
		//	c.a = Mathf.Min (c.a,1.0f);
		//	AxeLightEffectInstance.GetComponent<MeshRenderer> ().material.color = c;
		}
        protected override void TriggerJN()
		{
            if (canBeTrigger)
            {
                AxeLightEffectInstance = GameObject.Instantiate(AxeLightEffect, AxeLightPos.transform.position, AxeLightPos.transform.rotation) as GameObject;
                AxeLightEffectInstance.transform.SetParent(AxeLightPos.transform.parent);
            }
			base.TriggerJN ();

		}
        public override void TriggerWithPlane(float y)
		{
            base.TriggerWithPlane(y);
			if (IsTrigger) 
			{
				PlaneHight = y;
				IsTriggerWithPlane = true;
				effect ();
			}
		}
        public override void update()
		{
			base.update ();
		}	
	};
};
