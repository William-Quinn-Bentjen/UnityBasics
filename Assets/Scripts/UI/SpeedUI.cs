using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour {
    public Text SpeedDisplay;
    public GameObject Player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 temp = Player.GetComponent<Rigidbody>().velocity;
        temp.y = 0;
        SpeedDisplay.text = "Speed: " + temp.magnitude.ToString() + " m/s";
    }
}
