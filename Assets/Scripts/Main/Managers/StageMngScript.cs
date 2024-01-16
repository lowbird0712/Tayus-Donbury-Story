using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMngScript : MonoBehaviour {
    static public StageMngScript Inst { get; set; } = null;

    [SerializeField] StageButtonScript[]    stageButtons;

    [Header("Test SerializeField")]
    [SerializeField] int                    usableMaxStageIndex;

    List<StageButtonScript>                 stageButtonList;
    int                                     maxUnlockIndex = 5;
    int                                     nextUnblockIndex = 1;

    static public int                       UsableMaxStageIndex => Inst.usableMaxStageIndex;
    static public List<StageButtonScript>   StageButtonList => Inst.stageButtonList;
    static public int                       MaxUnlockIndex { get => Inst.maxUnlockIndex; set { Inst.maxUnlockIndex = value; } }
    static public int                       NextUnblockIndex => Inst.nextUnblockIndex;

    private void Awake() {
        Inst = this;
        stageButtonList = new List<StageButtonScript>(stageButtons);
    }
    private void Start() {
        Init();
        gameObject.SetActive(false);
    }

    static public int GetUnlockedStageNum() {
        int num = 0;
        foreach (var button in Inst.stageButtonList) {
            if (button.UnLockImage.enabled == true)
                break;
            num++;
        }
        return num;
    }

    static public void UnBlockNext() {
        if (Inst.nextUnblockIndex >= Inst.stageButtonList.Count)
            return;
        if (Inst.maxUnlockIndex < Inst.nextUnblockIndex) {
            MainGameMngScript.MessagePanel.Show(LocalizeManager.Localize("Page Can't Unlock"));
            return;
        }
        Inst.stageButtonList[Inst.nextUnblockIndex].UnBlock();
        Inst.nextUnblockIndex++;
    }

    static public void StageClear(int _stageNum) {
        StageButtonList[_stageNum].IsCleared = true;
        UnBlockNext();
    }

    void Init() {
        SaveData data = SaveLoadMngScript.SaveData;
        maxUnlockIndex = 5 + data.travelNoteOpenPageNum * 5;
        nextUnblockIndex = data.unlockedStageNum - 1;
        for (int i = 0; i < data.unlockedStageNum; i++) {
            stageButtonList[i].UnBlockImage.enabled = false;
            stageButtonList[i].UnLockImage.enabled = false;
            stageButtonList[i].NeededDotoriNumText.enabled = false;
        }
        if (maxUnlockIndex == stageButtonList.Count - 1 && data.unlockedStageNum == stageButtonList.Count)
            return;
        else if (maxUnlockIndex == data.unlockedStageNum - 1)
            nextUnblockIndex++;
        else {
            stageButtonList[data.unlockedStageNum].UnBlockImage.enabled = false;
            nextUnblockIndex += 2;
        }
    }
}
