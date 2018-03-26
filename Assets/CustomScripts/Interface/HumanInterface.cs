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
        /*
        * control scheme is as follows:
        *
        * steering: A, LeftArrow - Turn left
        *           D, RightArrow - Turn right
        * throttle: W, UpArrow, LeftShift, E - throttle forward
        *           S, DownArrow, F - throttle backward
        * handbrake: X, Spacebar
        */

        cmds.forward = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.LeftShift);
        cmds.backward = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.F);
        cmds.left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        cmds.right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        cmds.handbrake = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Space);
        cmds.message = "";
        return cmds;
    }

    public override void EndLevel(EndLevelData data) {
        Debug.Log("Ending level");
        Debug.Log(JsonUtility.ToJson(data));
    }
}
