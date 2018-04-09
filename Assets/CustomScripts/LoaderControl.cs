using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;


[Serializable]
public struct TrackData {
    public String title, desc, level;
    public Sprite image;
}


public class LoaderControl : MonoBehaviour {
    public Button leftButton, rightButton, loadButton;
    public Text titleText, descText;
    public Image trackImage;
    public TrackData[] data;

    int index = 0;

    void Start() {
        leftButton.onClick.AddListener(GoLeft);
        rightButton.onClick.AddListener(GoRight);
        loadButton.onClick.AddListener(LoadTrack);
        trackImage.preserveAspect = true;
        SetTrackData();
    }

    void GoLeft() {
        index = (index - 1 + data.Length) % data.Length;
        SetTrackData();
    }

    void GoRight() {
        index = (index + 1) % data.Length;
        SetTrackData();
    }

    void SetTrackData() {
        titleText.text = data[index].title;
        descText.text = data[index].desc;
        trackImage.sprite = data[index].image;
    }

    void LoadTrack() {
        GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(data[index].level);
    }
}
