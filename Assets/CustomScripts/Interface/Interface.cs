using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct ODRay {
    public float bearing;
    public float distance;
}

[Serializable]
public struct PlayerData {
    public float speed;
    public float obstacle_detection_left, obstacle_detection_center, obstacle_detection_right;
    public float obstacle_detection_far_left, obstacle_detection_far_right;
    public ODRay[] obstacle_detection_rays;
    public float waypoint_distance, waypoint_bearing;
    public float future_waypoint_distance, future_waypoint_bearing;
    public float nitro_left;
    public float time;
    public int frames;
}

[Serializable]
public struct PlayerCommands {
    public float steering, acceleration, brake, nitro;
    public String message;
}

[Serializable]
public struct EndLevelData {
    public float time, top_speed, fuel_used, distance_travelled, average_speed;
    public int frames;
    public String track;
}

public abstract class Interface : MonoBehaviour {
    public abstract void QueryEnv();
    public abstract void NewData(PlayerData data);
    public abstract bool HasCommands();
    public abstract PlayerCommands GetCommands();
    public abstract void EndLevel(EndLevelData data);
}
