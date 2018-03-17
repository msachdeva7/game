using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableCamera : MonoBehaviour {

	public GameObject TheCar;     // set in editor, this is the player Car object
	public float rotationDamping; // set in editor, suggested value 10
	private Rigidbody m_RigidBody;

	private void Start() {
		m_RigidBody = TheCar.GetComponent<Rigidbody> ();
	}

	void Update () {

		float actual_heading = (m_RigidBody.velocity.sqrMagnitude > 0.1 ?
			Quaternion.LookRotation (m_RigidBody.velocity, Vector3.up).eulerAngles.y
		:
			m_RigidBody.transform.eulerAngles.y
		                );

		Quaternion desiredHeadingQ = Quaternion.Euler (0, actual_heading, 0);

		// smoothly rotate towards current heading
		transform.rotation = Quaternion.Lerp (transform.rotation, desiredHeadingQ, Time.deltaTime * rotationDamping);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //steady the camera
	}
}
