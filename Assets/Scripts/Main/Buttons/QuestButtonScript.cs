using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestButtonScript : SFXButtonScript {
    [SerializeField] Image  clearedImage;
    [SerializeField] int    questIndex;

    int                     stageNum = -1;

    public Image            ClearedImage => clearedImage;
    public int              StageNum { get => stageNum; set { stageNum = value; } }
    public bool             Cleared() => clearedImage.gameObject.activeSelf;

    public void MakeClearedQuest() {
        clearedImage.gameObject.SetActive(true);
        GetComponent<Button>().interactable = false;
    }

    public void MakeUnClearedQuest() {
        clearedImage.gameObject.SetActive(false);
        GetComponent<Button>().interactable = true;
    }

    public void QuestButton() {
        MainGameMngScript.StageNum = stageNum;
        TayuBoxMngScript.QuestStageIndex = questIndex;
        Utils.LoadScene("CardGameScene");
    }
}