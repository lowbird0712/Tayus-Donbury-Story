using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Naninovel;

public class Utils : MonoBehaviour {
    static public Utils Inst { get; set; } = null;

    ///////////////////// 비주얼노벨 적용 변수 ///////////////////////////////////////////////////////////////////////////////
    static public float             storyBlockFlipTime = 0.3f;
    static public float             storyBlockFadeTime = 0.5f;

    ///////////////////// 카드게임 적용 변수 //////////////////////////////////////////////////////////////////////////////////
    static public Vector3           cardScale = new Vector3(1.0f, 1.0f, 1.0f);
    static public float             cardDragFloat = 1.2f;
    static public float             cardPutUpFloat = 2.0f;
    static public float             cardPutUpDownDotweenTime = 0.3f;
    static public float             cardSwapDotweenTime = 0.1f;
    static public float             cardPutCoverDotweenTime = 0.1f;
    static public float             cardAlignmentDotweenTime = 0.1f;

    ///////////////////// 패스트모드 적용 변수 ///////////////////////////////////////////////////////////////////////////////
    static public float             fastModeFloat = 0.1f;
    static public float             cardDrawDotweenTime = 0.5f;
    static public float             cardDrawExtraTime = 0.1f;
    static public float             turnStartPanelUpDownDotweenTime = 0.3f;
    static public float             turnStartPanelAppendDotweenTIme = 0.9f;
    static public float             cardExecDotweenTime = 0.3f;
    static public float             sceneCurtainDotweenTime = 2f;

    [SerializeField] Image          sceneCurtain = null;
    static public bool              sceneLoadingCompleted = false;

    private void Awake() => Inst = this;

    static public void LoadScene(string _sceneName) => Inst.StartCoroutine(LoadSceneCo(_sceneName));

    static public Vector3 MousePos {
        get {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z += 5;
            return pos;
        }
    }

    static public void SceneLoadingCompleted(Scene _scene, LoadSceneMode _mode) {
        sceneLoadingCompleted = true;
        SceneManager.sceneLoaded -= SceneLoadingCompleted;
    }

    static void PastSceneClean() {
        string pastSceneName = SceneManager.GetActiveScene().name;
        if (pastSceneName == "MainScene")
            MainGameMngScript.MainSceneClose();
        else if (pastSceneName == "CardGameScene")
            ;
        else if (pastSceneName == "NaninovelScene") {
            Engine.GetService<IStateManager>().ResetStateAsync();
            Engine.GetService<ICameraManager>().Camera.enabled = false;
        }
    }

    static public IEnumerator LoadSceneCo(string _sceneName) {
        Inst.SetSceneCurtain(_sceneName);
        Inst.sceneCurtain.enabled = true;
        Inst.sceneCurtain.DOFade(1, sceneCurtainDotweenTime);
        yield return new WaitForSeconds(sceneCurtainDotweenTime);
        PastSceneClean();
        SceneManager.LoadScene(_sceneName);
        SceneManager.sceneLoaded += SceneLoadingCompleted;
        while (!sceneLoadingCompleted)
            yield return null;
        if (_sceneName == "MainScene")
            MainGameMngScript.MainSceneCanvas.SetActive(true);
        sceneLoadingCompleted = false;
        Inst.sceneCurtain.DOFade(0, sceneCurtainDotweenTime);
        yield return new WaitForSeconds(sceneCurtainDotweenTime);
        Inst.sceneCurtain.enabled = false;
    }

    void SetSceneCurtain(string _sceneName)
    {
        if (_sceneName == "CardGameScene")
            SceneCurtainMngScript.SetSceneCurtain(CurtainIndex.CardGame);
        else
            SceneCurtainMngScript.SetSceneCurtain(CurtainIndex.Normal);
    }
}
