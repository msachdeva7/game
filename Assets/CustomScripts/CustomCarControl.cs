using System;
using System.Collections;
using System.Collections.Generic;
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
    public CarController m_Car;
    public UIControl ui;

    public float nitroFuel = 1; // amount of fuel left (in percentage of full tank)
    public float nitroForce = 100000; // nitro force applied per second
    public float nitroCost = 0.2f; // fuel burnt per second
    public float nitroRegen = 0.01f; // fuel regenerated per second
    public float fuelUsed = 0;

    public int updateEvery;
    public float obstacleDetectionRadius;

    const float NO_DETECTION = 10000;

    bool waitingForCommands = false;
    PlayerCommands cmds;
    bool hasSetup = false;

    float top_speed = 0;
    float distance_travelled = 0;
    float nitro_usage = 0;
    int lastUpdate = 0;

    int historyFramerate = 20; //number of frames between ghost car snapshots
    List<TransformData> history = new List<TransformData>();

    // Track related
    private int lastVisited = 0;
    private int lapsDone = 0;

    float secondsStuck = 0; //time we've been stuck for so far
    public float stuckTimeout = 3; //number of seconds of immobility before respawning
    public float stuckTolerance = 0.5f; //distance per second that counts as being 'non-stuck'

    private void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(gm == null) {
            Debug.Log("Error: Got no gm!");
        }
        tc = GameObject.Find("TrackController").GetComponent<TrackController>();
        if(tc == null) {
            Debug.Log("Error: Got no tc!");
        }
        ui = GameObject.Find("UI").GetComponent<UIControl>();
        if(ui == null) {
            Debug.Log("Error: Got no ui!");
        }
    }

    private void Awake() {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    public void Recolor(Color color) {
        Color old_color = transform.Find("SkyCar").transform.Find("SkyCarBodyPaintwork").GetComponent<MeshRenderer>().materials[1].color;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            for (int i = 0; i < mr.materials.Length; ++i) {
                if (mr.materials[i].color == old_color) {
                    mr.materials[i].SetColor("_Color", color);
                    mr.materials[i].SetColor("_SpecColor", color);
                }
            }
        }
    }

    private float CastRay(float angle, float distance) {
        RaycastHit hit;
        float detection = NO_DETECTION;
        Vector3 ray_direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        if (Physics.SphereCast(rb.position, obstacleDetectionRadius, ray_direction, out hit, distance, -1, QueryTriggerInteraction.Ignore)) {
            detection = Math.Min(detection, hit.distance);
        }
        if (Physics.Raycast(rb.position, ray_direction, out hit, distance, -1, QueryTriggerInteraction.Ignore)) {
            detection = Math.Min(detection, hit.distance);
        }
        return detection;
    }

    private List<ODRay> ODRays(float end_angle, float[] markers, float[] distances) {
        float initial_angle = (float)(Math.Asin(1 / distances[0]) * (180.0 / Math.PI));
        float position = initial_angle / end_angle;
        List<ODRay> odr_lst = new List<ODRay>();
        for (int i = 0; i < 50; ++i) {
            float angle = (float)(position * end_angle);
            float distance = distances[distances.Length - 1];
            for (int j = 0; j < markers.Length; ++j) {
                if (angle < markers[j]) {
                    float pos = (angle - markers[j - 1]) / (markers[j] - markers[j - 1]);
                    distance = (float)((1 - pos) * distances[j - 1] + pos * distances[j]);
                    break;
                }
            }
            ODRay odr;
            odr.bearing = angle;
            odr.distance = CastRay(angle, distance);
            odr_lst.Add(odr);
            ODRay odr2;
            odr2.bearing = -angle;
            odr2.distance = CastRay(-angle, distance);
            odr_lst.Add(odr2);

            if (position > 1) {
                return odr_lst;
            }

            float angle_increase = (float)(Math.Asin(2 / distance) * (180.0 / Math.PI));
            position += angle_increase / end_angle;
        }
        Debug.Log("Warning: Too many iterations in ODRays, shortcutting");
        return odr_lst;
    }

    private float CombineODRays(float start_angle, float end_angle, List<ODRay> odrays) {
        float detection = NO_DETECTION;
        foreach (ODRay odray in odrays) {
            if (odray.bearing >= start_angle && odray.bearing <= end_angle) {
                detection = Math.Min(detection, odray.distance);
            }
        }
        return detection;
    }

    public void VisitMarker(int index) {
        if (index == (lastVisited + 1) % tc.numMarkers) {
            lastVisited = index;
            Debug.Log("Marker " + lastVisited + " crossed");
            ui.FloatingText("Marker " + (lastVisited + 1) + "/" + tc.numMarkers + " passed");
            if (index == 0) {
                lapsDone += 1;
                ui.FloatingText("Lap " + lapsDone + "/" + (tc.numLaps > 0 ? tc.numLaps : 1) + " done");
                if (lapsDone >= tc.numLaps) {
                    EndLevel();
                }
            }
        }
        else if (index != lastVisited) {
            Debug.Log("Out of order marker ignored");
        }
    }

    private void applyNitro(float throttle) {
        if (throttle > 0 && nitroFuel > nitroCost * Time.deltaTime) {
            nitroFuel -= throttle * nitroCost * Time.deltaTime;
            nitro_usage += throttle * nitroCost * Time.deltaTime;
            rb.AddRelativeForce(Vector3.forward * nitroForce * Time.deltaTime);
        }
        nitroFuel += nitroRegen * Time.deltaTime;
        nitroFuel = Mathf.Clamp(nitroFuel, 0, 1);
    }

    private bool checkStuck() {
        if (rb.velocity.magnitude < stuckTolerance * Time.deltaTime) {
            secondsStuck += Time.deltaTime;
        } else {
            secondsStuck = 0;
        }
        return secondsStuck > stuckTimeout;
    }

    private void respawn() {
        secondsStuck = 0;
        Vector3 spawnPosition = tc.GetLastMarker(lastVisited);
        Quaternion spawnRotation = Quaternion.LookRotation(tc.GetNextMarker(lastVisited) - spawnPosition, Vector3.up);
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        rb.velocity = Vector3.zero;
    }

    private void FixedUpdate() {
        if (!hasSetup) {
            hasSetup = true;
            CarSetup cs = gm.inter.Setup();
            Recolor(cs.color);
        }
        int frames = gm.physicsFramesSinceStart;
        if (gm.inter.HasCommands()) {
            waitingForCommands = false;
            cmds = gm.inter.GetCommands();
            ui.ShowCommands(cmds);
        }

        m_Car.Move(cmds.steering, cmds.acceleration, cmds.brake);
        applyNitro(cmds.nitro);

        if (checkStuck()) {
            Debug.Log("Stuck!");
            ui.FloatingText("Stuck!\nReset to last waypoint");
            respawn();
        }
        if (rb.position.y < -1f) {
            Debug.Log("Too low!");
            ui.FloatingText("Drowning/falling!\nReset to last waypoint");
            respawn();
        }

        //record metrics
        fuelUsed += Mathf.Abs(cmds.acceleration);
        top_speed = Math.Max(top_speed, rb.velocity.magnitude);
        distance_travelled += rb.velocity.magnitude * Time.fixedDeltaTime;

        // record ghost
        if (frames % historyFramerate == 0) {
            Vector3 position = rb.transform.position;
            Quaternion rotation = rb.transform.rotation;
            history.Add(new TransformData(position, rotation));
        }

        if (!waitingForCommands && frames >= lastUpdate + updateEvery) {
            lastUpdate = frames;

            PlayerData data;
            data.speed = rb.velocity.magnitude;
            data.nitro_left = nitroFuel;

            Vector3[] waypoints = tc.GetNextMarkers(lastVisited, 2);
            Vector3 waypoint = waypoints[0] - rb.position;
            data.waypoint_distance = waypoint.magnitude;
            data.waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);
            waypoint = waypoints[1] - rb.position;
            data.future_waypoint_distance = waypoint.magnitude;
            data.future_waypoint_bearing = Vector3.SignedAngle(waypoint, transform.forward, Vector3.up);

            List<ODRay> odrays = ODRays(45, new float[]{0, 2, 6, 10, 22, 45}, new float[]{150, 100, 90, 40, 40, 30});
            data.obstacle_detection_rays = odrays.ToArray();

            data.time = frames * Time.fixedDeltaTime;
            data.frames = frames;

            ui.ShowData(data, fuelUsed);
            gm.inter.NewData(data);
            waitingForCommands = true;
        }
    }

    public void EndLevel() {
        EndLevelData data;
        data.time = gm.physicsFramesSinceStart * Time.fixedDeltaTime;
        data.frames = gm.physicsFramesSinceStart;
        data.top_speed = top_speed;
        data.track = SceneManager.GetActiveScene().name;
        data.fuel_used = fuelUsed;
        data.distance_travelled = distance_travelled;
        data.average_speed = distance_travelled / data.time;
        data.history = history;
        data.historyFramerate = historyFramerate;
        data.nitro_usage = nitro_usage;
        ui.EndLevel(data);
        gm.inter.EndLevel(data);
    }
}
