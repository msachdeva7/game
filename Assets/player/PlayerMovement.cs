using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public Rigidbody rb;

    public float speed;

	// Use this for initialization
	void Start () {
        speed *= Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {        
        if (Input.GetKey("d"))
            rb.AddForce(speed, 0, 0);
        if (Input.GetKey("a"))
            rb.AddForce(-speed, 0, 0);
        if (Input.GetKey("w"))
            rb.AddForce(0, 0, speed);
        if (Input.GetKey("s"))
            rb.AddForce(0, 0, -speed);
    }
}
