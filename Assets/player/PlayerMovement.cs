using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public Rigidbody rb;
	public float drive_force;
    bool waitingForCommands = false;
    int frames = 0;
	public Text speedText;
	private Vector3 v;
	//danger monitors if an object is nearby within an angle of +-15 deg in front of the car
	public bool danger;

    GameManager gm;

    // Use this for initialization
    void Start() {
		drive_force *= Time.fixedDeltaTime;
		v.x = 0.0f;
		v.z = 0.0f;
		danger = false;

		update_speed ();

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm == null) {
            Debug.Log("Cannot find GM");
        }
    }

    void FixedUpdate() {
        frames++;
        if (waitingForCommands && gm.inter.HasCommands()) {
            waitingForCommands = false;

            PlayerCommands cmds = gm.inter.GetCommands();

            if (cmds.forward) {
				rb.AddForce(drive_force*Vector3.forward);
				v.z += (drive_force / rb.mass) * Time.fixedDeltaTime;
            }
            if (cmds.backward) {
				rb.AddForce(drive_force*Vector3.back);
				v.z -= (drive_force / rb.mass) * Time.fixedDeltaTime;
            }
            if (cmds.left) {
				rb.AddForce(drive_force*Vector3.left);
				v.x += (drive_force / rb.mass) * Time.fixedDeltaTime;
            }
            if (cmds.right) {
				rb.AddForce(drive_force*Vector3.right);
				v.x -= (drive_force / rb.mass) * Time.fixedDeltaTime;
            }
			update_speed ();

			//radius of collision warning
			float radius = 10;
			Collider[] nearby = Physics.OverlapSphere (rb.position, radius);

			for (int i = 0; i < nearby.Length; i++) {
				float angle = Vector3.Angle (nearby [i].attachedRigidbody.position, rb.position);
				angle -= 90f;

				if (-15f <= angle && angle <= 15f)
					danger = true;
			}
        }

        if (!waitingForCommands) {
            PlayerData data;
			data.drive_force = drive_force;
            gm.inter.NewData(data);
            waitingForCommands = true;
        }
    }

    public void EndLevel() {
        EndLevelData data;
        data.time = Mathf.RoundToInt(frames * Time.fixedDeltaTime);
        gm.inter.EndLevel(data);
    }

	void update_speed (){
		//not being passed to the text display for some reason
		speedText.text = (v.magnitude).ToString () + " m/s";
	}
}
