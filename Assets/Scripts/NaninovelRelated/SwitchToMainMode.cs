using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Naninovel;

[CommandAlias("main")]
public class SwitchToMainMode : Command
{
    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        MainGameMngScript.BackToScript = false;
        MainGameMngScript.ScriptLabelNameTemp = "";
        Engine.GetService<IInputManager>().ProcessInput = false;
        Engine.GetService<IScriptPlayer>().Stop();
        //Engine.GetService<ICameraManager>().Camera.enabled = false;
        //await Engine.GetService<IStateManager>().ResetStateAsync();
        Utils.LoadScene("MainScene");
    }
}