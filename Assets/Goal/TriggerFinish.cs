using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFinish : MonoBehaviour {
    public CustomCarControl carcontrol;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            carcontrol.EndLevel();
        }
    }
}
