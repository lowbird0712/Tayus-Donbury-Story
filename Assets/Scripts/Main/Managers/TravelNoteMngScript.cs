using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum PageState {
    PS_UNLOCKED,
    PS_LOCKED,
    PS_BLOCKED
}

class Page {
    public string      storyText;
    public PageState   state = PageState.PS_BLOCKED;
}

public class TravelNoteMngScript : MonoBehaviour {
    static public TravelNoteMngScript   Inst { get; set; } = null;

    [SerializeField] Text               localizeTextLocked;
    [SerializeField] Text               localizeTextCleared;
    [SerializeField] Text               localizeTextCantUnlock;
    [SerializeField] Text               localizeTextCanUnlock;
    [SerializeField] Text               localizeTextLastPage;
    [SerializeField] TextMeshProUGUI    leftPageText;
    [SerializeField] TextMeshProUGUI    rightPageText;
    [SerializeField] TextMeshProUGUI    leftUnlockText;
    [SerializeField] TextMeshProUGUI    rightUnlockText;

    List<Page>                          pageList = new List<Page>();
    List<string>                        localizedPageListJP = new List<string>();
    int                                 leftPageIndex;

    static public int GetOpenPageNum() {
        int openPageNum = 0;
        foreach (var page in Inst.pageList) {
            if (page.state != PageState.PS_UNLOCKED)
                break;
            openPageNum++;
        }
        return openPageNum;
    }

    int LeftPageIndex {
        get => leftPageIndex;
        set {
            leftPageIndex = value;
            if (leftPageIndex + 1 == pageList.Count) {
                switch (pageList[leftPageIndex].state) {
                    case PageState.PS_BLOCKED:
                        leftPageText.text = localizeTextCantUnlock.text;
                        leftUnlockText.text = localizeTextLocked.text;
                        break;
                    case PageState.PS_LOCKED:
                        leftPageText.text = localizeTextCanUnlock.text;
                        leftUnlockText.text = "10";
                        break;
                    case PageState.PS_UNLOCKED:
                        switch (LocalizeManager.Inst.currentLanguageIndex) {
                            case 1:
                                leftPageText.text = pageList[leftPageIndex].storyText;
                                break;
                            case 2:
                                leftPageText.text = localizedPageListJP[leftPageIndex];
                                break;
                        }
                        leftUnlockText.text = localizeTextCleared.text;
                        break;
                }
                rightPageText.text = localizeTextLastPage.text;
                rightUnlockText.text = localizeTextLocked.text;
                return;
            }
            if (pageList[leftPageIndex].state == PageState.PS_BLOCKED) {
                leftPageText.text = localizeTextCantUnlock.text;
                rightPageText.text = localizeTextCantUnlock.text;
                leftUnlockText.text = localizeTextLocked.text;
                rightUnlockText.text = localizeTextLocked.text;
            }
            else if (pageList[leftPageIndex].state == PageState.PS_LOCKED) {
                leftPageText.text = localizeTextCanUnlock.text;
                rightPageText.text = localizeTextCantUnlock.text;
                leftUnlockText.text = "10";
                rightUnlockText.text = localizeTextLocked.text;
            }
            else if (pageList[leftPageIndex + 1].state == PageState.PS_LOCKED) {
                switch (LocalizeManager.Inst.currentLanguageIndex) {
                    case 1:
                        leftPageText.text = pageList[leftPageIndex].storyText;
                        break;
                    case 2:
                        leftPageText.text = localizedPageListJP[leftPageIndex];
                        break;
                }
                rightPageText.text = localizeTextCanUnlock.text;
                leftUnlockText.text = localizeTextCleared.text;
                rightUnlockText.text = "10";
            }
            else {
                switch (LocalizeManager.Inst.currentLanguageIndex) {
                    case 1:
                        leftPageText.text = pageList[leftPageIndex].storyText;
                        rightPageText.text = pageList[leftPageIndex + 1].storyText;
                        break;
                    case 2:
                        leftPageText.text = localizedPageListJP[leftPageIndex];
                        rightPageText.text = localizedPageListJP[leftPageIndex + 1];
                        break;
                }
                leftUnlockText.text = localizeTextCleared.text;
                rightUnlockText.text = localizeTextCleared.text;
            }
        }
    }

    private void Awake() => Inst = this;
    private void Start() {
        Init();
        gameObject.SetActive(false);
    }

    void Init() {
        Page page = new Page();
        page.storyText = "앞으로 하루 이틀 정도만 더 가면 우리의 첫 목적지인 \"도토리숲 마을\"에 도착한다." +
                        " 많은 수의 도토리 나무가 자라 이루어진 마을이고 이름처럼 도토리가 많이 열리는 마을이다." +
                        " 마침 우리가 도착할 즈음에는 \"도토리 축제\"가 시작되는 시기이기도 하니 나도 곰씨도 기대가 많이 된다.";
        page.state = PageState.PS_LOCKED;
        pageList.Add(page);
        localizedPageListJP.Add("これから２日ぐらいだけ行けば私達の初目的地の\"どんぐり森の村\"につく."　+
                            "多くのどんぐりの木が育て作られた村で, 名前どうりどんぐりがたくさん実る村である." +
                            "たまだま和たちたちがつくぐらいには\"どんぐり祭り\"が始まる時期でもあるし, 私もくまさんもとても期待している.");
        page = new Page();
        page.storyText = "도토리 축제는 기대한 것 이상으로 재미있었다. 잔잔하게 울리는 북소리와 음악에 맞춰 춤추는 사람들.." +
                        " 조금 더 있고 싶지만 그래도 이제는 다음 목적지로 이동해야 한다." +
                        " 내일 출발할 곳은 \"골드스푼\"이라고 불리는 무역의 대도시이다." +
                        " 전 대륙의 다양한 식재료들이 모이는 곳이어서 볼거리가 많을 것 같다.";
        page.state = PageState.PS_BLOCKED;
        pageList.Add(page);
        localizedPageListJP.Add("どんぐり祭りは期待していたよりも面白かった. 静かに響く太鼓の音と音楽に合わせて踊る人々..." +
                            "もうちょっといたいけれど, もう次の目的地に移動するべきだ." +
                            "明日出発するところは\"ゴールドスプーン\"と呼ばれる貿易の大都市だ." +
                            "全大陸の様々な食材が集まるところで見応えがありそうだ.");
        page = new Page();
        page.storyText = "3 페이지!";
        page.state = PageState.PS_BLOCKED;
        pageList.Add(page);
        localizedPageListJP.Add("3ページ!");

        int openPageNum = SaveLoadMngScript.SaveData.travelNoteOpenPageNum;
        for (int i = 0; i < openPageNum; i++)
            pageList[i].state = PageState.PS_UNLOCKED;
        if (openPageNum < pageList.Count)
            pageList[openPageNum].state = PageState.PS_LOCKED;
        LeftPageIndex = 0;
    }

    public void LocalizeRefresh() => Inst.LeftPageIndex = Inst.LeftPageIndex;

    public void GoToLeft() {
        if (leftPageIndex >= 2)
            LeftPageIndex -= 2;
    }

    public void GoToRight() {
        if (leftPageIndex + 2 < pageList.Count)
            LeftPageIndex += 2;
    }

    public void LeftUnlock() {
        if (pageList[leftPageIndex].state != PageState.PS_LOCKED)
            return;
        if (MainGameMngScript.DotoriNum.Value < 10) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        MainGameMngScript.DotoriNum.Value -= 10;
        if (StageMngScript.MaxUnlockIndex < StageMngScript.NextUnblockIndex) {
            StageMngScript.MaxUnlockIndex += 5;
            StageMngScript.UnBlockNext();
        }
        else
            StageMngScript.MaxUnlockIndex += 5;
        pageList[leftPageIndex].state = PageState.PS_UNLOCKED;
        if (leftPageIndex + 1 < pageList.Count)
            pageList[leftPageIndex + 1].state = PageState.PS_LOCKED;
        LeftPageIndex = leftPageIndex;
    }

    public void RightUnlock() {
        if (leftPageIndex + 1 == pageList.Count || pageList[leftPageIndex + 1].state != PageState.PS_LOCKED)
            return;
        if (MainGameMngScript.DotoriNum.Value < 10) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        MainGameMngScript.DotoriNum.Value -= 10;
        if (StageMngScript.MaxUnlockIndex < StageMngScript.NextUnblockIndex) {
            StageMngScript.MaxUnlockIndex += 5;
            StageMngScript.UnBlockNext();
        }
        else
            StageMngScript.MaxUnlockIndex += 5;
        pageList[leftPageIndex + 1].state = PageState.PS_UNLOCKED;
        if (leftPageIndex + 2 < pageList.Count)
            pageList[leftPageIndex + 2].state = PageState.PS_LOCKED;
        LeftPageIndex = leftPageIndex;
    }
}
