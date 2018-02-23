using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class JSInterface : Interface {
    [DllImport("__Internal")]
    private static extern string query_env(string data);

    [DllImport("__Internal")]
    private static extern string control(string data);

    public override void QueryEnv() {
        // Load data such as track id from webpage
        Debug.Log("Querying JS environment");
        Debug.Log(query_env("{}"));
    }

    public override PlayerCommands Control(PlayerData data) {
        // Send data to JS and get commands back
        PlayerCommands cmds = JsonUtility.FromJson<PlayerCommands>(control(JsonUtility.ToJson(data)));
        return cmds;
    }
}
