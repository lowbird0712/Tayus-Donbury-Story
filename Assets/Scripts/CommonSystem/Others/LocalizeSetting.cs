using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LocalizeManager;

public class LocalizeSetting : MonoBehaviour
{
    TMP_Dropdown dropdown;

    void LocalizeSettingChanged() => dropdown.value = Inst.currentLanguageIndex;

    private void Start() {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown.options.Count != Inst.languageDatas.Count)
            SetLanguageOption();
        dropdown.onValueChanged.AddListener(_ => SetLanguageIndex(dropdown.value));
        LocalizeSettingChanged();
        Inst.LocalizeSettingChanged += LocalizeSettingChanged;
    }

    private void OnDestroy() => Inst.LocalizeSettingChanged -= LocalizeSettingChanged;

    void SetLanguageOption() {
        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < Inst.languageDatas.Count; i++)
            optionDatas.Add(new TMP_Dropdown.OptionData() { text = Inst.languageDatas[i].languageLocalized });
        dropdown.options = optionDatas;
    }
}
