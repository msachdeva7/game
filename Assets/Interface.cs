using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerData {
    public float speed;
}

[Serializable]
public struct PlayerCommands {
    public bool forward, backward, left, right;
}

public abstract class Interface : MonoBehaviour {
    public abstract void QueryEnv();
    public abstract PlayerCommands Control(PlayerData data);
}
