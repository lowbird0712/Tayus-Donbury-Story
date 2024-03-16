using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHidingButtonScript : SFXButtonScript {
    [SerializeField] GameObject hidingUIs;
    GameObject                  settingButton;

    public void UIHidingButtonClicked() {
        if (!settingButton)
            settingButton = GameObject.Find("Setting Button");
        MainGameMngScript.CloseEveryUIs();
        hidingUIs.SetActive(!hidingUIs.activeSelf);
        settingButton.SetActive(!settingButton.activeSelf);
    }
}
