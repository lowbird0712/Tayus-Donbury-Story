using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class LanguageData {
    public string           language;
    public string           languageLocalized;
    public List<string>     value = new List<string>();
}

public class LocalizeManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI testText1;
    [SerializeField] public TextMeshProUGUI testText2;

    static public LocalizeManager           Inst { get; set; } = null;

    [Header("Manager Classes")]
    [SerializeField] CampingShopMngScript   campingShop;
    [SerializeField] LinBoxMngScript        inventory;
    [SerializeField] RecipeBookMngScript    recipeBook;
    [SerializeField] TravelNoteMngScript    travelNote;
    [SerializeField] TayuBoxMngScript       tayuBox;

    public List<LanguageData>               languageDatas;
    public event Action                     LocalizeSettingChanged = () => { };
    public event Action                     LocalizeChanged = () => { };
    public int                              currentLanguageIndex;
                                            
    const string                            localizingSheetURL = "https://docs.google.com/spreadsheets/d/1_IQNByrN9AhpBHUAjUkZIPRgCuRXLPbk0meOJxj_PGo/export?format=tsv";

    static public int                       CurrentLanguageIndex => Inst.currentLanguageIndex;

    private void Awake() {
        Inst = this;
        InitLanguage();
    }

    private void Start() => LocalizeRefresh();

    [ContextMenu("Pull Localizing Sheet")]
    void GetLocalizingSheet() => StartCoroutine(GetLocalizingSheetCo());

    static public void SetLanguageIndex(int _index) {
        Inst.currentLanguageIndex = _index;
        PlayerPrefs.SetInt("LanguageIndex", Inst.currentLanguageIndex);
        Inst.LocalizeSettingChanged();
        Inst.LocalizeChanged();
        Inst.LocalizeRefresh();
    }

    void LocalizeRefresh() {
        campingShop.LocalizeRefresh();
        inventory.LocalizeRefresh();
        recipeBook.LocalizeRefresh();
        travelNote.LocalizeRefresh();
        tayuBox.LocalizeRefresh();
    }

    void SetLocalizingList(string _tsv) {
        string[]    row = _tsv.Split('\n');
        string[]    column;
        int         rowLength = row.Length;
        int         columnLength = row[0].Split('\t').Length;
        string[,]   langTexts = new string[rowLength, columnLength];
        for (int i = 0; i < rowLength; i++) {
            column = row[i].Split('\t');
            for (int j = 0; j < columnLength; j++)
                langTexts[i, j] = column[j];
        }
        languageDatas = new List<LanguageData>();
        for (int i = 0; i < columnLength; i++) {
            LanguageData languageData = new LanguageData();
            languageData.language = langTexts[0, i];
            languageData.languageLocalized = langTexts[1, i];
            for (int j = 2; j < rowLength; j++)
                languageData.value.Add(langTexts[j, i]);
            languageDatas.Add(languageData);
        }
    }

    void InitLanguage() {
        //PlayerPrefs.DeleteAll();
        int languageIndex = PlayerPrefs.GetInt("LanguageIndex", -1);
        int systemIndex = languageDatas.FindIndex(x => x.language.ToLower() == Application.systemLanguage.ToString().ToLower());
        if (systemIndex == -1)
            systemIndex = 1;
        Inst.currentLanguageIndex = languageIndex == -1 ? systemIndex : languageIndex;
        PlayerPrefs.SetInt("LanguageIndex", Inst.currentLanguageIndex);
        Inst.LocalizeSettingChanged();
        Inst.LocalizeChanged();
        //SetLanguageIndex(languageIndex == -1 ? systemIndex : languageIndex);
    }

    static public string Localize(string _textKey) {
        int keyIndex = Inst.languageDatas[0].value.FindIndex(x => x.ToLower() == _textKey.ToLower());
        return Inst.languageDatas[Inst.currentLanguageIndex].value[keyIndex].Replace("\r", "");
    }

    IEnumerator GetLocalizingSheetCo() {
        UnityWebRequest www = UnityWebRequest.Get(localizingSheetURL);
        yield return www.SendWebRequest();
        SetLocalizingList(www.downloadHandler.text);
    }
}
