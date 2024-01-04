using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingMngScript : MonoBehaviour {
    static public SettingMngScript  Inst { get; set; } = null;

    [SerializeField] GameObject     setting;
    [SerializeField] GameObject     nonUIBlocker;
    [SerializeField] Slider         sliderBGM;
    [SerializeField] Slider         sliderSFX;

    private void Awake() => Inst = this;

    static public void GotoMainButton() => Utils.LoadScene("MainScene");

    static public void SettingButton() {
        Inst.nonUIBlocker.SetActive(!Inst.nonUIBlocker.activeSelf);
        Inst.setting.SetActive(!Inst.setting.activeSelf);
    }
}
