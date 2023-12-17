using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampMngScript : MonoBehaviour {
    static public CampMngScript     Inst { get; set; } = null;

    [SerializeField] Image          backGround;
    [SerializeField] Sprite         defaultBackGroundSprite;

    string                          backGroundName;
    string                          BGM_Name;
    string                          BGM_defaultName;

    static public string            BackGroundName => Inst.backGroundName;
    static public string            BGMName => Inst.BGM_Name;

    private void Awake() => Inst = this;

    private void Start() => BGM_defaultName = "Default BGM";

    static public bool UseItem(Item _item) {
        if (_item.name == Inst.backGroundName) {
            Inst.backGround.sprite = Inst.defaultBackGroundSprite;
            Inst.backGroundName = "Default Background";
            return false;
        }
        else if (_item.name == Inst.BGM_Name) {
            Inst.BGM_Name = Inst.BGM_defaultName;
            return false;
        }
        else if (_item.background) {
            Inst.backGround.sprite = _item.background;
            Inst.backGroundName = _item.name;
            return true;
        }
        else if (_item.bgmName != null) {
            Inst.BGM_Name = _item.bgmName;
            return true;
        }
        return false;
    }
}
