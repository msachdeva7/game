using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TriggerFinish : MonoBehaviour {

    public Text t;
    GameManager gm;

	// Use this for initialization
	void Start () {
        gm = FindObjectOfType<GameManager>();
        gm.enabled = true;
        t.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            t.enabled = true;
            gm.enabled = true;
        }
    }
}
