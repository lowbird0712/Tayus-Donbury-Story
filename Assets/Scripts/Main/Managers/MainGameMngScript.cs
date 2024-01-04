using UnityEngine;
using TMPro;
using UniRx;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript         Inst { get; set; } = null;

    [SerializeField] GameObject             mainSceneCanvas;
    [SerializeField] PanelScript            messagePanel;
    [SerializeField] TextMeshProUGUI        dotoriNumText; 
    [SerializeField] GameObject             calander;
    [SerializeField] GameObject             travelNote;
    [SerializeField] GameObject             recipeBook;
    [SerializeField] GameObject             tayuBox;
    [SerializeField] GameObject             linBox;
    [SerializeField] GameObject             campingShop;

    bool                                    isUIActive;
    bool                                    isPractice = false;
    ReactiveProperty<int>                   dotoriNum = new ReactiveProperty<int>();
    int                                     stageNum = -1;

    static public GameObject                MainSceneCanvas => Inst.mainSceneCanvas;
    static public PanelScript               MessagePanel => Inst.messagePanel;
    static public ReactiveProperty<int>     DotoriNum => Inst.dotoriNum;
    static public bool                      IsPractice { get => Inst.isPractice; set { Inst.isPractice = value; } }
    static public int                       StageNum { get => Inst.stageNum; set { Inst.stageNum = value; } }

    private void Awake()
    {
        Inst = this;
        Application.targetFrameRate = 60;
    }
    private void Start() {
        dotoriNum.Subscribe(_dotoriNum => dotoriNumText.text = _dotoriNum.ToString());
        dotoriNum.Value = SaveLoadMngScript.SaveData.dotoriNum;
    }

    static public void SendStageNum() {
        CardGameMngScript.StageNum = Inst.stageNum;
        MainSceneClose();
    }

    static public void MainSceneClose() {
        CloseEveryUIs();
        MainSceneCanvas.SetActive(false);
    }

    static public void CloseEveryUIs() {
        Inst.isUIActive = false;
        Inst.calander.SetActive(false);
        Inst.travelNote.SetActive(false);
        Inst.recipeBook.SetActive(false);
        Inst.tayuBox.SetActive(false);
        Inst.linBox.SetActive(false);
        Inst.campingShop.SetActive(false);
    }

    public void CalanderButton() {
        if (!isUIActive) {
            isUIActive = true;
            calander.SetActive(true);
        }
        else if (calander.activeSelf) {
            isUIActive = false;
            calander.SetActive(false);
        }
    }

    public void TravelNoteButton() {
        if (!isUIActive) {
            isUIActive = true;
            travelNote.SetActive(true);
        }
        else if (travelNote.activeSelf) {
            isUIActive = false;
            travelNote.SetActive(false);
        }
    }

    public void RecipeBookButton() {
        if (!isUIActive) {
            isUIActive = true;
            recipeBook.SetActive(true);
        }
        else if (recipeBook.activeSelf) {
            isUIActive = false;
            recipeBook.SetActive(false);
        }
    }

    public void TayuBoxButton() {
        if (!isUIActive) {
            isUIActive = true;
            tayuBox.SetActive(true);
        }
        else if (tayuBox.activeSelf) {
            isUIActive = false;
            tayuBox.SetActive(false);
        }
    }

    public void LinBoxButton() {
        if (!isUIActive) {
            isUIActive = true;
            LinBoxMngScript.Init();
            linBox.SetActive(true);
        }
        else if (linBox.activeSelf) {
            isUIActive = false;
            linBox.SetActive(false);
        }
    }

    public void CampingShopButton() {
        if (!isUIActive) {
            isUIActive = true;
            campingShop.SetActive(true);
        }
        else if (campingShop.activeSelf) {
            isUIActive = false;
            campingShop.SetActive(false);
        }
    }
}
