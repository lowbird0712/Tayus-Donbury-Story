using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButtonScript : SFXButtonScript {
    [SerializeField] Text   stageText;
    [SerializeField] Text   neededDotoriNumText;
    [SerializeField] Image  unLockImage;
    [SerializeField] Image  unBlockImage;
    [SerializeField] int    stageNum = -1;
    [SerializeField] int    neededDotoriNum;
    [SerializeField] int    unLockRecipeIndex = -1;

    bool                    isCleared = false;

    public Text             StageText => stageText;
    public Text             NeededDotoriNumText => neededDotoriNumText;
    public Image            UnLockImage => unLockImage;
    public Image            UnBlockImage => unBlockImage;
    public int              UnLockRecipeIndex => unLockRecipeIndex;
    public bool             IsCleared { get => isCleared; set { isCleared = value; } }

    private void Start() {
        stageText.text = stageNum.ToString();
        neededDotoriNumText.text = neededDotoriNum.ToString();
    }

    public void UnBlock() => unBlockImage.enabled = false;

    public void StageButton() {
        if (MainGameMngScript.DotoriNum.Value < neededDotoriNum) {
            Clicked(2);
            MainGameMngScript.MessagePanel.Show(LocalizeManager.Localize("No Acorn!"));
            return;
        }
        else if (unLockImage.IsActive()) {
            Clicked(1);
            MainGameMngScript.DotoriNum.Value -= neededDotoriNum;
            StageMngScript.UnBlockNext(); ////
            neededDotoriNumText.enabled = false;
            unLockImage.enabled = false;
            if (unLockRecipeIndex != -1)
                RecipeBookMngScript.UnLockRecipe(unLockRecipeIndex);
            return;
        }
        Clicked(0);
        Utils.LoadScene("NaninovelScene");
        MainGameMngScript.StageNum = stageNum;
    }
}
