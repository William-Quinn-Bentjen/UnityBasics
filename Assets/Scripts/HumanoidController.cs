using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidController : MonoBehaviour {
    public float speed = 8.0f;
    public Animator anim;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        transform.Translate(input * speed * Time.deltaTime, Space.World);
        anim.SetFloat("Speed", input.magnitude != 0.0f ? speed : 0.0f);
	}
}
