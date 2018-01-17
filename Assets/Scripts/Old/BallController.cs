using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public Rigidbody rb;
    public float speed;
    public int score;
    public int health;
    public int maxHealth;
    public int stamina;
    public int maxStamina;
    // Use this for initialization
    void Start()
    {
        health = maxHealth;

        stamina = maxStamina;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Vertical");
        float vertical = Input.GetAxis("Horizontal");
        bool sprint = Input.GetButton("Fire3");
        if (stamina > 0 && (vertical > 0 || horizontal > 0 || sprint))
        {
            stamina--;
            Debug.Log(stamina + "/" + maxStamina);
        }
        if (health > 0)
        {
            Vector3 move = new Vector3(horizontal, 0, vertical);
            if (sprint)
            {
                rb.AddForce(move * (speed * 2) * Time.deltaTime);
            }
            else
            {
                rb.AddForce(move * speed * Time.deltaTime);
                stamina++;
                if (stamina >= maxStamina)
                {
                    stamina = maxStamina;
                    Debug.Log(stamina + "/" + maxStamina);
                }
            }

        }
        else
        {
            Debug.Log("Player dead");
        }
    }
}