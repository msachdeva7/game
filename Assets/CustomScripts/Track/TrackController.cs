using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {
	
	public GameObject[] markers;
	public int LastVisited = 0;
	public Collider CarTracker;
	private int numMarkers;

		
	void Start() {
		numMarkers = markers.Length;
	}

	// The maintanance of LastVisited occurs via the Marker objects themselves, in the DetectCar component.

	public Vector3[] GetNextMarkers(int numReturned) {
		// returns an array containing the next markers/waypoints along the track
		Vector3[] nextMarkers = new Vector3[numReturned];
		for (int i = 0; i < numReturned; i++) {
			nextMarkers [i] = markers [(LastVisited + 1 + i) % numMarkers].transform.position;
		}
		return nextMarkers;
	}
}
