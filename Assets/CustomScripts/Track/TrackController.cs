using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {
    public GameObject[] markers;
    public int numLaps;
    private int lastVisited = -1;
    private int numMarkers;
    private int lapsDone = 0;
    private CustomCarControl car;

    void Start() {
        car = GameObject.Find("Car").GetComponent<CustomCarControl>();
        if(car == null) {
            Debug.Log("Error: Got no car!");
        }
        List<GameObject> marker_lst = new List<GameObject>();
        foreach (DetectCar dc in GetComponentsInChildren<DetectCar>()) {
            marker_lst.Add(dc.gameObject);
        }
        markers = marker_lst.ToArray();
        numMarkers = markers.Length;
    }

    public Vector3 GetNextMarker() {
        return GetNextMarkers(1)[0];
    }

    public Vector3[] GetNextMarkers(int numReturned) {
        // returns an array containing the next markers/waypoints along the track
        Vector3[] nextMarkers = new Vector3[numReturned];
        for (int i = 0; i < numReturned; i++) {
            nextMarkers [i] = markers [(lastVisited + 1 + i) % numMarkers].transform.position;
        }
        return nextMarkers;
    }

    public Vector3 GetLastMarker() {
        return markers[lastVisited].transform.position;
    }

    public void CarVisit(GameObject marker) {
        int index = System.Array.IndexOf(markers, marker);
        if (index == -1) {
            Debug.Log("Error: Invalid marker");
        }
        else if (index == (lastVisited + 1) % numMarkers) {
            lastVisited = index;
            Debug.Log("Marker " + lastVisited + " crossed");
            car.FloatMsg("Marker " + lastVisited + " passed");
            if (index == numMarkers - 1) {
                lapsDone += 1;
                if (lapsDone >= numLaps) {
                    car.EndLevel();
                }
            }
        }
        else if (index != lastVisited) {
            Debug.Log("Out of order marker ignored");
        }
    }
}
