using System;
using UnityEngine;
using UnityEngine.UI;

using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof (CarController))]
public class CustomCarControl : MonoBehaviour {
    public GameManager gm;
    public TrackController tc;
    public Rigidbody rb;
    public CarController m_Car; // the car controller we want to use

    public int updateEvery = 0;
    public float radius = 10;

    public Text speedText;
    public Text wonText;
    //danger monitors if an object is nearby within an angle of +-15 deg in front of the car
    public bool danger = false;

    bool waitingForCommands = false;
    PlayerCommands cmds;

    float top_speed = 0;
    int frames = 0;
    int lastUpdate = 0;

    private void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(gm == null) {
            Debug.Log("Error: Got no gm!");
        }
        tc = GameObject.Find("TrackController").GetComponent<TrackController>();
        if(tc == null) {
            Debug.Log("Error: Got no tc!");
        }
    }

    private void Awake() {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    private void FixedUpdate() {
        frames++;
        if (gm.inter.HasCommands()) {
            waitingForCommands = false;
            cmds = gm.inter.GetCommands();
        }

        float h = Convert.ToSingle(cmds.right) - Convert.ToSingle(cmds.left);
        float v = Convert.ToSingle(cmds.forward) - Convert.ToSingle(cmds.backward);
        float handbrake = Convert.ToSingle(cmds.handbrake);
        m_Car.Move(h, v, v, handbrake);

        top_speed = Math.Max(top_speed, rb.velocity.magnitude);

        if (!waitingForCommands && frames >= lastUpdate + updateEvery) {
            lastUpdate = frames;

            //radius of collision warning
            Collider[] nearby = Physics.OverlapSphere(rb.position, radius);
            danger = false;

            for (int i = 0; i < nearby.Length; i++) {
                float angle = Vector3.Angle(nearby[i].ClosestPointOnBounds(rb.position) - rb.position, transform.forward);

                if (angle <= 15f) {
                    danger = true;
                }
            }

            PlayerData data;
            data.speed = rb.velocity.magnitude;
            data.danger = danger;
            Vector3 waypoint = tc.GetNextMarker() - rb.position;
            data.waypoint_distance = waypoint.magnitude;
            data.waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);
            ShowData(data);
            gm.inter.NewData(data);
            waitingForCommands = true;
        }
    }

    public void EndLevel() {
        EndLevelData data;
        data.time = frames * Time.fixedDeltaTime;
        data.frames = frames;
        data.top_speed = top_speed;
        gm.inter.EndLevel(data);
        if (wonText.text == "") {
            wonText.text = "Time: " + data.time + " s\nMax speed: " + top_speed + " m/s";
        }
    }

    void ShowData(PlayerData data) {
        speedText.text = (Convert.ToInt32(data.speed) + " m/s, waypoint at "
                          + Convert.ToInt32(data.waypoint_bearing) + " deg "
                          + Convert.ToInt32(data.waypoint_distance) + " m "
                          + (data.danger ? "danger" : ""));
    }
}
