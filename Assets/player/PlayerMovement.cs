using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public Rigidbody rb;
    public float speed;
    bool waitingForCommands = false;

    GameManager gm;

    // Use this for initialization
    void Start () {
        speed *= Time.deltaTime;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm == null) {
            Debug.Log("Cannot find GM");
        }
    }

    // Update is called once per frame
    void Update () {
        if (waitingForCommands && gm.inter.HasCommands()) {
            waitingForCommands = false;

            PlayerCommands cmds = gm.inter.GetCommands();

            if (cmds.forward) {
                rb.AddForce(0, 0, speed);
            }
            if (cmds.backward) {
                rb.AddForce(0, 0, -speed);
            }
            if (cmds.left) {
                rb.AddForce(-speed, 0, 0);
            }
            if (cmds.right) {
                rb.AddForce(speed, 0, 0);
            }
        }

        if (!waitingForCommands) {
            PlayerData data;
            data.speed = speed;
            gm.inter.NewData(data);
            waitingForCommands = true;
        }
    }
}
