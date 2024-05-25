using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class CardGameMngScript : MonoBehaviour {   
    public static CardGameMngScript Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField][Tooltip("게임의 속도가 매우 빨라집니다")] bool                         fastMode;
    [SerializeField][Tooltip("시작 카드 개수를 정합니다")] int                              startCardCount;
    [SerializeField][Tooltip("패에 들고 있을 수 있는 최대 카드 개수를 정합니다")] int       maxCardCount;
    [SerializeField][Tooltip("첫 턴에 놓을 수 있는 카드의 개수를 정합니다")] int            startPutCardCount;
    [SerializeField][Tooltip("놓을 수 있는 카드의 최대 개수를 정합니다")] int               maxPutCardCount;

    [SerializeField] TurnStartPanelScript                                                   turnStartPanel;
    [SerializeField] TextMeshProUGUI                                                        turnNumText;
    [SerializeField] TextMeshProUGUI                                                        currentStageInfoText;
    [SerializeField] PanelScript                                                            cardExplainPanel;
    [SerializeField] TurnStartPanelScript                                                   startGamePanel;
    [SerializeField] TextMeshProUGUI stageKindText;
    [SerializeField] Image                                                                  menuImage;
    [SerializeField] TextMeshProUGUI                                                        menuNameText;
    [SerializeField] TextMeshProUGUI                                                        goalNumText;
    [SerializeField] TextMeshProUGUI                                                        maxTurnNumText;


    [Header("게임 시스템 변수")]
    public bool                                                                             isLoading;
    public List<bool>                                                                       isCoroutine = new List<bool>();

    bool myTurn;
    Dictionary<string, string>                                                              menuInfo = new Dictionary<string, string>();
    Dictionary<string, string>                                                              recipeExplanationInfo = new Dictionary<string, string>();
    Dictionary<string, int>                                                                 stageInfo = new Dictionary<string, int>();
    ReactiveDictionary<string, int>                                                         currentStageInfo = new ReactiveDictionary<string, int>();
    string                                                                                  recipeString;
    string                                                                                  tipString;
    int                                                                                     stageNum = -1;
    int                                                                                     maxTurnNum;
    ReactiveProperty<int>                                                                   turnNum = new ReactiveProperty<int>();

    static public int                                                                       MaxCardCount => Inst.maxCardCount;
    static public int                                                                       StartPutCardCount => Inst.startPutCardCount;
    static public int                                                                       MaxPutCardCount => Inst.maxPutCardCount;
    static public bool                                                                      MyTurn => Inst.myTurn;
    static public Dictionary<string, int>                                                   StageInfo => Inst.stageInfo;
    static public ReactiveDictionary<string, int>                                           CurrentStageInfo => Inst.currentStageInfo;
    static public PanelScript                                                               CardExplainPanel => Inst.cardExplainPanel;
    static public int StageNum {
        get => Inst.stageNum;
        set { Inst.stageNum = value; }
    }

    public static List<bool> IsCoroutine => Inst.isCoroutine;
    public void WrapperEndTurn() => StartCoroutine(EndTurnCo());

    void Start() {
        MainGameMngScript.SendStageNum();
        turnNum
            .Subscribe(x => {
                switch (LocalizeManager.CurrentLanguageIndex) {
                    case 0:
                        Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                        break;
                    case 1:
                        turnNumText.text = "남은 턴 : " + (maxTurnNum - x + 1).ToString();
                        break;
                    case 2:
                        turnNumText.text = "残りターン : " + (maxTurnNum - x + 1).ToString();
                        break;
                }
            });
        currentStageInfo
            .ObserveReplace()
            .Subscribe(_ => CurrentStageInfoTextSet());
#if UNITY_EDITOR
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.O))
            .Subscribe(_ => CardMngScript.AddCardItem());
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.E))
            .Subscribe(_ => StartCoroutine(EndTurnCo()));
#endif
        StartCoroutine(StartGameCo());
    }

    private void OnDestroy() {
        if (fastMode) {
            Utils.cardDrawDotweenTime /= Utils.fastModeFloat;
            Utils.cardDrawExtraTime /= Utils.fastModeFloat;
            Utils.turnStartPanelAppendDotweenTIme /= Utils.fastModeFloat;
            Utils.turnStartPanelUpDownDotweenTime /= Utils.fastModeFloat;
            Utils.cardExecDotweenTime /= Utils.fastModeFloat;
        }
    }

    static public void CurrentStageInfoTextSet() {
        string text = "";
        foreach (var keyValue in Inst.stageInfo)
            text += Inst.menuInfo[keyValue.Key] + " : " + Inst.currentStageInfo[keyValue.Key] + "/" + Inst.stageInfo[keyValue.Key];
        Inst.currentStageInfoText.text = text;
    }
    public void ShowRecipeString() => CardExplainPanel.Show(recipeString);
    public void ShowTipString() => CardExplainPanel.Show(tipString);

    void GameSetup(int _stageNum) {
        if (fastMode) {
            Utils.cardDrawDotweenTime *= Utils.fastModeFloat;
            Utils.cardDrawExtraTime *= Utils.fastModeFloat;
            Utils.turnStartPanelAppendDotweenTIme *= Utils.fastModeFloat;
            Utils.turnStartPanelUpDownDotweenTime *= Utils.fastModeFloat;
            Utils.cardExecDotweenTime *= Utils.fastModeFloat;
        }
        RecipeExplanationInit();
        Init(_stageNum);
        StartGamePanelInit();
        CardMngScript.Init(_stageNum);
    }

    void RecipeExplanationInit() {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되지 않고 있습니다!!!!");
                break;
            case 1:
                recipeExplanationInfo["밥"] =
                    "밥 짓기의 기본!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n\n" +
                    "재료와 도구를 배치하는 법!.\n" +
                    "1. 카드를 상단의 공간에 드래그 해 놓는다.\n" +
                    "2. 턴 종료 시 왼쪽 위부터 차례대로 재료와 도구가 배치된다.";
                recipeExplanationInfo["맛있는 밥"] =
                            "맛있는 밥을 짓는 법!\n" +
                            "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                            "주재료 : 쌀\n" +
                            "조미료 : 도토리주\n\n" +
                            "조미료를 만드는 법!.\n" +
                            "1. 밥솥, 쌀, 불지피기를 뒤집어 배치해 도토리솥, 도토리, 도토리 굽기를 얻는다.\n" +
                            "2. 도토리솥에 도토리를 넣고 불을 올린다."; ;
                recipeExplanationInfo["소고기 덮밥 기본"] =
                            "소고기 덮밥 기본 레시피\n" +
                            "1. 냄비에 아래의 재료를 넣고 끓인다.\n" +
                            "주재료 : 양파\n" +
                            "조미료 : 간장, 설탕, 해초도토리\n" +
                            "2. 끓인 후의 냄비에 아래의 재료를 넣고 다시 끓인다.\n" +
                            "주재료 : 우삼겹"; ;
                recipeExplanationInfo["소고기 덮밥 토핑 세트"] = // TODO: 레시피 설명 수정 필요!!!!
                            "도토리를 사용하는 법!\n" +
                            "1. 카드를 뒤집어 도토리 오브젝트 배치가 가능한 도토리 카드를 만든다.\n" +
                            "2. 도토리 : X 가 적혀 있는 카드를 사용하면 필드에 배치된 도토리 X개를 사용해서 효과를 발동할 수 있다.\n\n" +
                            "소고기 덮밥에 어울리는 토핑!\n" +
                            "1. 토핑 추가를 사용하면 조미료를 미리 만들어 덱에 추가할 수 있다.\n" +
                            "2. 이 방법을 사용하면 턴을 기다릴 필요 없이 바로 덱에 조미료를 넣는 것이 가능하다.\n" +
                            "3. 소고기 덮밥 토핑 세트 덱에는 여분의 카드가 많이 들어있으니 뒤집어서 도토리 오브젝트로서 활용해보자.";
                recipeExplanationInfo["소고기 덮밥 레드와인"] = // TODO: 레시피 설명 수정 필요!!!!
                            "레드와인을 만드는 법!\n" +
                            "1. 도토리 오브젝트 하나를 그리드에 배치한다.\n" +
                            "2. 오크통 배치를 사용하면 도토리 오브젝트가 있던 위치에 오크통을 놓을 수 있다.\n" +
                            "3. 오크통에 포도와 조미료의 도토리 카드인 향기로운 도토리를 하나씩 넣는다.\n" +
                            "4. 오크통 배치의 도토리 카드인 오크통 밀폐를 사용해 레드와인의 양조를 시작할 수 있다.\n\n" +
                            "레드와인의 고급스런 향!\n" +
                            "소고기 덮밥 레드와인 덱에서는 냄비에 양파뿐만 아니라 레드와인, 생강을 넣어주어야 한다.\n" +
                            "오크통으로 미리 레드와인을 만들어 놓도록 하자.";
                break;
            case 2:
                recipeExplanationInfo["밥"] =
                    "飯炊きの基本!\n" +
                    "1. めし釜に以下の食材を入れ, 火をつける.\n" +
                    "メイン食材 : 米\n\n" +
                    "食材と道具を置く方法!\n" +
                    "1. カードを上の空間にドラッグして置く.\n" +
                    "2. ターン終了時, 左上から順番に食材と道具が置かれる.";
                recipeExplanationInfo["맛있는 밥"] =
                            "美味しい飯の炊き方!\n" +
                            "1. めし釜に以下の食材とを入れ, 火をつける.\n" +
                            "メイン食材 : 米\n" +
                            "調味料 : どんぐり酒\n\n" +
                            "調味料の作り方.\n" +
                            "1. めし釜, 米, 火つけるカードを振り返え配置し, どんぐり釜, どんぐり, どんぐり焼くを得る.\n" +
                            "2. どんぐり釜にどんぐりを入れ, 火をつける."; ;
                recipeExplanationInfo["소고기 덮밥 기본"] =
                            "牛丼の基本的なレシピ\n" +
                            "1. 鍋に以下の食材を入れ, 煮込む.\n" +
                            "メイン食材 : 玉ねぎ\n" +
                            "調味料 : 醤油, 砂糖, 海草どんぐり\n" +
                            "2. 煮込み終えた後の鍋に以下の食材を入れ, また煮込む.\n" +
                            "メイン食材 : 牛バラ肉"; ;
                recipeExplanationInfo["소고기 덮밥 토핑 세트"] = // TODO: 로컬라이징 필요!!!!
                            "도토리를 사용하는 법!\n" +
                            "1. 카드를 뒤집어 도토리 오브젝트 배치가 가능한 도토리 카드를 만든다.\n" +
                            "2. 도토리 : X 가 적혀 있는 카드를 사용하면 필드에 배치된 도토리 X개를 사용해서 효과를 발동할 수 있다.\n\n" +
                            "소고기 덮밥에 어울리는 토핑!\n" +
                            "1. 토핑 추가를 사용하면 조미료를 미리 만들어 덱에 추가할 수 있다.\n" +
                            "2. 이 방법을 사용하면 턴을 기다릴 필요 없이 바로 덱에 조미료를 넣는 것이 가능하다.\n" +
                            "3. 소고기 덮밥 토핑 세트 덱에는 여분의 카드가 많이 들어있으니 뒤집어서 도토리 오브젝트로서 활용해보자.";
                recipeExplanationInfo["소고기 덮밥 레드와인"] = // TODO: 로컬라이징 필요!!!!
                            "레드와인을 만드는 법!\n" +
                            "1. 도토리 오브젝트 하나를 그리드에 배치한다.\n" +
                            "2. 오크통 배치를 사용하면 도토리 오브젝트가 있던 위치에 오크통을 놓을 수 있다.\n" +
                            "3. 오크통에 포도와 조미료의 도토리 카드인 향기로운 도토리를 하나씩 넣는다.\n" +
                            "4. 오크통 배치의 도토리 카드인 오크통 밀폐를 사용해 레드와인의 양조를 시작할 수 있다.\n\n" +
                            "레드와인의 고급스런 향!\n" +
                            "소고기 덮밥 레드와인 덱에서는 냄비에 양파뿐만 아니라 레드와인, 생강을 넣어주어야 한다.\n" +
                            "오크통으로 미리 레드와인을 만들어 놓도록 하자.";
                break;
        }
    }

    #region 스테이지 목표 설정
    void Init(int _stageNum) {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                break;
            case 1:
                switch (_stageNum) {
                    case 0:
                        // 밥 1개 만들기, 제한 50턴
                        menuInfo.Add("", "밥");
                        stageInfo.Add("", 1);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["밥"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "밥";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 1:
                        // 밥 2개 만들기, 제한 50턴
                        menuInfo.Add("", "밥");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["밥"];
                        tipString = "카운트다운을 활용해서 턴 수를 절약하자!";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "밥";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 2:
                        // 밥 2개 만들기, 제한 30턴
                        menuInfo.Add("", "밥");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["밥"];
                        tipString = "1. 한 턴에 여러 개의 카드를 사용해서 턴 수를 절약하자!\n" +
                            "2. 오브젝트에 사용하는 카드는 이전 턴에 그 오브젝트가 배치되어 있지 않으면 사용 불가!";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "밥";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 3:
                        // 맛있는밥 1개 만들기, 제한 50턴
                        menuInfo.Add("", "맛있는 밥");
                        stageInfo.Add("", 1);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["맛있는 밥"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "맛있는 밥";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 4:
                        // 맛있는밥 2개 만들기, 제한 50턴
                        menuInfo.Add("", "맛있는 밥");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["맛있는 밥"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "맛있는 밥";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 5:
                        // 맛있는밥 2개 만들기, 제한 30턴
                        menuInfo.Add("", "맛있는 밥");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["맛있는 밥"];
                        tipString = "도토리 솥의 조리 완료까지는 여러 턴이 걸리니까, 미리 조미료를 준비해 놓자!";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "맛있는 밥";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 6:
                        // 소고기 덮밥 기본 1개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 기본");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        tipString = "1. 레시피나 카드게임 팁이 잘 기억나지 않으면 오른쪽 위의 팁, 레시피 버튼을 눌러 확인하자!\n" +
                            "2. 패가 최대 매수(10장)일 경우 카드를 패에 추가할 수 없고, 카드를 생성해 패에 추가하는 상황이라면 생성된 카드가 사라지니 주의하자!";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "소고기 덮밥 - 기본";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 7:
                        // 소고기 덮밥 기본 1개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 기본");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        tipString = "오브젝트를 놓을 때에는 앞으로의 오브젝트 배치 상황을 생각하자!";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "소고기 덮밥 - 기본";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 8:
                        // 소고기 덮밥 기본 1개 만들기, 제한 30턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 기본");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "소고기 덮밥 - 기본";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 9:
                        // 소고기 덮밥 기본 2개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 기본");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "소고기 덮밥 - 기본";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 10:
                        // 소고기 덮밥 기본 2개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 기본");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "소고기 덮밥 - 기본";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 11:
                        // 소고기 덮밥 토핑 세트 1개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 토핑 세트");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 토핑 세트";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 12:
                        // 소고기 덮밥 토핑 세트 1개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 토핑 세트");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 토핑 세트";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 13:
                        // 소고기 덮밥 토핑 세트 1개 만들기, 제한 30턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 토핑 세트");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 토핑 세트";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 14:
                        // 소고기 덮밥 토핑 세트 2개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 토핑 세트");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 토핑 세트";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 15:
                        // 소고기 덮밥 토핑 세트 2개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 토핑 세트");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 토핑 세트";
                        goalNumText.text = "목표 개수 : " + 2;
                        break;
                    case 16:
                        // 소고기 덮밥 레드와인 1개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 레드와인");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 레드와인";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 17:
                        // 소고기 덮밥 레드와인 1개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 레드와인");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 레드와인";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 18:
                        // 소고기 덮밥 레드와인 1개 만들기, 제한 30턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 레드와인");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 레드와인";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 19:
                        // 소고기 덮밥 레드와인 2개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 레드와인");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 레드와인";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                    case 20:
                        // 소고기 덮밥 레드와인 2개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "소고기 덮밥 - 레드와인");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "소고기 덮밥 - 레드와인";
                        goalNumText.text = "목표 개수 : " + 1;
                        break;
                }
                break;
            case 2:
                switch (_stageNum) {
                    case 0:
                        // 밥 1개 만들기, 제한 50턴
                        menuInfo.Add("", "ご飯");
                        stageInfo.Add("", 1);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["밥"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "ご飯";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 1:
                        // 밥 2개 만들기, 제한 50턴
                        menuInfo.Add("", "ご飯");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["밥"];
                        tipString = "カウントダウンを活用してターン数を節約しよう！";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "ご飯";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 2:
                        // 밥 2개 만들기, 제한 30턴
                        menuInfo.Add("", "ご飯");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["밥"];
                        tipString = "１。一つのターンに複数のカードを使ってターン数を節約しよう！\n" +
                            "２。オブジェクトに使うカードは前のターンにそのオブジェクトが配置されてないのであれば使えない！";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "ご飯";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 3:
                        // 맛있는밥 1개 만들기, 제한 50턴
                        menuInfo.Add("", "美味しいご飯");
                        stageInfo.Add("", 1);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["맛있는 밥"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "美味しいご飯";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 4:
                        // 맛있는밥 2개 만들기, 제한 50턴
                        menuInfo.Add("", "美味しいご飯");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["맛있는 밥"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "美味しいご飯";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 5:
                        // 맛있는밥 2개 만들기, 제한 30턴
                        menuInfo.Add("", "美味しいご飯");
                        stageInfo.Add("", 2);
                        currentStageInfo.Add("", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["맛있는 밥"];
                        tipString = "どんぐり釜の調理完了までは複数ターンがかかるので、先に調味料を用意しておこう！";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[0];
                        menuNameText.text = "美味しいご飯";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 6:
                        // 소고기 덮밥 기본 1개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 基本");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        tipString = "1. レシピやカードゲームのコツがよく覚えられなかったなら、右上のコツ、レシピボタンを推して確認しよう！\n" +
                            "2. 手札が最大枚数(10枚)だった場合、カードを手札に追加するができなく、カードを生成して追加する状況だったらそのカードが消えるから注意しよう！";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "牛丼 - 基本";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 7:
                        // 소고기 덮밥 기본 1개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 基本");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        tipString = "オブジェクトを置く時には、今後のオブジェクト配置状況を考えましょう！";
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "牛丼 - 基本";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 8:
                        // 소고기 덮밥 기본 1개 만들기, 제한 30턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 基本");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "牛丼 - 基本";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 9:
                        // 소고기 덮밥 기본 2개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 基本");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "牛丼 - 基本";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 10:
                        // 소고기 덮밥 기본 2개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 基本");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 기본"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[1];
                        menuNameText.text = "牛丼 - 基本";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 11:
                        // 소고기 덮밥 토핑 세트 1개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - トッピングセット");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - トッピングセット";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 12:
                        // 소고기 덮밥 토핑 세트 1개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - トッピングセット");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - トッピングセット";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 13:
                        // 소고기 덮밥 토핑 세트 1개 만들기, 제한 30턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - トッピングセット");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - トッピングセット";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 14:
                        // 소고기 덮밥 토핑 세트 2개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - トッピングセット");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - トッピングセット";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 15:
                        // 소고기 덮밥 토핑 세트 2개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - トッピングセット");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 토핑 세트"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - トッピングセット";
                        goalNumText.text = "目標数 : " + 2;
                        break;
                    case 16:
                        // 소고기 덮밥 레드와인 1개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 赤ワイン");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - 赤ワイン";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 17:
                        // 소고기 덮밥 레드와인 1개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 赤ワイン");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - 赤ワイン";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 18:
                        // 소고기 덮밥 레드와인 1개 만들기, 제한 30턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 赤ワイン");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 1);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 30;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - 赤ワイン";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 19:
                        // 소고기 덮밥 레드와인 2개 만들기, 제한 100턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 赤ワイン");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 100;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - 赤ワイン";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                    case 20:
                        // 소고기 덮밥 레드와인 2개 만들기, 제한 50턴
                        menuInfo.Add("소고기 덮밥이 든 냄비(완료)", "牛丼 - 赤ワイン");
                        stageInfo.Add("소고기 덮밥이 든 냄비(완료)", 2);
                        currentStageInfo.Add("소고기 덮밥이 든 냄비(완료)", 0);
                        maxTurnNum = 50;
                        turnNumText.text = "残りターン : " + maxTurnNum.ToString();
                        recipeString = recipeExplanationInfo["소고기 덮밥 레드와인"];
                        menuImage.sprite = RecipeBookMngScript.RecipeSprites[2];
                        menuNameText.text = "牛丼 - 赤ワイン";
                        goalNumText.text = "目標数 : " + 1;
                        break;
                }
                break;
        }
        
        CurrentStageInfoTextSet();
    }
    #endregion

    void StartGamePanelInit() {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                break;
            case 1:
                if (TayuBoxMngScript.QuestStageIndex != -1)
                    stageKindText.text = "오늘은 이게 먹고 싶어!";
                else if (MainGameMngScript.IsPractice)
                    stageKindText.text = "요리 연습을 해보자!";
                else
                    stageKindText.text = "메인 스토리 스테이지!";
                maxTurnNumText.text = "주어진 턴 수 : " + maxTurnNum;
                break;
            case 2:
                if (TayuBoxMngScript.QuestStageIndex != -1)
                    stageKindText.text = "今日はこれが食べたい!";
                else if (MainGameMngScript.IsPractice)
                    stageKindText.text = "料理の練習をやってみよう!";
                else
                    stageKindText.text = "メインストーリーステージ!";
                maxTurnNumText.text = "もらったターン数 : " + maxTurnNum;
                break;
        }
    }

    bool TurnStart() {
        CardMngScript.IncreaseCardPutCount();
        turnNum.Value++;
        if (turnNum.Value > maxTurnNum)
            return false;
        else
            return true;
    }

    bool IsStageClear() {
        foreach (var menu in stageInfo.Keys) {
            if (stageInfo[menu] > currentStageInfo[menu])
                return false;
        }
        return true;
    }

    IEnumerator StartGameCo() {
        GameSetup(stageNum);
        yield return new WaitForSeconds(Utils.sceneCurtainDotweenTime);
        startGamePanel.Show(2);
        yield return new WaitForSeconds(Utils.turnStartPanelUpDownDotweenTime);
        SoundMngScript.PlaySFX(5);
        yield return new WaitForSeconds(Utils.turnStartPanelUpDownDotweenTime + 2);
        StartCoroutine(FirstDrowCo());
    }

    IEnumerator FirstDrowCo() {
        isLoading = true;
        for (int i = 0; i < startCardCount; i++) {
            CardMngScript.AddCardItem();
            yield return new WaitForSeconds(Utils.cardDrawDotweenTime + Utils.cardDrawExtraTime);
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {
        isLoading = true;

        myTurn = true;
        if (TurnStart()) {
            turnStartPanel.Show();
            yield return new WaitForSeconds(Utils.turnStartPanelUpDownDotweenTime * 2 + Utils.turnStartPanelAppendDotweenTIme);
            CardMngScript.AddCardItem();
            yield return new WaitForSeconds(Utils.cardDrawDotweenTime);
        }
        else
            StartCoroutine(GameOverCo());

        isLoading = false;
    }

    IEnumerator EndTurnCo() {
        if (CardMngScript.CardDragging || CardMngScript.CardState != ECardState.CanMouseDrag)
            yield break;
        isLoading = true;
        myTurn = false;
        isCoroutine.Add(true);
        isCoroutine.Add(true);
        StartCoroutine(CardMngScript.ExecuteCardsCo());
        while (isCoroutine[0]) yield return null;
        StartCoroutine(GridObjectMngScript.ExecuteGridObjectCo());
        while (isCoroutine[1]) yield return null;
        if (IsStageClear())
            StartCoroutine(StageClearCo());
        else
            StartCoroutine(StartTurnCo());
        isCoroutine.Clear();
    }

    IEnumerator GameOverCo() {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                break;
            case 1:
                turnStartPanel.Show("게임 오버!");
                break;
            case 2:
                turnStartPanel.Show("ゲームオーバー!");
                break;
        }
        SoundMngScript.PlaySFX(7);
        yield return new WaitForSeconds(Utils.turnStartPanelUpDownDotweenTime * 2 + Utils.turnStartPanelAppendDotweenTIme);
        MainGameMngScript.MainSceneCanvas.SetActive(true);
        SceneManager.LoadScene("MainScene");
    }

    IEnumerator StageClearCo() {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                break;
            case 1:
                turnStartPanel.Show("스테이지 클리어!");
                break;
            case 2:
                turnStartPanel.Show("ステージクリア!");
                break;
        }
        SoundMngScript.PlaySFX(6);
        yield return new WaitForSeconds(Utils.turnStartPanelUpDownDotweenTime * 2 + Utils.turnStartPanelAppendDotweenTIme);
        if (TayuBoxMngScript.QuestStageIndex != -1)
            TayuBoxMngScript.QuestClear();
        else if (!StageMngScript.StageButtonList[stageNum].IsCleared && !MainGameMngScript.IsPractice)
            StageMngScript.StageClear(stageNum);
        MainGameMngScript.IsPractice = false;
        if (MainGameMngScript.BackToScript)
            Utils.LoadScene("NaninovelScene");
        else
            Utils.LoadScene("MainScene");
    }
}
