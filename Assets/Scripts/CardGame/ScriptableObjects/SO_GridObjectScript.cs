using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SO_Timing {
    Start = 0,
    Effect = 1,
    End = 2
}

public class CurrentObjectItem {
    public string       objectName;
    public List<string> currentSpiceNames = new List<string>();
    public List<int>    currentObjectNums = new List<int>();
    public List<string> currentSpellNames = new List<string>();

    public CurrentObjectItem(string _objectName) {
        ObjectItem objectItem = GridObjectMngScript.GridObjectSO.GetObjectItem(_objectName);
        objectName = _objectName;
        if (objectItem.tool != null) {
            for (int i = 0; i < objectItem.tool.neededObjectNames.Count; i++)
                currentObjectNums.Add(0);
        }
        CurrentSpellNameSet();
    }

    void CurrentSpellNameSet() {
        // 처음부터 쓸 수 있는 스펠을 세팅
    }

    public bool CurrentSpellNameUpdate() {
        ObjectItem          objectItem = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName);
        ObjectSubItemTool   tool = objectItem.tool;
        //bool                neededObjectFlag = false;

        if (tool.neededSpiceNames != null && tool.neededSpiceNames.Count != currentSpiceNames.Count)
            return false;
        if (tool.neededObjectNames != null)
        {
            for (int i = 0; i < tool.neededObjectNames.Count; i++)
            {
                if (tool.neededObjectNums[i] != currentObjectNums[i])
                    return false;
            }
        }
        foreach (var spellName in objectItem.usableSpellNames)
            currentSpellNames.Add(spellName);
        return true;

        //foreach (var spellName in objectItem.usableSpellNames) {
        //    if (tool.neededSpiceNames != null && tool.neededSpiceNames.Count != currentSpiceNames.Count)
        //        continue;
        //    if (tool.neededObjectNames != null) {
        //        for (int i = 0; i < tool.neededObjectNames.Count; i++) {
        //            if (tool.neededObjectNums[i] != currentObjectNums[i]) {
        //                neededObjectFlag = true;
        //                break;
        //            }
        //        }
        //        if (neededObjectFlag) {
        //            neededObjectFlag = false;
        //            continue;
        //        }
        //    }
        //    currentSpellNames.Add(spellName);
        //}
    }
}

public class ObjectItem {
    public ObjectSubItemTool    tool;
    public ObjectSubItemCooking cooking;
    public string               explain;
    public bool                 isSpice;
    public bool                 isDotori;

    public List<string>         usableSpellNames;
    public Sprite               sprite;
    public string               animationKey;
    public float                xPivot;
}

public class ObjectSubItemTool {
    public string       nextObjectName;
    public List<string> neededSpiceNames;
    public List<string> neededObjectNames;
    public List<int>    neededObjectNums;
    public bool         useHeatPlate;

    public bool IsNeeded(string _name, bool _isSpice, GridObjectScript _gridObject) {
        if (_isSpice) {
            if (_name == "조미료")
                return true;
            if (neededSpiceNames == null || !neededSpiceNames.Contains(_name))
                return false;
            if (_gridObject.CurrentObjectItem.currentSpiceNames.Contains(_name))
                return false;
        }
        else {
            if (neededObjectNames == null || !neededObjectNames.Contains(_name))
                return false;
            int index = neededObjectNames.IndexOf(_name);
            if (neededObjectNums[index] == _gridObject.CurrentObjectItem.currentObjectNums[index])
                return false;
        }

        return true;
    }
}

public class ObjectSubItemCooking {
    public string               nextObjectName;
    public int                  originCountDown = -1;
    public int                  BGMindex = -1;
}

[CreateAssetMenu(fileName = "SO_GridObject", menuName = "Scriptable Object/SO_GridObject")]
public class SO_GridObjectScript : ScriptableObject {
    [SerializeField] Sprite[]   objectSprites = null;

    string[]                    objectNames;
    ObjectItem[]                objectItems;
    Dictionary<string, string>  localizedStringsJP = new Dictionary<string, string>();

    public string GetLocalizedString(string _koreanKey) {
        if (string.IsNullOrEmpty(_koreanKey))
            return null;
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되지 않고 있습니다!!!!");
                return null;
            case 1:
                return _koreanKey;
            case 2:
                return localizedStringsJP[_koreanKey];
            default:
                Debug.LogError("해당 언어는 아직 지원되지 않고 있습니다!!!!");
                return null;
        }
    }

    ObjectItem SetUpItem(string _objectName) {
        ObjectItem item = new ObjectItem();
        //// ObjectItem
        //item = new ObjectItem();
        //item.explain = "";
        //item.isSpice = false;
        //item.usableSpellNames = new string[1];
        //item.sprite = objectSprites[0];
        //item.animationKey = "";
        //// ObjectSubItemTool
        //item.tool = new ObjectSubItemTool();
        //item.nextObjectName = "";
        //item.neededObjectNames = new string[1];
        //item.insideObjectNames = new string[1];
        //item.neenedObjectNums = new int[1];
        //item.insideObjectNums = new int[1];
        //// ObjectSubItemCooking
        //item.cooking = new ObjectSubItemCooking();
        //item.nextObjectName = "";
        //item.originCountDown = 0;

        string deckName = CardMngScript.DeckName;

        // 기본
        if (_objectName == "밥솥(준비)") {
            item.usableSpellNames = new List<string>(1) { "불지피기" };
            item.sprite = objectSprites[0];
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "밥솥(조리중)";
            item.tool.neededObjectNames = new List<string>(1) { "쌀" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
            item.tool.useHeatPlate = true;
            if (CardGameMngScript.StageNum >= 3)
                item.tool.neededSpiceNames = new List<string>(1) { "도토리주" };
        }
        else if (_objectName == "밥솥(조리중)") {
            item.explain = "다음 오브젝트 : 밥솥(완료)";
            item.animationKey = "Rice Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.nextObjectName = "밥솥(완료)";
            item.cooking.originCountDown = 3;
            item.cooking.BGMindex = 0;
        }
        else if (_objectName == "밥솥(완료)")
            item.sprite = objectSprites[0];
        else if (_objectName == "쌀")
            item.sprite = objectSprites[1];
        else if (_objectName == "조미료")
            item.sprite = objectSprites[11];
        else if (_objectName == "도토리주") {
            item.isSpice = true;
            item.sprite = objectSprites[12];
        }

        // 기본 도토리
        else if (_objectName == "도토리 솥(준비)") {
            item.usableSpellNames = new List<string>(1) { "도토리 굽기" };
            item.sprite = objectSprites[2];
            item.isDotori = true;
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "도토리 솥(조리중)";
            item.tool.neededObjectNames = new List<string>(1) { "도토리" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
            item.tool.useHeatPlate = true;
        }
        else if (_objectName == "도토리 솥(조리중)") {
            item.explain = "조리가 완료될 때, 조미료 3장을 패에 추가합니다.";
            item.animationKey = "Dotori Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.originCountDown = 2;
            item.cooking.BGMindex = 0;
        }
        else if (_objectName == "도토리") {
            item.isDotori = true;
            item.sprite = objectSprites[3];
        }
        else if (_objectName == "향기로운 도토리") {
            item.isDotori = true;
            item.sprite = objectSprites[11];
        }

        // 자주 쓰이는 조리도구
        else if (_objectName == "냄비(준비)") {
            item.usableSpellNames = new List<string>(1) { "불지피기" };
            item.sprite = objectSprites[4];
            item.xPivot = 0.07f;
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "냄비(조리중)";
            item.tool.neededSpiceNames = new List<string>() { "간장", "설탕", "해초 도토리" };
            if (deckName == "소고기 덮밥 레드와인") {
                item.tool.neededObjectNames = new List<string>(2) { "양파", "레드와인" };
                item.tool.neededObjectNums = new List<int>(2) { 1, 1 };
                item.tool.neededSpiceNames.Add("생강");
            }
            else {
                item.tool.neededObjectNames = new List<string>(1) { "양파" };
                item.tool.neededObjectNums = new List<int>(1) { 1 };
            }
            item.tool.useHeatPlate = true;
        }
        
        // 소고기 덮밥 기본
        else if (_objectName == "냄비(조리중)") {
            item.explain = "다음 오브젝트 : 소고기 덮밥이 든 냄비(준비)";
            item.animationKey = "Soup Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.nextObjectName = "소고기 덮밥이 든 냄비(준비)";
            item.cooking.originCountDown = 3;
            item.cooking.BGMindex = 0;
        }
        else if (_objectName == "소고기 덮밥이 든 냄비(준비)") {
            item.usableSpellNames = new List<string>(1) { "불지피기" };
            item.sprite = objectSprites[4];
            item.xPivot = 0.07f;
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "소고기 덮밥이 든 냄비(조리중)";
            item.tool.neededObjectNames = new List<string>(1) { "우삼겹" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
            item.tool.useHeatPlate = true;
        }
        else if (_objectName == "소고기 덮밥이 든 냄비(조리중)") {
            item.explain = "다음 오브젝트 : 소고기 덮밥이 든 냄비(완료)";
            item.animationKey = "Soup Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.nextObjectName = "소고기 덮밥이 든 냄비(완료)";
            item.cooking.originCountDown = 1;
            item.cooking.BGMindex = 0;
        }
        else if (_objectName == "소고기 덮밥이 든 냄비(완료)")
            item.sprite = objectSprites[4];
        else if (_objectName == "우삼겹")
            item.sprite = objectSprites[5];
        else if (_objectName == "양파")
            item.sprite = objectSprites[6];
        else if (_objectName == "생강") {
            item.isSpice = true;
            item.sprite = objectSprites[7];
        }
        else if (_objectName == "간장") {
            item.isSpice = true;
            item.sprite = objectSprites[8];
        }
        else if (_objectName == "설탕") {
            item.isSpice = true;
            item.sprite = objectSprites[9];
        }
        else if (_objectName == "해초 도토리") {
            item.isSpice = true;
            item.sprite = objectSprites[10];
        }
        //// 11, 12번 인덱스에는 "조미료", "도토리주" 오브젝트를 설정했음

        // 소고기 덮밥 레드와인
        else if (_objectName == "오크통(준비)") {
            item.usableSpellNames = new List<string>(1) { "오크통 밀폐" };
            item.sprite = objectSprites[13];
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "오크통(숙성중)";
            item.tool.neededObjectNames = new List<string>(2) { "포도", "향기로운 도토리" };
            item.tool.neededObjectNums = new List<int>(2) { 1, 1 };
        }
        else if (_objectName == "오크통(숙성중)") {
            item.explain = "숙성이 완료될 때 레드와인 1장과 조미료 3장을 덱에 추가합니다.";
            item.sprite = objectSprites[13];
            item.cooking = new ObjectSubItemCooking();
            item.cooking.originCountDown = 3;
        }
        else if (_objectName == "포도")
            item.sprite = objectSprites[14];
        else if (_objectName == "레드와인")
            item.sprite = objectSprites[15];

        localizedStringsJP["밥솥(준비)"] = "飯釜(準備)";
        localizedStringsJP["밥솥(조리중)"] = "飯釜(調理中)";
        localizedStringsJP["불지피기"] = "火つける";
        localizedStringsJP["쌀"] = "米";
        localizedStringsJP["도토리주"] = "どんぐりの酒";
        localizedStringsJP["다음 오브젝트 : 밥솥(완료)"] = "次のオブジェクト : 飯釜(完了)";
        localizedStringsJP["밥솥(완료)"] = "飯釜(完了)";
        localizedStringsJP["조미료"] = "調味料";
        localizedStringsJP["도토리 솥(준비)"] = "どんぐり釜(準備)";
        localizedStringsJP["도토리 굽기"] = "どんぐり焼く";
        localizedStringsJP["도토리 솥(조리중)"] = "どんぐり釜(調理中)";
        localizedStringsJP["도토리"] = "どんぐり";
        localizedStringsJP["조리가 완료될 때, 조미료 3장을 패에 추가합니다."] = "調理が完了した時, 調味料3枚を手札に加えます.";
        localizedStringsJP["향기로운 도토리"] = "香り豊かなどんぐり";
        localizedStringsJP["냄비(준비)"] = "鍋(準備)";
        localizedStringsJP["냄비(조리중)"] = "鍋(調理中)";
        localizedStringsJP["간장"] = "醤油";
        localizedStringsJP["설탕"] = "砂糖";
        localizedStringsJP["해초 도토리"] = "海草どんぐり";
        localizedStringsJP["양파"] = "玉ねぎ";
        localizedStringsJP["레드와인"] = "赤ワイン";
        localizedStringsJP["다음 오브젝트 : 소고기 덮밥이 든 냄비(준비)"] = "次のオブジェクト : 牛丼が入っている鍋(準備)";
        localizedStringsJP["소고기 덮밥이 든 냄비(준비)"] = "牛丼が入っている鍋(準備)";
        localizedStringsJP["소고기 덮밥이 든 냄비(조리중)"] = "牛丼が入っている鍋(調理中)";
        localizedStringsJP["우삼겹"] = "牛バラ肉";
        localizedStringsJP["다음 오브젝트 : 소고기 덮밥이 든 냄비(완료)"] = "次のオブジェクト : 牛丼が入っている鍋(完了)";
        localizedStringsJP["소고기 덮밥이 든 냄비(완료)"] = "牛丼が入っている鍋(完了)";
        localizedStringsJP["오크통(준비)"] = "オーク(準備)";
        localizedStringsJP["오크통 밀폐"] = "オーク密閉";
        localizedStringsJP["오크통(숙성중)"] = "オーク(熟成中)";
        localizedStringsJP["포도"] = "ぶどう";
        localizedStringsJP["숙성이 완료될 때 레드와인 1장과 조미료 3장을 덱에 추가합니다."] = "熟成が完了した時, 赤ワイン1枚と, 調味料3枚をデッキに加えます.";

        return item;
    }

    public void SetUp(string _deckName) {
        if (_deckName == "밥") {
            objectNames = new string[4];
            objectItems = new ObjectItem[4];

            objectNames[0] = "밥솥(준비)";
            objectItems[0] = SetUpItem("밥솥(준비)");
            objectNames[1] = "밥솥(조리중)";
            objectItems[1] = SetUpItem("밥솥(조리중)");
            objectNames[2] = "밥솥(완료)";
            objectItems[2] = SetUpItem("밥솥(완료)");
            objectNames[3] = "쌀";
            objectItems[3] = SetUpItem("쌀");
        }
        if (_deckName == "맛있는 밥") {
            objectNames = new string[10];
            objectItems = new ObjectItem[10];

            objectNames[0] = "밥솥(준비)";
            objectItems[0] = SetUpItem("밥솥(준비)");
            objectNames[1] = "밥솥(조리중)";
            objectItems[1] = SetUpItem("밥솥(조리중)");
            objectNames[2] = "밥솥(완료)";
            objectItems[2] = SetUpItem("밥솥(완료)");
            objectNames[3] = "쌀";
            objectItems[3] = SetUpItem("쌀");
            objectNames[4] = "조미료";
            objectItems[4] = SetUpItem("조미료");
            objectNames[5] = "도토리주";
            objectItems[5] = SetUpItem("도토리주");

            objectNames[6] = "도토리 솥(준비)";
            objectItems[6] = SetUpItem("도토리 솥(준비)");
            objectNames[7] = "도토리 솥(조리중)";
            objectItems[7] = SetUpItem("도토리 솥(조리중)");
            objectNames[8] = "도토리 솥(완료)";
            objectItems[8] = SetUpItem("도토리 솥(완료)");
            objectNames[9] = "도토리";
            objectItems[9] = SetUpItem("도토리");
        }
        else if (_deckName == "소고기 덮밥 기본" || _deckName == "소고기 덮밥 토핑 세트") {
            objectNames = new string[20];
            objectItems = new ObjectItem[20];

            // 기본
            objectNames[0] = "밥솥(준비)";
            objectItems[0] = SetUpItem("밥솥(준비)"); 
            objectNames[1] = "밥솥(조리중)";
			objectItems[1] = SetUpItem("밥솥(조리중)");
            objectNames[2] = "밥솥(완료)";
            objectItems[2] = SetUpItem("밥솥(완료)");
            objectNames[3] = "쌀";
            objectItems[3] = SetUpItem("쌀");
            objectNames[18] = "조미료";
            objectItems[18] = SetUpItem("조미료");
            objectNames[19] = "도토리주";
            objectItems[19] = SetUpItem("도토리주");

            // 기본 도토리
            objectNames[4] = "도토리 솥(준비)";
            objectItems[4] = SetUpItem("도토리 솥(준비)");
            objectNames[5] = "도토리 솥(조리중)";
            objectItems[5] = SetUpItem("도토리 솥(조리중)");
            objectNames[6] = "도토리 솥(완료)";
            objectItems[6] = SetUpItem("도토리 솥(완료)");
            objectNames[7] = "도토리";
            objectItems[7] = SetUpItem("도토리");

            // 소고기 덮밥 기본
            objectNames[8] = "냄비(준비)";
            objectItems[8] = SetUpItem("냄비(준비)");
            objectNames[9] = "냄비(조리중)";
            objectItems[9] = SetUpItem("냄비(조리중)");
            objectNames[10] = "소고기 덮밥이 든 냄비(준비)";
            objectItems[10] = SetUpItem("소고기 덮밥이 든 냄비(준비)");
            objectNames[11] = "소고기 덮밥이 든 냄비(조리중)";
            objectItems[11] = SetUpItem("소고기 덮밥이 든 냄비(조리중)");
            objectNames[12] = "소고기 덮밥이 든 냄비(완료)";
            objectItems[12] = SetUpItem("소고기 덮밥이 든 냄비(완료)");
            objectNames[13] = "우삼겹";
            objectItems[13] = SetUpItem("우삼겹");
            objectNames[14] = "양파";
            objectItems[14] = SetUpItem("양파");
            //objectNames[15] = "생강";
            //objectItems[15] = SetUpItem("생강");
            objectNames[15] = "간장";
            objectItems[15] = SetUpItem("간장");
            objectNames[16] = "설탕";
            objectItems[16] = SetUpItem("설탕");
            objectNames[17] = "해초 도토리";
            objectItems[17] = SetUpItem("해초 도토리");
            //// 19, 20번 인덱스에는 "조미료", "도토리주" 오브젝트를 설정했음
        }
        else if (_deckName == "소고기 덮밥 레드와인") {
            objectNames = new string[25];
            objectItems = new ObjectItem[25];

            // 기본
            objectNames[0] = "밥솥(준비)";
            objectItems[0] = SetUpItem("밥솥(준비)");
            objectNames[1] = "밥솥(조리중)";
            objectItems[1] = SetUpItem("밥솥(조리중)");
            objectNames[2] = "밥솥(완료)";
            objectItems[2] = SetUpItem("밥솥(완료)");
            objectNames[3] = "쌀";
            objectItems[3] = SetUpItem("쌀");
            objectNames[19] = "조미료";
            objectItems[19] = SetUpItem("조미료");
            objectNames[20] = "도토리주";
            objectItems[20] = SetUpItem("도토리주");

            // 기본 도토리
            objectNames[4] = "도토리 솥(준비)";
            objectItems[4] = SetUpItem("도토리 솥(준비)");
            objectNames[5] = "도토리 솥(조리중)";
            objectItems[5] = SetUpItem("도토리 솥(조리중)");
            objectNames[6] = "도토리 솥(완료)";
            objectItems[6] = SetUpItem("도토리 솥(완료)");
            objectNames[7] = "도토리";
            objectItems[7] = SetUpItem("도토리");

            // 소고기 덮밥 토핑 세트
            objectNames[8] = "냄비(준비)";
            objectItems[8] = SetUpItem("냄비(준비)");
            objectNames[9] = "냄비(조리중)";
            objectItems[9] = SetUpItem("냄비(조리중)");
            objectNames[10] = "소고기 덮밥이 든 냄비(준비)";
            objectItems[10] = SetUpItem("소고기 덮밥이 든 냄비(준비)");
            objectNames[11] = "소고기 덮밥이 든 냄비(조리중)";
            objectItems[11] = SetUpItem("소고기 덮밥이 든 냄비(조리중)");
            objectNames[12] = "소고기 덮밥이 든 냄비(완료)";
            objectItems[12] = SetUpItem("소고기 덮밥이 든 냄비(완료)");
            objectNames[13] = "우삼겹";
            objectItems[13] = SetUpItem("우삼겹");
            objectNames[14] = "양파";
            objectItems[14] = SetUpItem("양파");
            objectNames[15] = "생강";
            objectItems[15] = SetUpItem("생강");
            objectNames[16] = "간장";
            objectItems[16] = SetUpItem("간장");
            objectNames[17] = "설탕";
            objectItems[17] = SetUpItem("설탕");
            objectNames[18] = "해초 도토리";
            objectItems[18] = SetUpItem("해초 도토리");
            //// 19, 20번 인덱스에는 "조미료", "도토리주" 오브젝트를 설정했음
            objectNames[21] = "오크통(준비)";
            objectItems[21] = SetUpItem("오크통(준비)");
            objectNames[22] = "오크통(숙성중)";
            objectItems[22] = SetUpItem("오크통(숙성중)");
            objectNames[23] = "포도";
            objectItems[23] = SetUpItem("포도");
            objectNames[24] = "레드와인";
            objectItems[24] = SetUpItem("레드와인");
        }
    }

    public ObjectItem GetObjectItem(string _objectName) {
        ObjectItem result = new ObjectItem();

        for (int i = 0; i < objectNames.Length; i++) {
            if (objectNames[i] == _objectName) {
                result = objectItems[i];
                return result;
            }
        }

        return result;
    }

    public void ExecuteObjectFunc(string _objectName, SO_Timing _timing, int _x = -1, int _y = -1) {
        // 오브젝트 효과 발동
        switch (_timing) {
            case SO_Timing.Start:

                break;
            case SO_Timing.Effect:

                break;
            case SO_Timing.End:
                if (_objectName == "도토리 솥(조리중)") {
                    CardMngScript.CardItemSO.AddCardToDeck("조미료", 3);
                    CardMngScript.AddCreatedSpiceNum(3);
                }
                else if (_objectName == "오크통(숙성중)") {
                    CardMngScript.CardItemSO.AddCardToDeck("레드와인");
                    CardMngScript.CardItemSO.AddCardToDeck("조미료", 3);
                    CardMngScript.AddCreatedSpiceNum(3);
                }
                break;
        }
    }
}
