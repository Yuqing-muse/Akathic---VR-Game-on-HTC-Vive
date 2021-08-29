using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class StartingButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public void StartGame() 
    {
        if (File.Exists(Application.dataPath + "/1.xml"))
            File.Delete(Application.dataPath + "/1.xml");
        SceneManager.LoadScene(1);
    }
    public void EndGame() 
    {
        SceneManager.LoadScene(1);
    }
	// Update is called once per frame
	void Update () 
    {
		
	}
}
