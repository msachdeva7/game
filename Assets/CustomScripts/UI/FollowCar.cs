using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCar : MonoBehaviour {
    public Transform car;

    // Use this for initialization
    void Start () {
        car = GameObject.Find("Car").transform;
    }

    // Update is called once per frame
    void Update () {
        Vector3 newPosition = car.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, car.eulerAngles.y, 0f);
    }
}
