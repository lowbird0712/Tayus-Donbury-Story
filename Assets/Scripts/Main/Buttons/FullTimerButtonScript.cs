using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullTimerButtonScript : SFXButtonScript {
    public void FullTimerButtonClicked() => MainGameMngScript.DotoriNum.Value++;
}
