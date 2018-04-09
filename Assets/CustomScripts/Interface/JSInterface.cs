using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;
using System.Runtime.InteropServices;


public class JSInterface : Interface {
    public delegate void Callback(System.IntPtr ptr);

    [DllImport("__Internal")]
    private static extern string query_env(string data);

    [DllImport("__Internal")]
    private static extern void new_data(string data, Callback callback);

    [DllImport("__Internal")]
    private static extern void end_level(string data);

    static PlayerCommands cmds;
    static bool hasCmds = false;

    public override void QueryEnv() {
        // Load data such as track id from webpage
        Debug.Log("Querying JS environment");
        Debug.Log(query_env("{}"));
    }

    public override void NewData(PlayerData data) {
        new_data(JsonUtility.ToJson(data), SetCommands);
    }

    public override bool HasCommands() {
        return hasCmds;
    }

    public override PlayerCommands GetCommands() {
        hasCmds = false;
        return cmds;
    }

    public override void EndLevel(EndLevelData data) {
        end_level(JsonUtility.ToJson(data));
    }

    [MonoPInvokeCallback(typeof(Callback))]
    public static void SetCommands(System.IntPtr ptr) {
        string value = Marshal.PtrToStringAuto(ptr);
        cmds = JsonUtility.FromJson<PlayerCommands>(value);
        hasCmds = true;
    }
}
