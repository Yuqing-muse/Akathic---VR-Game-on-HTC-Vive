using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
	public AudioClip[] allSound;
	public AudioClip[] allBGM;
	public string[] allName;
	AudioSource myAudioSoure;
	public AudioSource bgmAudioSoure;
	float timer=0;
	int tempIndex=0;
	// Use this for initialization
	void Start () 
	{
		myAudioSoure = GetComponent<AudioSource>();
	}
	public void play(string name)
	{
		for (int i = 0; i < allName.Length; i++) 
		{
			if (allName [i] == name) 
			{
				myAudioSoure.clip = allSound [i];
				myAudioSoure.Play ();
				break;
			}
		}
	}
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		if (timer > 900.0f) 
		{
			timer = 0.0f;
			tempIndex = (tempIndex + 1) % allBGM.Length;
			myAudioSoure.clip = allBGM [tempIndex];
		}
	}
}
