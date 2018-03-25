using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {
	
	// Settings
	private int numReturned = 10;
	private int numMarkers;

	public GameObject[] markers;
	public int LastVisited = 0;
	public Collider CarTracker;

		
	void Start() {
		numMarkers = markers.Length;
	}

	// The maintanance of LastVisited occurs via the Marker objects themselves, in the DetectCar component.

	public Vector3[] GetNextMarkers() {
		// returns an array containing the next markers/waypoints along the track
		Vector3[] nextMarkers = new Vector3[numReturned];
		for (int i = 0; i < numReturned; i++) {
			nextMarkers [i] = markers [(LastVisited + 1 + i) % numMarkers].transform.position;
		}
		return nextMarkers;
	}
}
