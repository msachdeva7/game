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

	private float currentSlide;
	private float maxSlide = 2;
	private float slideLerpCoeff = 0.5f;
	private float slideAngleCoeff = 0.05f;

	void Start () {
		TopSpeed = TheCar.GetComponent<CarController> ().MaxSpeed;

		slowPos = new Vector3(0, slowHeight, -slowDistance);
		fastPos = new Vector3(0, fastHeight, -fastDistance);

		currentSlide = 0;
	}

	void FixedUpdate () {

		float rigAngle = transform.localEulerAngles.y;
		if (Mathf.Abs (rigAngle) > 180) {
			rigAngle = -(360 - rigAngle);
		}
		float targetSlide = Mathf.Clamp (rigAngle * slideAngleCoeff, -maxSlide, maxSlide);
		currentSlide = Mathf.Lerp (currentSlide, targetSlide, slideLerpCoeff);

		float speed = TheCar.GetComponent<CarController> ().CurrentSpeed;
		float relativeSpeed = speed / TopSpeed;

		float fov = Mathf.Lerp      (slowFoV,      fastFoV,      relativeSpeed);
		float rot = Mathf.LerpAngle (slowRotation, fastRotation, relativeSpeed);

		Vector3 pos = Vector3.Lerp  (slowPos,      fastPos,      relativeSpeed);
		pos.x = -currentSlide;

		Camera.main.fieldOfView = fov;
		Camera.main.transform.localPosition = pos;
		Camera.main.transform.localEulerAngles = new Vector3(rot, 0, 0);
	}
}
