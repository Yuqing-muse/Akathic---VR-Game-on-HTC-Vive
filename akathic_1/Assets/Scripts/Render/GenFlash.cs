using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenFlash : MonoBehaviour 
{
	public struct Line
	{
		public Vector3 startPoint;
		public Vector3 endPoint;
	};
	Line[] allLines;
	public int maxCount;
	public int tempIndex=0;
	public GameObject testsample1;
	public GameObject testsample2;
	// Use this for initialization
	public Camera cam;
	void Start ()
	{
		allLines = new Line[maxCount];

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	public void addBuffer(Vector3 vStart,Vector3 vEnd)
	{
		allLines [tempIndex].startPoint = vStart;
		allLines [tempIndex].endPoint = vEnd;
		tempIndex++;
	}
	public void clearBuffer()
	{
		tempIndex = 0;
	}
	public void Gen()
	{
		int Indexall = tempIndex;
		for(int i=0;i<Indexall;i++)
		{
			Vector3 v1 = allLines [i].startPoint;
			Vector3 v2 = allLines [i].endPoint;
			Vector3 vmid = (v1 + v2) / 2;
			float vdis = Vector3.Distance (v1,v2);
			Vector3 vDir = Vector3.Cross (v1 - v2, Camera.main.transform.position - vmid).normalized;
			vmid += vDir * vdis * Random.Range (-0.5f,0.5f);
			if (tempIndex <= maxCount - 1)
			{
				addBuffer (v1, vmid);
				allLines [i].startPoint = vmid;
				allLines [i].endPoint = v2;
			}
			else
			{
				break;
			}
			if (tempIndex <= maxCount - 1) 
			{
				float start = Random.Range (0.0f,1.0f);
				Vector3 v_s = Vector3.Lerp (v1,vmid,start);
				Vector3 v_dir = new Vector3 (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
				Vector3 v_e = v_s + v_dir.normalized * Random.Range (0.35f, 0.85f) * vdis * 0.5f;
				addBuffer (v_s,v_e);
			}
			else
			{
				break;
			}
			if (Random.Range (0, 3) == 0) 
			{
				i++;
			}
		}
		if (tempIndex <= maxCount - 1)
		{
			Gen ();
		}
	}
}
