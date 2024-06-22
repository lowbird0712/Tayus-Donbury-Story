using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using TMPro;
using Naninovel;

public class WallpaperTimerMngScript : MonoBehaviour {
    static public WallpaperTimerMngScript   Inst { get; set; } = null;

    [Header("UI Settings")]
    [SerializeField] Button                 rewardButton;
    [SerializeField] TextMeshProUGUI        timerText;
    [SerializeField] TextMeshProUGUI        rewardNumText;
    [Header("Client Settings")]
    [SerializeField] const int              startTime = 19;
    [SerializeField] const int              endTime = 23;
    [SerializeField] const int              rewardDotoriNum = 1;
    [SerializeField] const int              maxRewardNum = 4;
    [SerializeField] const int              rewardSeconds = 1800;

    ReactiveProperty<int>                   rewardNum = new ReactiveProperty<int>(0);
    int                                     constRewardNum = 0;
    int                                     timerSec = 0;
    ReactiveProperty<int>                   constTimerSec = new ReactiveProperty<int>(0);
    SerializedDateTime                      lastPlayDateTime;
    DateTime                                tmp = default;
    bool                                    timerOn;

    static public int                       RewardNum => Inst.rewardNum.Value;
    static public int                       ConstTimerSec => Inst.constTimerSec.Value;


    private void Awake() => Inst = this;

    private void Start() {
        rewardNum
            .Subscribe(num =>
            {
                rewardButton.interactable = num > 0;
                rewardNumText.text = num.ToString();
            })
            .AddTo(this);
        constTimerSec
            .Subscribe(_ => timerText.text = string.Format("{0:D2} : {1:D2}", constTimerSec.Value / 60, constTimerSec.Value % 60))
            .AddTo(this);
        SetupTimer();
    }

    private void Update() {
        if (!timerOn &&
            startTime <= DateTime.Now.Hour && DateTime.Now.Hour < endTime &&
            constTimerSec.Value < maxRewardNum * rewardSeconds)
        {
            StartTimer();
        }
        if (SceneManager.GetActiveScene().name != "MainScene" || !timerOn)
            return;
        if (DateTime.Now.Hour >= endTime || tmp.Day < DateTime.Now.Day || tmp.Month < DateTime.Now.Month)
        {
            StopTimer();
            return;
        }
        if ((int)(DateTime.Now - tmp).TotalSeconds < 1)
            return;
        timerSec++;
        constTimerSec.Value++;
        tmp = DateTime.Now;
        if (timerSec >= rewardSeconds)
            AddRewardDotori();
    }

    void SetupTimer()
    {
        lastPlayDateTime = SaveLoadMngScript.SaveData.lastPlayDateTime;
        rewardNum.Value = SaveLoadMngScript.SaveData.wallPaperRewardNum;
        if (lastPlayDateTime.day != DateTime.Now.Day || lastPlayDateTime.month != DateTime.Now.Month)
        {
            lastPlayDateTime.timerSec = 0;
            rewardNum.Value = 0;
        }
        constRewardNum = lastPlayDateTime.timerSec / rewardSeconds;
        timerSec = lastPlayDateTime.timerSec % rewardSeconds;
        constTimerSec.Value = lastPlayDateTime.timerSec;
    }

    void StartTimer() {
        tmp = DateTime.Now;
        timerOn = true;
        timerSec = lastPlayDateTime.timerSec % rewardSeconds;
        constTimerSec.Value = lastPlayDateTime.timerSec;
    }

    void StopTimer() => timerOn = false;

    void AddRewardDotori()
    {
        rewardNum.Value++;
        constRewardNum++;
        timerSec = 0;
        if (constRewardNum == maxRewardNum)
            StopTimer();
    }

    public void OnClickRewardButton()
    {
        rewardNum.Value--;
        rewardButton.GetComponent<FullTimerButtonScript>().FullTimerButtonClicked();
    }
}
