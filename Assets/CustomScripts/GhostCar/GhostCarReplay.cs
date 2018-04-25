using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCarReplay : MonoBehaviour {
    GameManager gm;

    GhostCarSetup data;

    void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm == null) {
            Debug.Log("Error: Got no gm!");
        }
    }

    void FixedUpdate () {
        if (data.history == null || data.history.Count == 0) {
            data = gm.inter.GhostSetup();
            Debug.Log(data.history.Count);
            if (data.history.Count == 0) {
                gameObject.SetActive(false);
                return;
            }
        }
        int frame = gm.physicsFramesSinceStart;

        //update the interpolation endpoints
        int indexInHistory = frame / data.historyFramerate;
        Debug.Log(indexInHistory);
        TransformData lastTransform, nextTransform;
        if (indexInHistory >= data.history.Count - 1) {
            lastTransform = nextTransform = data.history[data.history.Count - 1];
        }
        else {
            lastTransform = data.history[indexInHistory];
            nextTransform = data.history[(indexInHistory + 1)];
        }

        //how far are we between the two frames?
        float interpolation = (float)(frame % data.historyFramerate) / data.historyFramerate;

        //compute interpolation and update position
        Vector3 position = Vector3.Lerp(lastTransform.position, nextTransform.position, interpolation);
        Quaternion rotation = Quaternion.Slerp(lastTransform.rotation, nextTransform.rotation, interpolation);
        transform.SetPositionAndRotation(position, rotation);
    }

}
