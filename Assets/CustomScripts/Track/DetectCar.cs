using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCar : MonoBehaviour {
    public TrackController track_controller;

    private void Start() {
        track_controller = GameObject.Find("TrackController").GetComponent<TrackController>();
        if(track_controller == null) {
            Debug.Log("Got no track controller!");
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            track_controller.CarVisit(this.gameObject);
        }
    }
}
