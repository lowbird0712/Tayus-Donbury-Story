using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeButtonScript : SFXButtonScript {
    int         stageNum = -1;

    public int  StageNum { get => stageNum; set => stageNum = value; }

    public void PracticeButton() {
        MainGameMngScript.StageNum = stageNum;
        MainGameMngScript.IsPractice = true;
        Utils.LoadScene("CardGameScene");
    }
}
