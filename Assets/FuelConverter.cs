using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelConverter : MonoBehaviour {
	static FuelConverter thisGauge;
	static float minAngle = -76.7f, maxAngle = -193f;

	// Use this for initialization
	void Start () {
		thisGauge = this;
	}

	public static void ShowFuel (float fuel) {
		float angle = Mathf.Lerp (minAngle, maxAngle, Mathf.InverseLerp (0, 1, fuel));
		thisGauge.transform.eulerAngles = new Vector3 (0, 0, angle);
	}
}