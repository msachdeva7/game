using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedConverter : MonoBehaviour {
    public float minAngle = -11.6f, maxAngle = -266f;

    public void ShowSpeed(float speed) {
        float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(0, 120, speed));
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
