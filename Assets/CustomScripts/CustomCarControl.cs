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

    public int updateEvery;
    public float obstacleDetectionDistance;
    public float obstacleDetectionRadius;
    public float obstacleDetectionCenterExtension;
    const float NO_DETECTION = 10000;

    public Text dataText;
    public Text endLevelText;
    public Text scriptText;

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
        dataText = GameObject.Find("DataText").GetComponent<Text>();
        if(dataText == null) {
            Debug.Log("Error: Got no DataText!");
        }
        endLevelText = GameObject.Find("EndLevelText").GetComponent<Text>();
        if(endLevelText == null) {
            Debug.Log("Error: Got no EndLevelText!");
        }
        scriptText = GameObject.Find("ScriptText").GetComponent<Text>();
        if(scriptText == null) {
            Debug.Log("Error: Got no ScriptText!");
        }
    }

    private void Awake() {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    private float ObstacleDetectionBeam(float angle, float min_angle, float max_angle, float additional_distance=0) {
        Vector3 ray_direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        RaycastHit hit;
        if (Physics.SphereCast(rb.position, obstacleDetectionRadius, ray_direction, out hit, obstacleDetectionDistance + additional_distance, -1, QueryTriggerInteraction.Ignore)) {
            float hitangle = Vector3.Angle(hit.point - rb.position, transform.forward);
            if (min_angle <= hitangle && hitangle < max_angle) {
                return hit.distance;
            }
        }
        return NO_DETECTION;
    }

    private float CombineBeams(params float[] args) {
        float min = NO_DETECTION;
        foreach (float a in args) {
            min = Math.Min(a, min);
        }
        return min;
    }

    private void FixedUpdate() {
        frames++;
        if (gm.inter.HasCommands()) {
            waitingForCommands = false;
            cmds = gm.inter.GetCommands();
            scriptText.text = cmds.message;
        }

        float h = Convert.ToSingle(cmds.right) - Convert.ToSingle(cmds.left);
        float v = Convert.ToSingle(cmds.forward) - Convert.ToSingle(cmds.backward);
        float handbrake = Convert.ToSingle(cmds.handbrake);
        m_Car.Move(h, v, v, handbrake);

        top_speed = Math.Max(top_speed, rb.velocity.magnitude);

        if (!waitingForCommands && frames >= lastUpdate + updateEvery) {
            lastUpdate = frames;

            PlayerData data;
            data.speed = rb.velocity.magnitude;

            Vector3[] waypoints = tc.GetNextMarkers(2);
            Vector3 waypoint = waypoints[0] - rb.position;
            data.waypoint_distance = waypoint.magnitude;
            data.waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);
            waypoint = waypoints[1] - rb.position;
            data.future_waypoint_distance = waypoint.magnitude;
            data.future_waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);

            // If you change these values, make sure that the beams still overlap!
            // For a obstacleDetectionDistance of 30, the beam separation can be no more than 3 degrees.
            data.obstacle_detection_center = CombineBeams(ObstacleDetectionBeam(-3, 0, 5), ObstacleDetectionBeam(0, 0, 90, obstacleDetectionCenterExtension), ObstacleDetectionBeam(3, 0, 5));
            data.obstacle_detection_left = CombineBeams(ObstacleDetectionBeam(-6, 5, 90), ObstacleDetectionBeam(-9, 5, 90), ObstacleDetectionBeam(-12, 5, 90));
            data.obstacle_detection_right = CombineBeams(ObstacleDetectionBeam(6, 5, 90), ObstacleDetectionBeam(9, 5, 90), ObstacleDetectionBeam(12, 5, 90));

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
        if (endLevelText.text == "") {
            endLevelText.text = "Time: " + data.time + " s\nMax speed: " + top_speed + " m/s";
        }
    }

    void ShowData(PlayerData data) {
        dataText.text = (Convert.ToInt32(data.speed) + " m/s, waypoint at "
                         + Convert.ToInt32(data.waypoint_bearing) + " deg "
                         + Convert.ToInt32(data.waypoint_distance) + " m "
                         + (data.obstacle_detection_left != NO_DETECTION ? "danger left " + Convert.ToInt32(data.obstacle_detection_left) + " m " : "")
                         + (data.obstacle_detection_center != NO_DETECTION ? "danger center " + Convert.ToInt32(data.obstacle_detection_center) + " m " : "")
                         + (data.obstacle_detection_right != NO_DETECTION ? "danger right " + Convert.ToInt32(data.obstacle_detection_right) + " m " : "")
                         );
    }
}
