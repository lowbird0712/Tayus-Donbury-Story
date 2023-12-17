using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMngScript : MonoBehaviour {
    static public SoundMngScript    Inst { get; set; } = null;

    [SerializeField] AudioSource    BGMsource = null;
    [SerializeField] AudioSource    CookBGMsource = null;
    [SerializeField] AudioSource    SFXsource = null;
    [SerializeField] AudioClip[]    BGMs = null;
    [SerializeField] AudioClip[]    CookBGMs = null;
    [SerializeField] AudioClip[]    SFXs = null;

    private void Awake() => Inst = this;

    static public void PlayBGM(int _index = -1) {
        Inst.BGMsource.Stop();
        Inst.BGMsource.clip = Inst.BGMs[_index];
        Inst.BGMsource.time = 0.0f;
        Inst.BGMsource.Play();
    }
    static public void PlayCookBGM(int _index = -1) {
        Inst.CookBGMsource.Stop();
        Inst.CookBGMsource.clip = Inst.CookBGMs[_index];
        Inst.CookBGMsource.time = 0.0f;
        Inst.CookBGMsource.Play();
    }
    static public void PlaySFX(int _index = -1) => Inst.SFXsource.PlayOneShot(Inst.SFXs[_index]);
    static public void SetVolumnBGM(float _valuefromSlider) {
        Inst.BGMsource.volume = 1 - _valuefromSlider;
        if (Inst.CookBGMsource)
            Inst.CookBGMsource.volume = 1 - _valuefromSlider;
    }
    static public void SetVolumnSFX(float _valuefromSlider) => Inst.SFXsource.volume = 1 - _valuefromSlider;
    static public void StopBGM() {
        Inst.BGMsource.Stop();
        Inst.CookBGMsource.Stop();
    }
    static public void StopCookBGM() => Inst.CookBGMsource.Stop();
}
