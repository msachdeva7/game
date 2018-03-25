using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCar : MonoBehaviour {

	public TrackController TrackController;

	void OnTriggerEnter(Collider other) {
		if (other == TrackController.CarTracker) {
			int index = System.Array.IndexOf (TrackController.markers, this.gameObject);
			TrackController.LastVisited = index;
		}
	}
}
