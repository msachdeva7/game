using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerData {
    public float speed;
    public float obstacle_detection_left, obstacle_detection_center, obstacle_detection_right;
    public float obstacle_detection_far_left, obstacle_detection_far_right;
    public float waypoint_distance, waypoint_bearing;
    public float future_waypoint_distance, future_waypoint_bearing;
}

[Serializable]
public struct PlayerCommands {
    public float steering, acceleration, brake, nitro;
    public String message;
}

[Serializable]
public struct EndLevelData {
    public float time;
    public int frames;
    public float top_speed;
}

public abstract class Interface : MonoBehaviour {
    public abstract void QueryEnv();
    public abstract void NewData(PlayerData data);
    public abstract bool HasCommands();
    public abstract PlayerCommands GetCommands();
    public abstract void EndLevel(EndLevelData data);
}
