using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
