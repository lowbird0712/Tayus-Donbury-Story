using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingMngScript : MonoBehaviour {
    static public SettingMngScript  Inst { get; set; } = null;

    [SerializeField] GameObject     setting;
    [SerializeField] Slider         sliderBGM;
    [SerializeField] Slider         sliderSFX;

    private void Awake() => Inst = this;

    static public void GotoMainButton()
    {
        MainGameMngScript.BackToScript = false;
        MainGameMngScript.ScriptLabelNameTemp = "";
        Utils.LoadScene("MainScene");
    }

    public void RestartButton() => Utils.LoadScene("CardGameScene");

    static public void SettingButton() => Inst.setting.SetActive(!Inst.setting.activeSelf);
}
