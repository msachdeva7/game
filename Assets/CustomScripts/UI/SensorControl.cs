using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorControl : MonoBehaviour {
    public RawImage sensor_image;
    public Image dot_prefab;

    Vector3 adj;

    // Use this for initialization
    void Start () {
        RectTransform rt = dot_prefab.GetComponent<RectTransform>();
        float w_adj = rt.rect.width / 2;
        float h_adj = rt.rect.height / 2;
        adj = new Vector3(w_adj, h_adj, 0);
    }

    private Image NewDot() {
        Image dot = Instantiate(dot_prefab);
        dot.transform.SetParent(sensor_image.transform, false);
        return dot;
    }

    private void PositionDot(Image dot, ODRay odray) {
        dot.transform.position = sensor_image.transform.position + Quaternion.Euler(0, 0, -odray.bearing) * new Vector3(0, odray.distance * 0.5f, 0) + adj;
    }

    public void ShowSensors(ODRay[] odrays) {
        int index = 0;
        while (index < odrays.Length && odrays[index].distance >= 10000) {
            ++index;
        }
        foreach (Image dot in GetComponentsInChildren<Image>()) {
            if (dot.gameObject.GetInstanceID() == gameObject.GetInstanceID()) {
                continue;
            }
            if (index < odrays.Length) {
                PositionDot(dot, odrays[index]);
                ++index;
                while (index < odrays.Length && odrays[index].distance >= 10000) {
                    ++index;
                }
            }
            else {
                Destroy(dot.gameObject);
            }
        }
        while (index < odrays.Length) {
            PositionDot(NewDot(), odrays[index]);
            ++index;
            while (index < odrays.Length && odrays[index].distance >= 10000) {
                ++index;
            }
        }
    }
}
