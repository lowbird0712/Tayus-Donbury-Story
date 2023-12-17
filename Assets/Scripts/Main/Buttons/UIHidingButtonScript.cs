using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHidingButtonScript : SFXButtonScript {
    [SerializeField] GameObject hidingUIs;
    public void UIHidingButtonClicked() {
        MainGameMngScript.CloseEveryUIs();
        hidingUIs.SetActive(!hidingUIs.activeSelf);
    }
}
