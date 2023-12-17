using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXButtonScript : MonoBehaviour {
    public void Clicked(int _SFXindex) => SoundMngScript.PlaySFX(_SFXindex);
}
