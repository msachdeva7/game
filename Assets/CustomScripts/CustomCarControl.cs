using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof (CarController))]
public class CustomCarControl : MonoBehaviour {
    public GameManager gm;
    public TrackController tc;
    public Rigidbody rb;
    public CarController m_Car; // the car controller we want to use

    public float nitroFuel = 1; // amount of fuel left (in percentage of full tank)
    public float nitroForce = 100000; // nitro force applied per second
    public float nitroCost = 0.2f; // fuel burnt per second
    public float nitroRegen = 0.01f; // fuel regenerated per second

	public float fuel = 1;

    public int updateEvery;
    public float obstacleDetectionRadius;

    const float NO_DETECTION = 10000;

    Text dataText;
    Text endLevelText;
    Text scriptText;
    Text timeText;

    bool waitingForCommands = false;
    PlayerCommands cmds;

    float top_speed = 0;
	float last_speed = 0;
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
        timeText = GameObject.Find("TimeText").GetComponent<Text>();
        if(timeText == null) {
            Debug.Log("Error: Got no TimeText!");
        }
    }

    private void Awake() {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    private float CastArc(float start_angle, float end_angle, float start_distance, float center_distance, float end_distance) {
        Assert.IsTrue(start_angle < end_angle);
        float initial_angle = (float)(Math.Asin(1 / start_distance) * (180.0 / Math.PI));
        float position = initial_angle / (end_angle - start_angle);
        float detection = NO_DETECTION;
        RaycastHit hit;
        float distance;
        for (int i = 0; i < 50; ++i) {
            if (position < 0.5) {
                distance = (float)((0.5 - position) * 2 * start_distance + position * 2 * center_distance);
            }
            else {
                distance = (float)((1 - position) * 2 * center_distance + (position - 0.5) * 2 * end_distance);
            }
            float angle = (float)((1 - position) * start_angle + position * end_angle);

            Vector3 ray_direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            if (Physics.SphereCast(rb.position, obstacleDetectionRadius, ray_direction, out hit, distance, -1, QueryTriggerInteraction.Ignore)) {
                float hitangle = Vector3.Angle(hit.point - rb.position, transform.forward);
                if (start_angle <= hitangle && hitangle < end_angle) {
                }
                detection = Math.Min(detection, hit.distance);
            }

            if (position > 1) {
                return detection;
            }

            float angle_increase = (float)(Math.Asin(2 / distance) * (180.0 / Math.PI));
            position += angle_increase / (end_angle - start_angle);
        }
        Debug.Log("Warning: Too many iterations in CastArc, shortcutting");
        return detection;
    }

    private void applyNitro(float throttle) {
        if (throttle > 0 && nitroFuel > nitroCost * Time.deltaTime) {
            nitroFuel -= throttle * nitroCost * Time.deltaTime;
            rb.AddRelativeForce(Vector3.forward * nitroForce * Time.deltaTime);
        }
        nitroFuel += nitroRegen * Time.deltaTime;
        nitroFuel = Mathf.Clamp(nitroFuel, 0, 1);
    }

    private void FixedUpdate() {
        frames++;
        if (gm.inter.HasCommands()) {
            waitingForCommands = false;
            cmds = gm.inter.GetCommands();
            scriptText.text = cmds.message;
        }

        m_Car.Move(cmds.steering, cmds.acceleration, cmds.brake);
        applyNitro(cmds.nitro);

        top_speed = Math.Max(top_speed, rb.velocity.magnitude);

        if (!waitingForCommands && frames >= lastUpdate + updateEvery) {
            lastUpdate = frames;

            PlayerData data;
            data.speed = rb.velocity.magnitude;
			float acceleration = (data.speed - last_speed) / Time.fixedDeltaTime;
			last_speed = data.speed;
			//maximum fuel consumption factor
			float f = 20000;
			fuel -= Mathf.Abs(acceleration/f);

            Vector3[] waypoints = tc.GetNextMarkers(2);
            Vector3 waypoint = waypoints[0] - rb.position;
            data.waypoint_distance = waypoint.magnitude;
            data.waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);
            waypoint = waypoints[1] - rb.position;
            data.future_waypoint_distance = waypoint.magnitude;
            data.future_waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);

            data.obstacle_detection_center = CastArc(-2, 2, 100, 150, 100);
            data.obstacle_detection_left = CastArc(-10, -2, 40, 90, 100);
            data.obstacle_detection_right = CastArc(2, 10, 100, 90, 40);
            data.obstacle_detection_far_left = CastArc(-45, -10, 30, 40, 40);
            data.obstacle_detection_far_right = CastArc(10, 45, 40, 40, 30);

            data.time = frames * Time.fixedDeltaTime;
            data.frames = frames;

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
        data.track = SceneManager.GetActiveScene().name;
        gm.inter.EndLevel(data);
        if (endLevelText.text == "") {
            endLevelText.text = "Time: " + data.time + " s\nMax speed: " + top_speed + " m/s";
        }
    }

    void ShowData(PlayerData data) {
        dataText.text = (Convert.ToInt32(data.speed) + " m/s, WP "
                         + Convert.ToInt32(data.waypoint_bearing) + " deg "
                         + Convert.ToInt32(data.waypoint_distance) + " m "
                         + "Nitro " + Convert.ToInt32(nitroFuel * 100) + "% "
                         + (data.obstacle_detection_left != NO_DETECTION ? "FL " + Convert.ToInt32(data.obstacle_detection_far_left) + " m " : "")
                         + (data.obstacle_detection_left != NO_DETECTION ? "L " + Convert.ToInt32(data.obstacle_detection_left) + " m " : "")
                         + (data.obstacle_detection_center != NO_DETECTION ? "C " + Convert.ToInt32(data.obstacle_detection_center) + " m " : "")
                         + (data.obstacle_detection_right != NO_DETECTION ? "R " + Convert.ToInt32(data.obstacle_detection_right) + " m " : "")
                         + (data.obstacle_detection_right != NO_DETECTION ? "FR " + Convert.ToInt32(data.obstacle_detection_far_right) + " m " : "")
                         );
        timeText.text = Math.Floor(data.time / 60) + ":" + (data.time % 60 < 10 ? "0" : "") + Math.Floor(data.time % 60);
        SpeedConverter.ShowSpeed(data.speed);
		FuelConverter.ShowFuel (fuel);
    }
}
