using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SpeedCamera : MonoBehaviour {

	public GameObject TheCar;
	private float TopSpeed;

	private float slowDistance = 5;
	private float slowHeight = 1.5f;
	private float slowRotation = 12;
	private float slowFoV = 60;
	private float fastDistance = 4;
	private float fastHeight = 0.8f;
	private float fastRotation = 0;
	private float fastFoV = 90;
	private Vector3 slowPos; 
	private Vector3 fastPos;

	void Start () {
		TopSpeed = TheCar.GetComponent<CarController> ().MaxSpeed;

		slowPos = new Vector3(0, slowHeight, -slowDistance);
		fastPos = new Vector3(0, fastHeight, -fastDistance);
	}

	void Update () {
		float speed = TheCar.GetComponent<CarController> ().CurrentSpeed;
		float relativeSpeed = speed / TopSpeed;

		Vector3 pos = Vector3.Lerp  (slowPos,      fastPos,      relativeSpeed);
		float fov = Mathf.Lerp      (slowFoV,      fastFoV,      relativeSpeed);
		float rot = Mathf.LerpAngle (slowRotation, fastRotation, relativeSpeed);

		Camera.main.fieldOfView = fov;
		Camera.main.transform.localPosition = pos;
		Camera.main.transform.localEulerAngles = new Vector3(rot, 0, 0);
	}
}
