using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableCamera : MonoBehaviour {

	public GameObject TheCar;


	void Update () {
		// counteract rotation of the car model
		transform.eulerAngles = new Vector3(0, TheCar.transform.eulerAngles.y, 0);
	}
}
