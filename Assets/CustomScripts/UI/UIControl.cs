using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;


public class UIControl : MonoBehaviour {
    public Text endLevelText, scriptText, timeText, floatText;
    public FuelConverter fuel_converter;
    public SensorControl sensor_control;
    public SpeedConverter speed_converter;

    float floatFadeAmount = 1;
    public float floatFadeFactor = 4; // reverse exponential fade
    public float floatFadeTime;

    void Start() {
        if (new List<UnityEngine.Object>{endLevelText, scriptText, timeText, floatText, fuel_converter, sensor_control, speed_converter}.Contains(null)) {
            Debug.Log("Error: Malformed UI!");
        }
    }

    void Update() {
        floatText.color = new Color(1f, 1f, 1f, (float)(1 - (Math.Pow(floatFadeFactor, floatFadeAmount) - 1) / (floatFadeFactor - 1)));
        floatFadeAmount = Math.Min(1, floatFadeAmount + 1 / floatFadeTime * Time.fixedDeltaTime);
    }

    public void ShowData(PlayerData data) {
        timeText.text = Math.Floor(data.time / 60) + ":" + (data.time % 60 < 10 ? "0" : "") + Math.Floor(data.time % 60);
        speed_converter.ShowSpeed(data.speed);
        fuel_converter.ShowFuel(data.nitro_left);
        sensor_control.ShowSensors(data.obstacle_detection_rays);
    }

    public void ShowCommands(PlayerCommands cmds) {
        scriptText.text = cmds.message;
    }

    public void EndLevel(EndLevelData data) {
        endLevelText.text = "Time: " + data.time + " s\nMax speed: " + data.top_speed + " m/s";
    }

    public void FloatingText(String text) {
        floatText.text = text;
        floatFadeAmount = 0;
    }
}
