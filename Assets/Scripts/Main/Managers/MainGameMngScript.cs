using UnityEngine;
using TMPro;
using UniRx;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript         Inst { get; set; } = null;

    [SerializeField] GameObject             mainSceneCanvas;
    [SerializeField] GameObject             eventSystem;
    [SerializeField] PanelScript            messagePanel;
    [SerializeField] TextMeshProUGUI        dotoriNumText; 
    [SerializeField] GameObject             calander;
    [SerializeField] GameObject             travelNote;
    [SerializeField] GameObject             recipeBook;
    [SerializeField] GameObject             tayuBox;
    [SerializeField] GameObject             inventory;
    [SerializeField] GameObject             campingShop;
    [SerializeField] GameObject             calanderButton;
    [SerializeField] GameObject             travelNoteButton;
    [SerializeField] GameObject             recipeBookButton;
    [SerializeField] GameObject             tayuBoxButton;
    [SerializeField] GameObject             inventoryButton;
    [SerializeField] GameObject             campingShopButton;

    bool isUIActive;
    bool                                    isPractice = false;
    ReactiveProperty<int>                   dotoriNum = new ReactiveProperty<int>();
    int                                     stageNum = -1;
    string                                  scriptLabelNameTemp;
    bool                                    backToScript = false;

    static public GameObject                MainSceneCanvas => Inst.mainSceneCanvas;
    static public GameObject                EventSystem => Inst.eventSystem;
    static public PanelScript               MessagePanel => Inst.messagePanel;
    static public ReactiveProperty<int>     DotoriNum => Inst.dotoriNum;
    static public bool                      IsPractice { get => Inst.isPractice; set { Inst.isPractice = value; } }
    static public int                       StageNum { get => Inst.stageNum; set { Inst.stageNum = value; } }
    static public string                    ScriptLabelNameTemp { get => Inst.scriptLabelNameTemp; set => Inst.scriptLabelNameTemp = value; }
    static public bool                      BackToScript { get => Inst.backToScript; set => Inst.backToScript = value; }

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
        Inst.inventory.SetActive(false);
        Inst.campingShop.SetActive(false);
    }

    public void CalanderButton() {
        if (!isUIActive) {
            isUIActive = true;
            calanderButton.transform.localPosition += Vector3.left * 50;
            calander.SetActive(true);
        }
        else if (calander.activeSelf) {
            isUIActive = false;
            calanderButton.transform.localPosition -= Vector3.left * 50;
            calander.SetActive(false);
        }
    }

    public void TravelNoteButton() {
        if (!isUIActive) {
            isUIActive = true;
            travelNoteButton.transform.localPosition += Vector3.left * 50;
            travelNote.SetActive(true);
        }
        else if (travelNote.activeSelf) {
            isUIActive = false;
            travelNoteButton.transform.localPosition -= Vector3.left * 50;
            travelNote.SetActive(false);
        }
    }

    public void RecipeBookButton() {
        if (!isUIActive) {
            isUIActive = true;
            recipeBookButton.transform.localPosition += Vector3.left * 50;
            recipeBook.SetActive(true);
        }
        else if (recipeBook.activeSelf) {
            isUIActive = false;
            recipeBookButton.transform.localPosition -= Vector3.left * 50;
            recipeBook.SetActive(false);
        }
    }

    public void TayuBoxButton() {
        if (!isUIActive) {
            isUIActive = true;
            tayuBoxButton.transform.localPosition += Vector3.left * 50;
            tayuBox.SetActive(true);
        }
        else if (tayuBox.activeSelf) {
            isUIActive = false;
            tayuBoxButton.transform.localPosition -= Vector3.left * 50;
            tayuBox.SetActive(false);
        }
    }

    public void InventoryButton() {
        if (!isUIActive) {
            isUIActive = true;
            LinBoxMngScript.Init();
            inventoryButton.transform.localPosition += Vector3.left * 50;
            inventory.SetActive(true);
        }
        else if (inventory.activeSelf) {
            isUIActive = false;
            inventoryButton.transform.localPosition -= Vector3.left * 50;
            inventory.SetActive(false);
        }
    }

    public void CampingShopButton() {
        if (!isUIActive) {
            isUIActive = true;
            campingShopButton.transform.localPosition += Vector3.left * 50;
            campingShop.SetActive(true);
        }
        else if (campingShop.activeSelf) {
            isUIActive = false;
            campingShopButton.transform.localPosition -= Vector3.left * 50;
            campingShop.SetActive(false);
        }
    }
}
