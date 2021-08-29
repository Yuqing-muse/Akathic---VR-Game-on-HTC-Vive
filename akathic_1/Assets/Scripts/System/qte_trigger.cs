using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class qte_trigger : MonoBehaviour 
{
	#region qte
	private float qte_timer;
	public GameObject[] sampleGameObject;
	private Vector3[] samplePoint;
	public GameObject sampleGameObjectInstance;
	public GameObject lightsphereInstance;
	private GameObject[] lightsphereall;
	private float[] life;
	[HideInInspector]
	public bool[] is_broken;
	public float maxLife;
	public float boomInterval;
	private int[] Index;
	public GameObject boom;
	public Camera camera_this;
	public GameObject camera_gameobject;
	private Animator animator;
	[HideInInspector]
	public bool is_trigger;
	public float max_triggerTime;
	private int tempIndex=0;
	private float speedNormal;
	private int startIndex=0;
	public Vector3 offset;
	private bool first = true;
    public BattleSystem battlesystem;
    private bool attackLock;
    public bool is_boom;
	#endregion

	#region shakeCamera
	private float shakefps=45f;
	private float shakeDelta = 0.005f;
	private Rect changeRect;
	private float shakeTime = 0.0f;
	private float frameTime = 0.0f;
	private bool isshakeCamera = false;
	public float shakeLevel = 3f;
	public float setShakeTime = 0.2f;	
	#endregion

	void Start () 
	{
		animator = GetComponent<Animator> ();
		samplePoint = new Vector3[sampleGameObject.Length];
		lightsphereall = new GameObject[sampleGameObject.Length];
		life = new float[sampleGameObject.Length];
		is_broken = new bool[sampleGameObject.Length];
		Index = new int[sampleGameObject.Length];
		speedNormal = animator.speed;
		setShakeTime = 10.0f;
		changeRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
	}

	public void qtetrigger()
	{
        if (!is_trigger&&!is_boom)
        {
            first = true;
            camera_gameobject.GetComponent<testPost>().changeCenter(camera_this.WorldToScreenPoint(this.transform.position));
            camera_gameobject.GetComponent<testPost>().enabled = true;
            camera_gameobject.GetComponent<BloomOptimized>().enabled = true;
            for (int i = 0; i < is_broken.Length; i++)
            {
                is_broken[i] = false;
            }
            animator.speed = 0.2f;
            qte_timer = 0.0f;
            is_trigger = true;
            for (int i = 0; i < sampleGameObject.Length; i++)
            {
                samplePoint[i] = sampleGameObject[i].transform.position;
            }
            for (int i = 0; i < lightsphereall.Length; i++)
            {
                float tempScale = sampleGameObject[i].transform.localScale.x / sampleGameObjectInstance.transform.localScale.x;
                Vector3 resultScale = lightsphereInstance.transform.localScale * tempScale;
                lightsphereall[i] = Instantiate(lightsphereInstance, samplePoint[i] + offset, Quaternion.identity) as GameObject;
                //lightsphereall [i].GetComponent<lightSphereManager> ().broken.transform.localScale = resultScale;
                //lightsphereall [i].GetComponent<lightSphereManager> ().notbroken.transform.localScale = resultScale;
                life[i] = maxLife;
                lightsphereall[i].GetComponent<lightSphereManager>().index = i;
                lightsphereall[i].GetComponent<lightSphereManager>().qteManager = this;
            }
            tempIndex = 0;
            startIndex = 0;
        }
	}
    IEnumerator attackLockWait() 
    {
        yield return new WaitForSeconds(0.4f);
        attackLock = false;
    }
	public void attack(int index,float velocity)
	{
        if (!attackLock)
        {
            attackLock = true;
            StartCoroutine(attackLockWait());
            if (index > is_broken.Length - 1)
            {
                return;
            }
            if (!is_broken[index])
            {
                if (velocity > maxLife * 0.4f)
                {
                    life[index] -= velocity;
                }
                if (life[index] <= 0)
                {
                    is_broken[index] = true;
                    Index[tempIndex] = index;
                    tempIndex++;
                }
            }
        }
	}

	public void boomtrigger()
	{
		isshakeCamera = true;
		float shakeLevel_ = (float)(startIndex + 1) / (float)tempIndex;
		shakeDelta = 0.005f * (1 - shakeLevel_) + 0.015f * shakeLevel_;
		Instantiate (boom,lightsphereall[Index[startIndex]].transform.position,Quaternion.identity) ;
        battlesystem.attack("player",this.gameObject,100);
		Destroy (lightsphereall[Index[startIndex]]);
		if (startIndex != tempIndex - 1) 
		{
			startIndex++;
			Invoke ("boomtrigger", boomInterval);
		}
		else
		{
			Invoke ("end", 2.0f);
		}
	}

	private void end()
	{
        //is_trigger = false;
        is_boom = false;
		camera_gameobject.GetComponent<testPost> ().enabled=false;
		camera_gameobject.GetComponent<BloomOptimized> ().enabled = false;
		animator.speed = speedNormal;
		isshakeCamera = false;
		changeRect.xMin = 0.0f;
		changeRect.yMin = 0.0f;
		camera_this.rect = changeRect;
		shakeTime = 0.0f;
		frameTime = 0.0f;
	}
	void Update () 
	{
		if (is_trigger) 
		{
			for(int i=0;i<sampleGameObject.Length;i++)
			{
				lightsphereall [i].transform.position = sampleGameObject [i].transform.position+offset;
			}
			if (qte_timer < max_triggerTime)
				qte_timer += Time.deltaTime;
			else 
			{
				is_trigger = false;
				animator.speed = speedNormal;
				camera_gameobject.GetComponent<testPost> ().enabled=false;
				camera_gameobject.GetComponent<BloomOptimized> ().enabled=false;
				for(int i=0;i<sampleGameObject.Length;i++)
				{
					Destroy (lightsphereall[i]);
				}
			}
			bool init=true;
			for(int i=0;i<is_broken.Length;i++)
			{
				if (!is_broken [i]) 
				{
					init = false;
					break;
				}
			}
			if (init&&first) 
			{
				isshakeCamera = true;
				is_trigger = false;
                is_boom = true ;
				boomtrigger ();

				first = false;
			}
		}
		if (isshakeCamera) 
		{
			if (shakeTime <= setShakeTime)
			{
				shakeTime += Time.deltaTime;
					changeRect.xMin = shakeDelta * (-1.0f + shakeLevel * Random.value);
					changeRect.yMin = shakeDelta * (-1.0f + shakeLevel * Random.value);
					camera_this.rect = changeRect;
			}
			else
			{
				isshakeCamera = false;
				changeRect.xMin = 0.0f;
				changeRect.yMin = 0.0f;
				camera_this.rect = changeRect;
				shakeTime = 0.0f;
				frameTime = 0.0f;
			}
		}
	}
}
