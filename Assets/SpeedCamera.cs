using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SpeedCamera : MonoBehaviour {

	public GameObject TheCar;
	private float TopSpeed;

	private float slowDistance = 5;
	private float fastDistance = 4;
	private float slowFoV = 60;
	private float fastFoV = 90;

	void Start () {
		TopSpeed = TheCar.GetComponent<CarController> ().MaxSpeed;
	}

	void Update () {
		float speed = TheCar.GetComponent<CarController> ().CurrentSpeed;
		float relativeSpeed = speed / TopSpeed;
		float dist = slowDistance + (fastDistance - slowDistance) * relativeSpeed;
		float fov = slowFoV + (fastFoV - slowFoV) * relativeSpeed;
		Camera.main.fieldOfView = fov;
		Vector3 localPos = Camera.main.transform.localPosition;
		localPos.z = -dist;
		Camera.main.transform.localPosition = localPos;
	}
}
