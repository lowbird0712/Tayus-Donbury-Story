using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Naninovel;

public class NaninovelMngScript : MonoBehaviour {
    static public NaninovelMngScript    Inst { get; set; } = null;

    bool                                initialized = false;

    private void Awake() => Inst = this;

    private void Start()
    {
        if (string.IsNullOrEmpty(MainGameMngScript.ScriptLabelNameTemp))
            Engine.OnInitializationFinished += LoadScript;
        else
            Engine.OnInitializationFinished += LoadScriptFromLabel;
    }

    async void LoadScript() {
        if (initialized)
            return;
        switch (MainGameMngScript.StageNum)
        {
            case 1:
                MainGameMngScript.BackToScript = true;
                MainGameMngScript.ScriptLabelNameTemp = "EndCardGame";
                break;
            default :
                MainGameMngScript.BackToScript = false;
                MainGameMngScript.ScriptLabelNameTemp = "";
                break;
        }
        await Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync(GetNaninovelScriptName(MainGameMngScript.StageNum));
        initialized = true;
    }

    async void LoadScriptFromLabel()
    {
        if (initialized)
            return;
        await Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync(Inst.GetNaninovelScriptName(MainGameMngScript.StageNum), label : MainGameMngScript.ScriptLabelNameTemp);
        Inst.initialized = true;
    }

    string GetNaninovelScriptName(int _stageNum) {
        switch (LocalizeManager.Inst.currentLanguageIndex) {
            case 0:
                Debug.LogError("English is not supported!!!!");
                return null; 
            case 1:
                return "Script " + _stageNum.ToString();
            case 2:
                return "Script " + _stageNum.ToString() + " JP";
            default:
                Debug.Log("Incorrect Naninovel script name!!!!");
                return null;
        }
    }
}
