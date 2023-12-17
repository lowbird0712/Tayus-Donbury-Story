using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using TMPro;

public class WallpaperTimerMngScript : MonoBehaviour {
    static public WallpaperTimerMngScript Inst { get; set; } = null;

    [SerializeField] Button             dotoriButton;
    [SerializeField] TextMeshProUGUI    timerText;

    ReactiveProperty<int>               timerSec = new ReactiveProperty<int>(0);
    DateTime                            tmp = default;
    bool                                timerOn;

    static public int TimerSec => Inst.timerSec.Value;


    private void Awake() => Inst = this;

    private void Start() {
        timerSec
            .Subscribe(_ => UpdateTimerText());

        if (19 > DateTime.Now.Hour || DateTime.Now.Hour >= 23) {
            timerSec.Value = 0;
            return;
        }
        timerSec.Value = SaveLoadMngScript.SaveData.lastPlayDateTime.timerSec;
        StartTimer();
    }

    private void Update() {
        if (SceneManager.GetActiveScene().name != "MainScene" || !timerOn)
            return;
        if ((int)(DateTime.Now - tmp).TotalSeconds < 1)
            return;
        timerSec.Value++;
        tmp = DateTime.Now;
        if (timerSec.Value >= 3600)
            FullTimer();
        else if (DateTime.Now.Hour >= 23)
            StopTimer();
        else if (!timerOn && DateTime.Now.Hour >= 19)
            StartTimer();
    }

    void UpdateTimerText() => timerText.text = string.Format("{0:D2} : {1:D2}", timerSec.Value / 60, timerSec.Value % 60);

    void StartTimer() {
        tmp = DateTime.Now;
        timerOn = true;
    }

    void StopTimer() {
        timerOn = false;
    }

    void FullTimer() {
        dotoriButton.interactable = true;
        timerSec.Value = 0;
        StopTimer();
    }
}
