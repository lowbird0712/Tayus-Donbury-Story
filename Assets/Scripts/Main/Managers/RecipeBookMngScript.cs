using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum RecipeState {
    RS_UNLOCKED,
    RS_LOCKED
}

public class Recipe {
    public Sprite       recipeSprite;
    public string       recipeName;
    public int          stageNum;
    public int          indexInRecipeList;
    public RecipeState  state = RecipeState.RS_LOCKED;
}

public class RecipeBookMngScript : MonoBehaviour {
    static public RecipeBookMngScript   Inst { get; set; } = null;

    [SerializeField] Sprite             recipeEmptySprite;
    [SerializeField] Sprite             recipeLockedSprite;
    [SerializeField] Sprite[]           recipeSprites;
    [SerializeField] Button[]           recipeButtons;
    [SerializeField] Image[]            recipeImages;
    [SerializeField] TextMeshProUGUI[]  recipeNameTexts;
    [SerializeField] Text               localizedText;

    List<Recipe>                        recipeList = new List<Recipe> ();
    List<string>                        localizedRecipeListJP = new List<string>();
    int                                 leftTopRecipeIndex;

    static public Sprite[]              RecipeSprites => Inst.recipeSprites;
    static public List<Recipe>          RecipeList => Inst.recipeList;
    static public List<string>          LocalizedRecipeListJP => Inst.localizedRecipeListJP;

    static public int LeftTopRecipeIndex {
        get => Inst.leftTopRecipeIndex;
        set {
            Inst.leftTopRecipeIndex = value;
            for (int i = LeftTopRecipeIndex; i < LeftTopRecipeIndex + Inst.recipeImages.Length; i++) {
                if (i >= RecipeList.Count) {
                    Inst.recipeButtons[i - LeftTopRecipeIndex].interactable = false;
                    Inst.recipeImages[i - LeftTopRecipeIndex].sprite = Inst.recipeEmptySprite;
                    Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = "";
                    continue;
                }
                switch (RecipeList[i].state) {
                    case RecipeState.RS_UNLOCKED:
                        Inst.recipeButtons[i - LeftTopRecipeIndex].interactable = true;
                        Inst.recipeButtons[i - LeftTopRecipeIndex].GetComponent<PracticeButtonScript>().StageNum = RecipeList[i].stageNum;
                        Inst.recipeImages[i - LeftTopRecipeIndex].sprite = RecipeList[i].recipeSprite;
                        switch (LocalizeManager.Inst.currentLanguageIndex) {
                            case 1:
                                Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = RecipeList[i].recipeName;
                                break;
                            case 2:
                                Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = Inst.localizedRecipeListJP[i];
                                break;
                        }
                        break;
                    case RecipeState.RS_LOCKED:
                        Inst.recipeButtons[i - LeftTopRecipeIndex].interactable = false;
                        Inst.recipeImages[i - LeftTopRecipeIndex].sprite = Inst.recipeLockedSprite;
                        Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = Inst.localizedText.text;
                        break;
                }
            }
        }
    }

    private void Awake() => Inst = this;
    void Start() {
        Init();
        gameObject.SetActive(false);
    }

    static public int GetRecipeIndex(string _recipeName, int _localizeIndex = 1) {
        for (int i = 0; i < RecipeList.Count; i++) {
            switch (_localizeIndex) {
                case 0:
                    Debug.LogError("아직 영어 레시피명은 지원되고 있지 않습니다!!!!");
                    break;
                case 1:
                    if (RecipeList[i].recipeName == _recipeName)
                        return i;
                    break;
                case 2:
                    if (Inst.localizedRecipeListJP[i] == _recipeName)
                        return i;
                    break;
            }
        }
        return -1;
    }

    static public void UnLockRecipe(int _index) {
        Inst.recipeList[_index].state = RecipeState.RS_UNLOCKED;
        LeftTopRecipeIndex = 0;
    }

    void Init() {
        Recipe recipe = new Recipe();
        recipe.recipeSprite = recipeSprites[1];
        recipe.recipeName = "소고기 덮밥 -  기본";
        recipe.stageNum = 6;
        recipe.indexInRecipeList = 0;
        recipe.state = RecipeState.RS_LOCKED;
        recipeList.Add(recipe);
        localizedRecipeListJP.Add("牛丼 - 基本");
        recipe = new Recipe();
        recipe.recipeSprite = recipeSprites[2];
        recipe.recipeName = "소고기 덮밥 - 토핑 세트";
        recipe.stageNum = 11;
        recipe.indexInRecipeList = 1;
        recipe.state = RecipeState.RS_LOCKED;
        recipeList.Add(recipe);
        localizedRecipeListJP.Add("牛丼 - トッピングセット");
        recipe = new Recipe();
        recipe.recipeSprite = recipeSprites[3];
        recipe.recipeName = "소고기 덮밥 - 레드와인";
        recipe.stageNum = 16;
        recipe.indexInRecipeList = 2;
        recipe.state = RecipeState.RS_LOCKED;
        recipeList.Add(recipe);
        localizedRecipeListJP.Add("牛丼 - 赤ワイン");

        foreach (var button in StageMngScript.StageButtonList) {
            if (button.UnLockRecipeIndex == -1)
                continue;
            if (button.UnLockImage.enabled == true)
                break;
            recipeList[button.UnLockRecipeIndex].state = RecipeState.RS_UNLOCKED;
        }
        LeftTopRecipeIndex = 0;
    }

    public void LocalizeRefresh() => LeftTopRecipeIndex = LeftTopRecipeIndex;

    public void GoToLeft() {
        if (leftTopRecipeIndex >= 8)
            LeftTopRecipeIndex -= 8;
    }

    public void GoToRight() {
        if (leftTopRecipeIndex + 8 < recipeList.Count)
            LeftTopRecipeIndex += 8;
    }
}
