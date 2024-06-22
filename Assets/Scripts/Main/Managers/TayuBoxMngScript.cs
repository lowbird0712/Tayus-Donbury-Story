using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class TayuBoxMngScript : MonoBehaviour { 
    static public TayuBoxMngScript          Inst { get; set; } = null;

    [SerializeField] QuestButtonScript[]    questButtons;
    [SerializeField] Image[]                questImages;
    [SerializeField] TextMeshProUGUI[]      questTexts;

    int                                     questStageIndex = -1;
    DateTime                                lastPlayDateTime = default;

    static public int                       QuestStageIndex { get => Inst.questStageIndex; set { Inst.questStageIndex = value; } }

    private void Awake() => Inst = this;
    void Start() {
        Init();
        gameObject.SetActive(false);
        if (DateTime.Now.Day != lastPlayDateTime.Day || DateTime.Now.Month != lastPlayDateTime.Month || DateTime.Now.Year != lastPlayDateTime.Year || lastPlayDateTime == default)
            SetDailyQuest();
        lastPlayDateTime = DateTime.Now;
    }

    public void SetDailyQuestCheatButton() => SetDailyQuest();

    static public List<QuestData> GetQuestDataList() {
        List<QuestData> list = new List<QuestData>();
        QuestData       data;
        for (int i = 0; i < Inst.questButtons.Length; i++) {
            data = new QuestData();
            data.recipeIndex = RecipeBookMngScript.GetRecipeIndex(Inst.questTexts[i].text, LocalizeManager.CurrentLanguageIndex);
            if (data.recipeIndex == -1)
                continue;
            data.cleared = Inst.questButtons[i].Cleared();
            list.Add(data);
        }
        return list;
    }

    static public void QuestClear() {
        Inst.questButtons[Inst.questStageIndex].MakeClearedQuest();
        Inst.questStageIndex = -1;
        MainGameMngScript.DotoriNum.Value++;
    }

    void Init() {
        SerializedDateTime sDateTime = SaveLoadMngScript.SaveData.lastPlayDateTime;
        lastPlayDateTime = new DateTime(sDateTime.year, sDateTime.month, sDateTime.day);
        List<QuestData> questDataList = SaveLoadMngScript.SaveData.questDataList;
        QuestData       questData;
        Recipe          recipe;
        for (int i = 0; i < questDataList.Count; i++) {
            questData = questDataList[i];
            recipe = RecipeBookMngScript.RecipeList[questData.recipeIndex];
            questImages[i].sprite = recipe.recipeSprite;
            switch (LocalizeManager.CurrentLanguageIndex) {
                case 0:
                    Debug.LogError("?? ??? ???? ?? ????!!!!");
                    break;
                case 1:
                    questTexts[i].text = recipe.recipeName;
                    break;
                case 2:
                    questTexts[i].text = RecipeBookMngScript.LocalizedRecipeListJP[i];
                    break;
            }
            questButtons[i].StageNum = recipe.stageNum;
            if (!questDataList[i].cleared)
                questButtons[i].MakeUnClearedQuest();
            else
                questButtons[i].ClearedImage.gameObject.SetActive(true);
        }
    }

    void SetDailyQuest() {
        List<Recipe>    recipeList = new List<Recipe> (RecipeBookMngScript.RecipeList);
        List<Recipe>    unLockRecipeList = new List<Recipe>();
        List<string>    questNameList = new List<string>();
        Recipe          recipe;
        int             filledQuestNum = 0;
        foreach (Recipe iter in recipeList) {
            if (iter.state == RecipeState.RS_UNLOCKED)
                unLockRecipeList.Add(iter);
        }
        for (int i = 0; i < questTexts.Length; i++) {
            if (unLockRecipeList.Count == 0 || filledQuestNum == questTexts.Length)
                break;
            questButtons[i].MakeUnClearedQuest();
            questButtons[i].GetComponent<Button>().interactable = false;
            do
                recipe = unLockRecipeList[Random.Range(0, unLockRecipeList.Count)];
            while (questNameList.Contains(recipe.recipeName));
            questButtons[i].GetComponent<Button>().interactable = true;
            questButtons[i].StageNum = recipe.stageNum;
            questImages[i].sprite = recipe.recipeSprite;
            switch (LocalizeManager.CurrentLanguageIndex) {
                case 0:
                    Debug.LogError("?? ??? ???? ?? ????!!!!");
                    break;
                case 1:
                    questTexts[i].text = recipe.recipeName;
                    break;
                case 2:
                    questTexts[i].text = RecipeBookMngScript.LocalizedRecipeListJP[recipe.indexInRecipeList];
                    break;
            }
            unLockRecipeList.Remove(recipe);
            questNameList.Add(recipe.recipeName);
            filledQuestNum++;
        }
    }

    public void LocalizeRefresh() => Init();
}
