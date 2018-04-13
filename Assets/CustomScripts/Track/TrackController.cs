using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {
    private GameObject[] markers;
    public int numLaps;
    public int numMarkers;

    void Start() {
        List<GameObject> marker_lst = new List<GameObject>();
        foreach (DetectCar dc in GetComponentsInChildren<DetectCar>()) {
            marker_lst.Add(dc.gameObject);
        }
        markers = marker_lst.ToArray();
        numMarkers = markers.Length;
    }

    public Vector3 GetNextMarker(int lastVisited) {
        return GetNextMarkers(lastVisited, 1)[0];
    }

    public Vector3[] GetNextMarkers(int lastVisited, int numReturned) {
        // returns an array containing the next markers/waypoints along the track
        Vector3[] nextMarkers = new Vector3[numReturned];
        if (numMarkers == 0) {
            return nextMarkers;
        }
        for (int i = 0; i < numReturned; i++) {
            nextMarkers [i] = markers [(lastVisited + 1 + i) % numMarkers].transform.position;
        }
        return nextMarkers;
    }

    public Vector3 GetLastMarker(int lastVisited) {
        return markers[lastVisited].transform.position;
    }

    public void CarVisit(GameObject marker, GameObject car) {
        int index = System.Array.IndexOf(markers, marker);
        if (index == -1) {
            Debug.Log("Error: Invalid marker");
        }
        else {
            car.GetComponentInParent<CustomCarControl>().VisitMarker(index);
        }
    }
}
