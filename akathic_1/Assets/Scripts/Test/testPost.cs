using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class testPost : MonoBehaviour {

	public Material material;
	public float DistortTimeFactor;
	public float DistortStrength;
	private Vector2 bossTransform;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	public void changeCenter(Vector3 v)
	{
		bossTransform.x = v.x;
		bossTransform.y = v.y;
		bossTransform.x /= Camera.main.pixelWidth;
		bossTransform.y /= Camera.main.pixelHeight;
	}
	void OnRenderImage(RenderTexture src,RenderTexture dest)
	{
		if (material != null) 
		{
			material.SetFloat("_DistortTimeFactor", DistortTimeFactor);  
			material.SetFloat("_DistortStrength", DistortStrength);  
			material.SetFloat("centerX", bossTransform.x);  
			material.SetFloat("centerY", bossTransform.y);  
			Graphics.Blit (src, dest, material);
		}
		else 
		{
			Graphics.Blit (src,dest);
		}
	}
}
