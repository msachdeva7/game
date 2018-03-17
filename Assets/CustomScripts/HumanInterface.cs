using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class HumanInterface : Interface {
    public override void QueryEnv() {
    }

    public override void NewData(PlayerData data) {
    }

    public override bool HasCommands() {
        return true;
    }

    public override PlayerCommands GetCommands() {
        PlayerCommands cmds;
        cmds.forward = Input.GetKey("w");
        cmds.backward = Input.GetKey("s");
        cmds.left = Input.GetKey("a");
        cmds.right = Input.GetKey("d");
        return cmds;
    }

    public override void EndLevel(EndLevelData data) {
        Debug.Log("Ending level");
        Debug.Log(JsonUtility.ToJson(data));
    }
}
