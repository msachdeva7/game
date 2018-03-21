using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerData {
    public float speed;
    public bool danger;
}

[Serializable]
public struct PlayerCommands {
    public bool forward, backward, left, right;
}

[Serializable]
public struct EndLevelData {
    public int time;
    public float top_speed;
}

public abstract class Interface : MonoBehaviour {
    public abstract void QueryEnv();
    public abstract void NewData(PlayerData data);
    public abstract bool HasCommands();
    public abstract PlayerCommands GetCommands();
    public abstract void EndLevel(EndLevelData data);
}
