using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurtainIndex
{
    Normal,
    CardGame,
}

public class SceneCurtainMngScript : MonoBehaviour
{
    static public SceneCurtainMngScript Inst { get; set; } = null;

    [Header("씬 커튼 이미지")]
    [SerializeField] Image              sceneCurtain;
    [Header("씬 전환 시 사용할 스프라이트 리스트")]
    [SerializeField] List<Sprite>       curtainList = new List<Sprite>();

    private void Awake() => Inst = this;

    static public void SetSceneCurtain(CurtainIndex _index)
        => Inst.sceneCurtain.sprite = Inst.curtainList[(int)_index];
}
