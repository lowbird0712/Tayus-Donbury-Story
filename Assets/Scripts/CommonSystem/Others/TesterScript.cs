using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TestInfo {
    public bool    Using;
    public string  mapName;
    public int     storySelectingIndex;
    public int     visitedNum;
    public int     givenGold;
}

public class TesterScript : MonoBehaviour {
    static public TesterScript Inst { get; set; } = null;

    [SerializeField]
    bool    mUsing = false;
    [SerializeField]
    string  mMapName = "";
    [SerializeField]
    int     mStorySelectingIndex = -1;
    [SerializeField]
    int     mVisitedNum = -1;
    [SerializeField]
    int     mGivenGold = 0;

    static public TestInfo GetInfo() {
        TestInfo info;
        info.Using = Inst.mUsing;
        info.mapName = Inst.mMapName;
        info.storySelectingIndex = Inst.mStorySelectingIndex;
        info.visitedNum = Inst.mVisitedNum;
        info.givenGold = Inst.mGivenGold;

        return info;
    }

    static public void SetVisitedNum() {
        // 다른 맵으로 넘어가는 부분을 테스트 해야 할 때 해당 맵의 방문 횟수를 여기에서 수동으로 설정한다
        //GameMngScript.VisitedMapName.Add("Minimori1stRoad", 3);
        //GameMngScript.VisitedMapName.Add("KokoroRestaurant", 4);
        //GameMngScript.VisitedMapName.Add("KokoroKitchen", 3);
    }

    private void Awake() {
        if (!Inst)
            Inst = this;
    }
}
