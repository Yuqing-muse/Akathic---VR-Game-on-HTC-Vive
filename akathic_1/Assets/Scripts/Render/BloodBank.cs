using UnityEngine;
using System.Collections;

public class BloodBank : MonoBehaviour 
{
	public float FadeTime;
	public GameObject canvas_;
	private bool trigger_;
	private float timer;
	private MeshRenderer mr;
	public Camera MainCamera;
	private bool always;
	private float alwaysf;
	// Use this for initialization
	void Start () 
	{
		mr = canvas_.GetComponent<MeshRenderer>();
	//	BeTriggered ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (trigger_)
		{
			if (timer < FadeTime / 2) 
			{
				timer += Time.deltaTime;
				mr.material.SetFloat ("fadescale", timer / (FadeTime / 2)*0.8f);
			} 
			else if (timer < FadeTime) 
			{
				timer += Time.deltaTime;
				mr.material.SetFloat ("fadescale", (FadeTime - timer) / (FadeTime / 2)*0.8f);
			}
			else
			{
				canvas_.SetActive (false);
				trigger_ = false;
				timer = 0.0f;
			}
		}
		if(always)
			mr.material.SetFloat ("fadescale",alwaysf);
	}
	public void alwaysShow(float f)
	{
		canvas_.SetActive (true);	
		always = true;
		alwaysf = f;
	}
	public void close()
	{
		mr.material.SetFloat ("fadescale",0);
		always = false;
		canvas_.SetActive (false);	
	}
	public void BeTriggered()
	{
		canvas_.SetActive (true);	
		trigger_ = true;
		canvas_.transform.position = MainCamera.transform.position;

	}
}
