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
public struct TransformData {
    public Vector3 position;
    public Quaternion rotation;
    public TransformData(Vector3 pos, Quaternion rot) {
        position = pos;
        rotation = rot;
    }
}

[Serializable]
public struct PlayerData {
    public float speed;
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

public struct CarSetup {
    public Color color;
}

public struct GhostCarSetup {
    public List<TransformData> history;
    public int historyFramerate; //record this so old runs can still be used if we decide to change the framerate
}

[Serializable]
public struct EndLevelData {
    public float time, top_speed, fuel_used, distance_travelled, average_speed, nitro_usage;
    public int frames;
    public String track;
    public List<TransformData> history;
    public int historyFramerate; //record this so old runs can still be used if we decide to change the framerate
}

public abstract class Interface : MonoBehaviour {
    public abstract void QueryEnv(GameManager gm);
    public abstract CarSetup Setup();
    public abstract GhostCarSetup GhostSetup();
    public abstract void NewData(PlayerData data);
    public abstract bool HasCommands();
    public abstract PlayerCommands GetCommands();
    public abstract void EndLevel(EndLevelData data);
}
