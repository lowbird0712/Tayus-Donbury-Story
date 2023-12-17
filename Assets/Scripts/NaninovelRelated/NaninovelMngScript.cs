using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Naninovel;

public class NaninovelMngScript : MonoBehaviour {
    static public NaninovelMngScript    Inst { get; set; } = null;

    bool                                initialized = false;

    private void Awake() => Inst = this;

    private void Start() {
        if (Engine.Initialized)
            LoadScript();
        else
            Engine.OnInitializationFinished += LoadScript;
    }

    async void LoadScript() {
        if (initialized)
            return;
        await Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync(GetNaninovelScriptName(MainGameMngScript.StageNum));
        //await Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync("Script 0 JP");
        initialized = true;
    }

    string GetNaninovelScriptName(int _stageNum) {
        switch (LocalizeManager.Inst.currentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                return null; 
            case 1:
                return "Script " + _stageNum.ToString();
            case 2:
                return "Script " + _stageNum.ToString() + " JP";
            default:
                Debug.Log("잘못된 언어 인덱스입니다!!!!");
                return null;
        }
    }
}
