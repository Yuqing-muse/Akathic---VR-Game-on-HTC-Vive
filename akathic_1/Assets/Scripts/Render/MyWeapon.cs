using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  
 
class Wp  
{  
	public Vector3 point;  
	public Vector3 upDir;  
	public float time;  
	public Wp() {  

	}  
	public Wp(Vector3 p, float t) {  
		point = p;  
		time = t;  
	}  
}  

public class MyWeapon : MonoBehaviour {  
	private List<Wp> sections = new List<Wp>(); 
	private Mesh mesh;  
	public float time = 2.0f;  
	public Color startColor = Color.white;  
	public Color endColor = new Color(1, 1, 1, 0);  
	private MeshRenderer meshRenderer;  
	private Material trailMaterial;  
	public float height = 2.0f;  
	private bool isPlay = false;  
	void Awake() 
	{  

		MeshFilter meshF = GetComponent(typeof(MeshFilter)) as MeshFilter;  
		mesh = meshF.mesh;  
		meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;  
		trailMaterial = meshRenderer.material;  


	}  
	void Start () 
	{  
		weStart ();
	}  
		
	void FixedUpdate()  
	{  
		if (isPlay)  
		{  
			Itterate(Time.time);  
			UpdateTrail(Time.time);  
		}  
	}  

	public void weStart() 
	{  
		isPlay = true;  
	}  

	public void weStop() {  
		isPlay = false;  
		ClearTrail();  
	}  

	public void Itterate(float itterateTime) { 
		//Debug.Log ("sssss");
		Vector3 position = transform.position;  
		float now = itterateTime;  


		if (sections.Count == 0 || (sections[0].point - position).sqrMagnitude > 0) {  
			Wp section = new Wp();  
			section.point = position;  
			section.upDir = transform.TransformDirection(Vector3.up); 
			section.time = now;  
			sections.Insert(0, section);  

		}  
	}  
	public void UpdateTrail(float currentTime) 
	{ 

		//Debug.Log ("sss");
		//Debug.Log ("sssss111");
		mesh.Clear();  

		while (sections.Count > 0 && currentTime > sections[sections.Count - 1].time + time)
		{  
			sections.RemoveAt(sections.Count - 1);  
		}  

		Vector3[] vertices = new Vector3[sections.Count * 2];  
		Color[] colors = new Color[sections.Count * 2];  
		Vector2[] uv = new Vector2[sections.Count * 2];  
 
		Wp currentSection = sections[0];  


		Matrix4x4 localSpaceTransform = transform.worldToLocalMatrix;  
  
		for (var i = 0; i < sections.Count; i++) 
		{  
			currentSection = sections[i];  
			float u = 0.0f;  
			if (i != 0)  
				u = Mathf.Clamp01((currentTime - currentSection.time) / time);   

			Vector3 upDir = currentSection.upDir;  


			vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.point);  
			vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + upDir * height);  

			uv[i * 2 + 0] = new Vector2(u, 0);  
			uv[i * 2 + 1] = new Vector2(u, 1);  

			Color interpolatedColor = Color.Lerp(startColor, endColor, u);  
			colors[i * 2 + 0] = interpolatedColor;  
			colors[i * 2 + 1] = interpolatedColor;  
		}  
			

		int[] triangles = new int[(sections.Count - 1) * 2 * 3];  
		for (int i = 0; i < triangles.Length / 6; i++) {  
			triangles[i * 6 + 0] = i * 2;  
			triangles[i * 6 + 1] = i * 2 + 1;  
			triangles[i * 6 + 2] = i * 2 + 2;  

			triangles[i * 6 + 3] = i * 2 + 2;  
			triangles[i * 6 + 4] = i * 2 + 1;  
			triangles[i * 6 + 5] = i * 2 + 3;  
		}  
		Debug.Log ("ssssssss");
		Debug.Log (vertices.Length.ToString());
		mesh.vertices = vertices;  
		mesh.colors = colors;  
		mesh.uv = uv;  
		mesh.triangles = triangles;  


	}  
	public void ClearTrail() {  

		if (mesh != null) {  
			mesh.Clear();  
			sections.Clear();  
		}  
	}  
}
