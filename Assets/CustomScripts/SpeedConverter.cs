using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedConverter : MonoBehaviour {
    static SpeedConverter thisSpeedo;
    static float minAngle = -11.6f, maxAngle = -266f;

    // Use this for initialization
    void Start () {
        thisSpeedo = this;
    }

    public static void ShowSpeed (float speed, float min, float max) {
        float angle = Mathf.Lerp (minAngle, maxAngle, Mathf.InverseLerp (min, max, speed));
        thisSpeedo.transform.eulerAngles = new Vector3 (0, 0, angle);
    }
}
