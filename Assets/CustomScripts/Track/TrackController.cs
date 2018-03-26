using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {
    public GameObject[] markers;
    public int lastVisited = -1;
    private int numMarkers;

    void Start() {
        List<GameObject> marker_lst = new List<GameObject>();
        foreach (DetectCar dc in GetComponentsInChildren<DetectCar>()) {
            marker_lst.Add(dc.gameObject);
        }
        markers = marker_lst.ToArray();
        numMarkers = markers.Length;
    }

    public Vector3[] GetNextMarkers(int numReturned) {
        // returns an array containing the next markers/waypoints along the track
        Vector3[] nextMarkers = new Vector3[numReturned];
        for (int i = 0; i < numReturned; i++) {
            nextMarkers [i] = markers [(lastVisited + 1 + i) % numMarkers].transform.position;
        }
        return nextMarkers;
    }

    public void CarVisit(GameObject marker) {
        int index = System.Array.IndexOf(markers, marker);
        if (index == -1) {
            Debug.Log("Error: Invalid marker");
        }
        else if (index != lastVisited) {
            lastVisited = index;
            Debug.Log("Marker " + lastVisited + " crossed");
        }
    }
}
