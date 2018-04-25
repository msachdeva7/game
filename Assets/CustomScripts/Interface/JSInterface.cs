using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;


[Serializable]
public struct QueryData {
    public bool human_override;
    public String script_name;
}


public class JSInterface : Interface {
    Interface proxy;
    public delegate void Callback(System.IntPtr ptr);

    [DllImport("__Internal")]
    private static extern string query_env(string data);

    [DllImport("__Internal")]
    private static extern void new_data(string data, Callback callback);

    [DllImport("__Internal")]
    private static extern void end_level(string data);

    static PlayerCommands cmds;
    static bool hasCmds = false;
    static QueryData query_data;

    public override void QueryEnv(GameManager gm) {
        Debug.Log("Querying JS environment");
        query_data = JsonUtility.FromJson<QueryData>(query_env("{}"));
        if (query_data.human_override) {
            Debug.Log("Interface override, proxying HumanInterface");
            proxy = gm.GetComponent<HumanInterface>();
            proxy.QueryEnv(gm);
        }
    }

    public override CarSetup Setup() {
        if (proxy != null) {
            return proxy.Setup();
        }
        else {
            HashAlgorithm algorithm = MD5.Create();
            byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(query_data.script_name));

            CarSetup cs;
            cs.color = new Color(Convert.ToSingle(hash[0]) / 255f, Convert.ToSingle(hash[1]) / 255f, Convert.ToSingle(hash[2]) / 255f);
            return cs;
        }
    }

    public override void NewData(PlayerData data) {
        if (proxy != null) {
            proxy.NewData(data);
        }
        else {
            new_data(JsonUtility.ToJson(data), SetCommands);
        }
    }

    public override bool HasCommands() {
        if (proxy != null) {
            return proxy.HasCommands();
        }
        else {
            return hasCmds;
        }
    }

    public override PlayerCommands GetCommands() {
        if (proxy != null) {
            return proxy.GetCommands();
        }
        else {
            hasCmds = false;
            return cmds;
        }
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
