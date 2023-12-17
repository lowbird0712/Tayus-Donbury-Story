using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SO_CardType {
    Placable = 1,
    Spell = 2,
    NonTargetSpell = 3,
    DotoriTargetSpell = 4
}

public enum SO_Dotori {
    Default = 0,
    DotoriSide = 1,
    DotoriBackSide = 2
}

public class CardItem {
    public SO_CardType  type = SO_CardType.Placable;
    public Sprite       sprite = null;
    public string       explain = null;
    public bool         isDotori = false;
    public int          neededDotoriNum;
    public int          SFXindex = -1;
}

[CreateAssetMenu(fileName = "SO_CardItem", menuName = "Scriptable Object/SO_CardItem")]
public class SO_CardItemScript : ScriptableObject {
    [SerializeField] Sprite[]           cardSprites = null;

    string[]                            cardNames = null;
    Dictionary<string, string>          localizedCardNameDictionaryKR = new Dictionary<string, string>();
    Dictionary<string, string>          localizedCardNameDictionaryJP = new Dictionary<string, string>();
    CardItem[]                          cardItems = null;

    public Dictionary<string, string>   LocalizedCardNameDictionaryKR => localizedCardNameDictionaryKR;

    public string GetLocalizedCardName(string _koreanKey) {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되지 않고 있습니다!!!!");
                return null;
            case 1:
                return _koreanKey;
            case 2:
                return localizedCardNameDictionaryJP[_koreanKey];
            default:
                Debug.LogError("해당 언어는 아직 지원되지 않고 있습니다!!!!");
                return null;
        }
    }

    public string GetReverseLocalizedCardName(string _foreignKey) {
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되지 않고 있습니다!!!!");
                return null;
            case 1:
                return _foreignKey;
            case 2:
                return localizedCardNameDictionaryKR[_foreignKey];
            default:
                Debug.LogError("해당 언어는 아직 지원되지 않고 있습니다!!!!");
                return null;
        }
    }

    CardItem SetUpItem(string _cardName) {
        CardItem item = new CardItem();
        switch (LocalizeManager.CurrentLanguageIndex) {
            case 0:
                Debug.LogError("아직 영어는 지원되고 있지 않습니다!!!!");
                break;
            case 1:
                // 기본
                if (_cardName == "밥솥") {
                    item.sprite = cardSprites[0];
                    item.explain = "가장 앞의 그리드에 밥솥(준비)을 놓는다.";
                }
                else if (_cardName == "불지피기") {
                    item.sprite = cardSprites[1];
                    item.explain = "불을 지필 수 있는 도구가 그리드 위에 있으면 가장 앞의 도구에 불을 지핀다.";
                    item.SFXindex = 4;
                    item.type = SO_CardType.Spell;
                }
                else if (_cardName == "쌀") {
                    item.sprite = cardSprites[2];
                    item.explain = "가장 앞의 그리드에 쌀을 놓는다.";
                }
                else if (_cardName == "조미료") {
                    item.sprite = cardSprites[3];
                    item.explain = "가장 앞의 그리드에 조미료를 놓는다. 인접한 그리드에 조미료를 필요로 하는 도구가 있으면 아직 넣지 않은 조미료 중 하나를 랜덤으로 놓는다.";
                }

                // 기본 도토리
                else if (_cardName == "도토리 솥") {
                    item.sprite = cardSprites[4];
                    item.explain = "도토리를 넣을 수 있는 솥이다. 가장 앞의 그리드에 도토리 솥을 놓는다.";
                    item.isDotori = true;
                }
                else if (_cardName == "도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "탐스러운 도토리이다. 섬세하게 조절된 불로 구우면 쓸만한 조미료가 된다. 가장 우선의 그리드에 도토리를 놓는다.";
                    item.isDotori = true;
                }
                else if (_cardName == "도토리 굽기") {
                    item.sprite = cardSprites[6];
                    item.explain = "섬세하게 조절된 불로 도토리를 굽는다. 도토리를 구울 수 있는 도구가 그리드 위에 있으면 가장 앞의 도구에 불을 지핀다.";
                    item.SFXindex = 4;
                    item.type = SO_CardType.Spell;
                    item.isDotori = true;
                }
                else if (_cardName == "향기로운 도토리") {
                    item.sprite = cardSprites[3];
                    item.explain = "도토리가 강렬한 향기를 내뿜고 있다. 식욕을 돋우는 향이다.";
                    item.isDotori = true;
                }

                // 소고기 덮밥 기본
                else if (_cardName == "냄비") {
                    item.sprite = cardSprites[8];
                    item.explain = "가장 앞의 그리드에 냄비(준비)을 놓는다.";
                }
                else if (_cardName == "우삼겹") {
                    item.sprite = cardSprites[9];
                    item.explain = "가장 앞의 그리드에 우삼겹을 놓는다.";
                }
                else if (_cardName == "양파") {
                    item.sprite = cardSprites[10];
                    item.explain = "가장 앞의 그리드에 양파를 놓는다.";
                }

                // 소고기 덮밥 기본 도토리
                else if (_cardName == "냄비와 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "도토리가 냄비 안에 들어 있다.";
                    item.isDotori = true;
                }
                else if (_cardName == "우삼겹과 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "도토리가 우삼겹 옆에 놓여 있다.";
                    item.isDotori = true;
                }
                else if (_cardName == "양파와 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "도토리가 양파 옆에 놓여 있다.";
                    item.isDotori = true;
                }

                // 소고기 덮밥 토핑 세트
                else if (_cardName == "토핑 추가") {
                    item.type = SO_CardType.NonTargetSpell;
                    item.sprite = cardSprites[14];
                    item.neededDotoriNum = 2;
                    item.explain = "어떤 덮밥이든 토핑이 더해지면 더욱 맛과 향이 풍부해진다.\n\n" +
                        "도토리2 : 조미료 3장을 덱에 추가한다.";
                }

                // 소고기 덮밥 토핑 세트 도토리
                else if (_cardName == "토핑과 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "안정적인 감칠맛을 가진 도토리를 갈아 만든 가루. 덮밥에 뿌리면 요리의 맛이 한 층 올라간다.";
                    item.isDotori = true;
                }

                // 소고기 덮밥 레드와인
                else if (_cardName == "오크통 배치") {
                    item.type = SO_CardType.DotoriTargetSpell;
                    item.sprite = cardSprites[15];
                    item.neededDotoriNum = 1;
                    item.explain = "와인을 빚는 데 사용되는 오크통. 안에 도토리가 들어 있어 숙성된 술의 맛이 더욱 좋아진다.\n\n" +
                        "도토리1 : 사용된 도토리 오브젝트의 위치에 오크통을 배치한다.";
                }
                else if (_cardName == "포도") {
                    item.sprite = cardSprites[16];
                    item.explain = "가장 앞의 그리드에 포도를 놓는다.";
                }
                else if (_cardName == "오크통 밀폐") {
                    item.sprite = cardSprites[17];
                    item.explain = "오크통을 닫아 공기로부터 차단시킨다. 오크통 안의 포도와 도토리가 숙성되기 시작한다.";
                    item.SFXindex = 3;
                    item.type = SO_CardType.Spell;
                    item.isDotori = true;
                }
                else if (_cardName == "레드와인") {
                    item.sprite = cardSprites[18];
                    item.explain = "가장 앞의 그리드에 레드와인을 놓는다.";
                }

                // 소고기 덮밥 레드와인 도토리
                else if (_cardName == "포도와 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "도토리가 포도 옆에 놓여 있다.";
                    item.isDotori = true;
                }
                else if (_cardName == "레드와인과 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "도토리가 레드와인 옆에 놓여 있다.";
                    item.isDotori = true;
                }
                break;
            case 2:
                // 기본
                if (_cardName == "밥솥") {
                    item.sprite = cardSprites[0];
                    item.explain = "一番前のグリッドに飯釜(準備)を置く.";
                }
                else if (_cardName == "불지피기") {
                    item.sprite = cardSprites[1];
                    item.explain = "火をつけられる道具がグリッド上にあると, 一番前の道具に火をつける,";
                    item.SFXindex = 4;
                    item.type = SO_CardType.Spell;
                }
                else if (_cardName == "쌀") {
                    item.sprite = cardSprites[2];
                    item.explain = "一番前のグリッドに米を置く.";
                }
                else if (_cardName == "조미료") {
                    item.sprite = cardSprites[3];
                    item.explain = "一番前のグリッドに調味料を置く. 接しているグリッドに調味料を必要とする道具があれば, まだ入れてない調味料の中, 一つをランダムに置く.";
                }

                // 기본 도토리
                else if (_cardName == "도토리 솥") {
                    item.sprite = cardSprites[4];
                    item.explain = "どんぐりを入れられる釜だ. 一番前のグリッドににどんぐり釜を置く.";
                    item.isDotori = true;
                }
                else if (_cardName == "도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "大きく丸いどんぐりだ. 繊細に操作された火で焼くと, 使える調味料になる. 一番前のグリッドにどんぐりを置く.";
                    item.isDotori = true;
                }
                else if (_cardName == "도토리 굽기") {
                    item.sprite = cardSprites[6];
                    item.explain = "繊細に操作された火でどんぐりを焼く. どんぐりを焼ける道具がグリッド上にあると一番前のグリッドに火をつける.";
                    item.SFXindex = 4;
                    item.type = SO_CardType.Spell;
                    item.isDotori = true;
                }
                else if (_cardName == "향기로운 도토리") {
                    item.sprite = cardSprites[3];
                    item.explain = "どんぐりが強烈な香りを湧き出している. 食欲が湧く香りだ.";
                    item.isDotori = true;
                }

                // 소고기 덮밥 기본
                else if (_cardName == "냄비") {
                    item.sprite = cardSprites[8];
                    item.explain = "一番前のグリッドに鍋(準備)を置く.";
                }
                else if (_cardName == "우삼겹") {
                    item.sprite = cardSprites[9];
                    item.explain = "一番前のグリッドに牛バラ肉を置く.";
                }
                else if (_cardName == "양파") {
                    item.sprite = cardSprites[10];
                    item.explain = "一番前のグリッドに玉ねぎを置く.";
                }

                // 소고기 덮밥 기본 도토리
                else if (_cardName == "냄비와 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "どんぐりが鍋の中に入っている.";
                    item.isDotori = true;
                }
                else if (_cardName == "우삼겹과 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "どんぐりが牛バラ肉の隣に置いてある.";
                    item.isDotori = true;
                }
                else if (_cardName == "양파와 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "どんぐりが玉ねぎの隣に置いてある.";
                    item.isDotori = true;
                }

                // 소고기 덮밥 토핑 세트
                else if (_cardName == "토핑 추가") {
                    item.type = SO_CardType.NonTargetSpell;
                    item.sprite = cardSprites[14];
                    item.neededDotoriNum = 2;
                    item.explain = "どんな丼でもトッピングが加えられると, より味と香りが豊富になる.\n\n" +
                        "どんぐり2 : 調味料3枚をデッキに加える.";
                }

                // 소고기 덮밥 토핑 세트 도토리
                else if (_cardName == "토핑과 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "安定的な旨味を持つどんぐりをすりおろして作った粉. 丼に降ると料理の味が一段階上がる.";
                    item.isDotori = true;
                }

                // 소고기 덮밥 레드와인
                else if (_cardName == "오크통 배치") {
                    item.type = SO_CardType.DotoriTargetSpell;
                    item.sprite = cardSprites[15];
                    item.neededDotoriNum = 1;
                    item.explain = "ワインを作るのに使用されるオーク. 中にどんぐりが入っており, 熟成されたお酒の味がさらに良くなる.\n\n" +
                        "どんぐり1 : 使用されたどんぐりオブジェクトのいちにオークを置く.";
                }
                else if (_cardName == "포도") {
                    item.sprite = cardSprites[16];
                    item.explain = "一番前のグリッドにぶどうを置く.";
                }
                else if (_cardName == "오크통 밀폐") {
                    item.sprite = cardSprites[17];
                    item.explain = "オークを閉め, 空気から遮断させる. オーク内のぶどうとどんぐりが熟成し始める.";
                    item.SFXindex = 3;
                    item.type = SO_CardType.Spell;
                    item.isDotori = true;
                }
                else if (_cardName == "레드와인") {
                    item.sprite = cardSprites[18];
                    item.explain = "一番前のグリッドに赤ワインを置く.";
                }

                // 소고기 덮밥 레드와인 도토리
                else if (_cardName == "포도와 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "どんぐりがぶどうの隣に置いてある.";
                    item.isDotori = true;
                }
                else if (_cardName == "레드와인과 도토리") {
                    item.sprite = cardSprites[5];
                    item.explain = "どんぐりが赤ワインの隣に置いてある.";
                    item.isDotori = true;
                }
                break;
        }
        return item;
	}

    // 도토리 관련 함수
    void ChangeToDotori(string _cardName) {
        _cardName = GetReverseLocalizedCardName(_cardName);

        // 기본
        if (_cardName == "불지피기")
            CreateCardToHand("도토리 굽기");
        else if (_cardName == "밥솥")
            CreateCardToHand("도토리 솥");
        else if (_cardName == "쌀")
            CreateCardToHand("도토리");
        else if (_cardName == "조미료")
            CreateCardToHand("향기로운 도토리");

        // 소고기 덮밥 기본
        else if (_cardName == "냄비")
            CreateCardToHand("냄비와 도토리");
        else if (_cardName == "우삼겹")
            CreateCardToHand("우삼겹과 도토리");
        else if (_cardName == "양파")
            CreateCardToHand("양파와 도토리");

        // 소고기 덮밥 토핑 세트
        else if (_cardName == "토핑 추가")
            CreateCardToHand("토핑과 도토리");

        // 소고기 덮밥 레드와인
        else if (_cardName == "오크통 배치")
            CreateCardToHand("오크통 밀폐");
        else if (_cardName == "포도")
            CreateCardToHand("포도와 도토리");
        else if (_cardName == "레드와인")
            CreateCardToHand("레드와인과 도토리");
    }

    void DotoriReturn(string _cardName) {
        _cardName = GetReverseLocalizedCardName(_cardName);

        // 기본 도토리
        if (_cardName == "도토리 솥")
            AddCardToDeck("밥솥");
        else if (_cardName == "도토리 굽기")
            AddCardToDeck("불지피기");
        else if (_cardName == "도토리")
            AddCardToDeck("쌀");
        else if (_cardName == "향기로운 도토리")
            AddCardToDeck("조미료");

        // 소고기 덮밥 기본 도토리
        else if (_cardName == "냄비와 도토리")
            AddCardToDeck("냄비");
        else if (_cardName == "우삼겹과 도토리")
            AddCardToDeck("우삼겹");
        else if (_cardName == "양파와 도토리")
            AddCardToDeck("양파");

        // 소고기 덮밥 토핑 도토리
        else if (_cardName == "토핑과 도토리")
            AddCardToDeck("토핑 추가");

        // 소고기 덮밥 레드와인
        else if (_cardName == "오크통 밀폐")
            AddCardToDeck("오크통 배치");
        else if (_cardName == "포도와 도토리")
            AddCardToDeck("포도");
        else if (_cardName == "레드와인과 도토리")
            AddCardToDeck("레드와인");

        CardMngScript.AddCardItem();
    }

    bool CanUseSpell(string _cardName) {
        SO_CardType type = GetCardItem(_cardName).type;
        if (type == SO_CardType.NonTargetSpell || type == SO_CardType.DotoriTargetSpell)
            return HasEnoughDotori(_cardName);

        CurrentObjectItem   current;
        int                 spellNum = 0;
        int                 objectNum = 0;
        foreach (var card in CardMngScript.PutCards) {
            if (card.CardName != _cardName)
                continue;
            spellNum++;
        }
        foreach (var gridObject in GridObjectMngScript.GridObjects) {
            current = gridObject.CurrentObjectItem;
            if (current == null)
                continue;
            if (current.currentSpellNames == null)
                continue;
            if (!current.currentSpellNames.Contains(GetReverseLocalizedCardName(_cardName)))
                continue;
            objectNum++;
        }

        return (spellNum + 1 <= objectNum) && HasEnoughDotori(_cardName);
    }

    bool HasEnoughDotori(string _cardName) {
        CardItem    cardTmp;
        int         neededDotoriNum = 0;
        foreach (var card in CardMngScript.PutCards) {
            if (card.CardName == null)
                continue;
            cardTmp = GetCardItem(card.CardName);
            neededDotoriNum += cardTmp.neededDotoriNum;
        }

        return GetCardItem(_cardName).neededDotoriNum + neededDotoriNum <= GridObjectMngScript.GetDotoriObjectNum();
    }

    GridObjectScript GetSpellNextGridObject(string _cardName) {
        if (GetCardItem(GetLocalizedCardName(_cardName)).type != SO_CardType.Spell)
            Debug.LogError("_cardName의 카드는 스펠 카드가 아닙니다!");

        CurrentObjectItem current;
        foreach (var gridObject in GridObjectMngScript.GridObjects) {
            current = gridObject.CurrentObjectItem;
            if (current == null)
                continue;
            if (current.currentSpellNames == null)
                continue;
            if (!current.currentSpellNames.Contains(_cardName))
                continue;
            return gridObject;
        }
        return null;
    }

    bool IsSpiceToolExist() {
        foreach (var gridObject in GridObjectMngScript.GetAdjacentGridObjects()) {
            if (!gridObject.HasObject)
                continue;
            ObjectItem objectItem = GridObjectMngScript.GridObjectSO.GetObjectItem(gridObject.ObjectName);
            if (objectItem.tool == null)
                continue;
            if (objectItem.tool.neededSpiceNames == null)
                continue;
            if (objectItem.tool.neededSpiceNames.Count != gridObject.CurrentObjectItem.currentSpiceNames.Count)
                return true;
        }
        return false;
    }

    // 카드 효과 함수
    public void CreateCardToHand(string _cardName, int _num = 1) {
        for (int i = 0; i < _num; i++) {
            CardMngScript.CardBuffer.Insert(0, _cardName);
            CardMngScript.AddCardItem();
        }
    }

    public void AddCardToDeck(string _cardName, int _num = 1) {
        int index;
        for (int i = 0; i < _num; i++) {
            index = Random.Range(0, CardMngScript.CardBuffer.Count + 1);
            if (index == CardMngScript.CardBuffer.Count + 1)
                CardMngScript.CardBuffer.Add(_cardName);
            else
                CardMngScript.CardBuffer.Insert(index, _cardName);
        }
    }

    public void SetUp(string _deckName) {
        if (_deckName == "밥") {
            cardNames = new string[3];
            cardItems = new CardItem[3];

            cardNames[0] = "밥솥";
            cardItems[0] = SetUpItem("밥솥");
            cardNames[1] = "불지피기";
            cardItems[1] = SetUpItem("불지피기");
            cardNames[2] = "쌀";
            cardItems[2] = SetUpItem("쌀");
        }
        if (_deckName == "맛있는 밥") {
            cardNames = new string[8];
            cardItems = new CardItem[8];

            cardNames[0] = "밥솥";
            cardItems[0] = SetUpItem("밥솥");
            cardNames[1] = "불지피기";
            cardItems[1] = SetUpItem("불지피기");
            cardNames[2] = "쌀";
            cardItems[2] = SetUpItem("쌀");

            cardNames[3] = "도토리 솥";
            cardItems[3] = SetUpItem("도토리 솥");
            cardNames[4] = "도토리 굽기";
            cardItems[4] = SetUpItem("도토리 굽기");
            cardNames[5] = "도토리";
            cardItems[5] = SetUpItem("도토리");
            cardNames[6] = "조미료";
            cardItems[6] = SetUpItem("조미료");
            cardNames[7] = "향기로운 도토리";
            cardItems[7] = SetUpItem("향기로운 도토리");
        }
        else if (_deckName == "소고기 덮밥 기본") {
            cardNames = new string[14];
            cardItems = new CardItem[14];

            cardNames[0] = "밥솥";
            cardItems[0] = SetUpItem("밥솥");
            cardNames[1] = "냄비";
            cardItems[1] = SetUpItem("냄비");
            cardNames[2] = "불지피기";
            cardItems[2] = SetUpItem("불지피기");
            cardNames[3] = "쌀";
            cardItems[3] = SetUpItem("쌀");
            cardNames[4] = "우삼겹";
            cardItems[4] = SetUpItem("우삼겹");
            cardNames[5] = "양파";
            cardItems[5] = SetUpItem("양파");
            cardNames[6] = "조미료";
            cardItems[6] = SetUpItem("조미료");

            cardNames[7] = "도토리 솥";
            cardItems[7] = SetUpItem("도토리 솥");
            cardNames[8] = "냄비와 도토리";
            cardItems[8] = SetUpItem("냄비와 도토리");
            cardNames[9] = "도토리 굽기";
            cardItems[9] = SetUpItem("도토리 굽기");
            cardNames[10] = "도토리";
            cardItems[10] = SetUpItem("도토리");
            cardNames[11] = "우삼겹과 도토리";
            cardItems[11] = SetUpItem("우삼겹과 도토리");
            cardNames[12] = "양파와 도토리";
            cardItems[12] = SetUpItem("양파와 도토리");
            cardNames[13] = "향기로운 도토리";
            cardItems[13] = SetUpItem("향기로운 도토리");
        }
        else if (_deckName == "소고기 덮밥 토핑 세트") {
            cardNames = new string[16];
            cardItems = new CardItem[16];

            cardNames[0] = "밥솥";
            cardItems[0] = SetUpItem("밥솥");
            cardNames[1] = "냄비";
            cardItems[1] = SetUpItem("냄비");
            cardNames[2] = "불지피기";
            cardItems[2] = SetUpItem("불지피기");
            cardNames[3] = "쌀";
            cardItems[3] = SetUpItem("쌀");
            cardNames[4] = "우삼겹";
            cardItems[4] = SetUpItem("우삼겹");
            cardNames[5] = "양파";
            cardItems[5] = SetUpItem("양파");
            cardNames[6] = "조미료";
            cardItems[6] = SetUpItem("조미료");
            cardNames[7] = "토핑 추가";
            cardItems[7] = SetUpItem("토핑 추가");

            cardNames[8] = "도토리 솥";
            cardItems[8] = SetUpItem("도토리 솥");
            cardNames[9] = "냄비와 도토리";
            cardItems[9] = SetUpItem("냄비와 도토리");
            cardNames[10] = "도토리 굽기";
            cardItems[10] = SetUpItem("도토리 굽기");
            cardNames[11] = "도토리";
            cardItems[11] = SetUpItem("도토리");
            cardNames[12] = "우삼겹과 도토리";
            cardItems[12] = SetUpItem("우삼겹과 도토리");
            cardNames[13] = "양파와 도토리";
            cardItems[13] = SetUpItem("양파와 도토리");
            cardNames[14] = "향기로운 도토리";
            cardItems[14] = SetUpItem("향기로운 도토리");
            cardNames[15] = "토핑과 도토리";
            cardItems[15] = SetUpItem("토핑과 도토리");
        }
        else if (_deckName == "소고기 덮밥 레드와인") {
            cardNames = new string[20];
            cardItems = new CardItem[20];

            cardNames[0] = "밥솥";
            cardItems[0] = SetUpItem("밥솥");
            cardNames[1] = "냄비";
            cardItems[1] = SetUpItem("냄비");
            cardNames[2] = "불지피기";
            cardItems[2] = SetUpItem("불지피기");
            cardNames[3] = "쌀";
            cardItems[3] = SetUpItem("쌀");
            cardNames[4] = "우삼겹";
            cardItems[4] = SetUpItem("우삼겹");
            cardNames[5] = "양파";
            cardItems[5] = SetUpItem("양파");
            cardNames[6] = "조미료";
            cardItems[6] = SetUpItem("조미료");

            cardNames[7] = "도토리 솥";
            cardItems[7] = SetUpItem("도토리 솥");
            cardNames[8] = "냄비와 도토리";
            cardItems[8] = SetUpItem("냄비와 도토리");
            cardNames[9] = "도토리 굽기";
            cardItems[9] = SetUpItem("도토리 굽기");
            cardNames[10] = "도토리";
            cardItems[10] = SetUpItem("도토리");
            cardNames[11] = "우삼겹과 도토리";
            cardItems[11] = SetUpItem("우삼겹과 도토리");
            cardNames[12] = "양파와 도토리";
            cardItems[12] = SetUpItem("양파와 도토리");
            cardNames[13] = "향기로운 도토리";
            cardItems[13] = SetUpItem("향기로운 도토리");
            cardNames[14] = "오크통 배치";
            cardItems[14] = SetUpItem("오크통 배치");
            cardNames[15] = "포도";
            cardItems[15] = SetUpItem("포도");
            cardNames[16] = "오크통 밀폐";
            cardItems[16] = SetUpItem("오크통 밀폐");
            cardNames[17] = "포도와 도토리";
            cardItems[17] = SetUpItem("포도와 도토리");
            cardNames[18] = "레드와인";
            cardItems[18] = SetUpItem("레드와인");
            cardNames[19] = "레드와인과 도토리";
            cardItems[19] = SetUpItem("레드와인과 도토리");
        }

        localizedCardNameDictionaryJP["밥솥"] = "飯釜";
        localizedCardNameDictionaryJP["쌀"] = "米";
        localizedCardNameDictionaryJP["불지피기"] = "火つける";
        localizedCardNameDictionaryJP["냄비"] = "鍋";
        localizedCardNameDictionaryJP["우삼겹"] = "牛バラ肉";
        localizedCardNameDictionaryJP["양파"] = "玉ねぎ";
        localizedCardNameDictionaryJP["조미료"] = "調味料";
        localizedCardNameDictionaryJP["도토리 솥"] = "どんぐり釜";
        localizedCardNameDictionaryJP["도토리"] = "どんぐり";
        localizedCardNameDictionaryJP["도토리 굽기"] = "どんぐり焼く";
        localizedCardNameDictionaryJP["냄비와 도토리"] = "鍋とどんぐり";
        localizedCardNameDictionaryJP["우삼겹과 도토리"] = "牛バラ肉とどんぐり";
        localizedCardNameDictionaryJP["양파와 도토리"] = "玉ねぎとどんぐり";
        localizedCardNameDictionaryJP["향기로운 도토리"] = "香り豊かなどんぐり";
        localizedCardNameDictionaryJP["토핑 추가"] = "トッピング追加";
        localizedCardNameDictionaryJP["토핑과 도토리"] = "トッピングとどんぐり";
        localizedCardNameDictionaryJP["오크통"] = "オーク";
        localizedCardNameDictionaryJP["오크통 밀폐"] = "オーク密閉";
        localizedCardNameDictionaryJP["포도"] = "ぶどう";
        localizedCardNameDictionaryJP["포도와 도토리"] = "ぶどうとどんぐり";
        localizedCardNameDictionaryJP["레드와인"] = "赤ワイン";
        localizedCardNameDictionaryJP["레드와인과 도토리"] = "赤ワインとどんぐり";
        foreach (var pair in localizedCardNameDictionaryJP)
            localizedCardNameDictionaryKR[pair.Value] = pair.Key;
    }

    public CardItem GetCardItem(string _cardName) {
        CardItem result = new CardItem();

        for (int i = 0; i < cardNames.Length; i++) {
            if (GetLocalizedCardName(cardNames[i]) == _cardName) {
                result = cardItems[i];
                return result;
            }
        }

        Debug.LogError("찾으려는 CardItem이 Scriptable Object에 없습니다!");
        return result;
    }

    public string GetSpellAnimationKey(string _cardName) {
        if (GetCardItem(GetLocalizedCardName(_cardName)).type != SO_CardType.Spell)
            Debug.LogError("_cardName의 카드는 스펠 카드가 아닙니다!");

        if (_cardName == "불지피기")
            return "Fire";
        else if (_cardName == "도토리 굽기")
            return "Fire";
        else
            return null;
	}

    public void ExecuteCardFunc(string _cardName, SO_Dotori _dotoriFlag) {
        GridObjectScript    nextGridObject;
        SO_CardType         type;
        int                 SFXindex;
        int                 x;
        int                 y;

        if (_dotoriFlag == SO_Dotori.Default) {
            type = GetCardItem(_cardName).type;
            if ((SFXindex = GetCardItem(_cardName).SFXindex) != -1)
                SoundMngScript.PlaySFX(SFXindex);

            _cardName = GetReverseLocalizedCardName(_cardName);

            if (type == SO_CardType.Spell)
                nextGridObject = GetSpellNextGridObject(_cardName);
            else if (type == SO_CardType.DotoriTargetSpell)
                nextGridObject = GridObjectMngScript.GetDotoriObjects()[0];
            else
                nextGridObject = GridObjectMngScript.NextGridObject;
            x = nextGridObject.Position[0];
            y = nextGridObject.Position[1];

            // 기본
            if (_cardName == "밥솥")
                GridObjectMngScript.PlaceObject("밥솥(준비)", x, y);
            else if (_cardName == "불지피기") {
                GridObjectMngScript.Spell("불지피기", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "쌀")
                GridObjectMngScript.PlaceObject("쌀", x, y);
            else if (_cardName == "조미료") {
                if (IsSpiceToolExist())
                    GridObjectMngScript.PlaceObject(GridObjectMngScript.GetRandomNeededSpice(), x, y);
                else
                    GridObjectMngScript.PlaceObject("조미료", x, y); ;
                CardMngScript.AddConsumedSpiceNum(1);
            }

            // 기본 도토리
            else if (_cardName == "도토리 솥")
                GridObjectMngScript.PlaceObject("도토리 솥(준비)", x, y);
            else if (_cardName == "도토리")
                GridObjectMngScript.PlaceObject("도토리", x, y);
            else if (_cardName == "도토리 굽기") {
                GridObjectMngScript.Spell("도토리 굽기", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "향기로운 도토리") {
                GridObjectMngScript.PlaceObject("향기로운 도토리", x, y);
                CardMngScript.AddConsumedSpiceNum(1);
            }

            // 소고기 덮밥 기본
            else if (_cardName == "냄비")
                GridObjectMngScript.PlaceObject("냄비(준비)", x, y);
            else if (_cardName == "양파")
                GridObjectMngScript.PlaceObject("양파", x, y);
            else if (_cardName == "우삼겹")
                GridObjectMngScript.PlaceObject("우삼겹", x, y);

            // 소고기 덮밥 토핑 세트
            else if (_cardName == "토핑 추가") {
                GridObjectMngScript.UseDotoriObject(2);
                CardMngScript.AddCreatedSpiceNum(3);
                AddCardToDeck("조미료", 3);
            }

            // 소고기 덮밥 레드와인
            else if (_cardName == "오크통 배치") {
                GridObjectMngScript.UseDotoriObject(1);
                GridObjectMngScript.PlaceObject("오크통(준비)", x, y);
            }
            else if (_cardName == "포도")
                GridObjectMngScript.PlaceObject("포도", x, y);
            else if (_cardName == "오크통 밀폐") {
                GridObjectMngScript.Spell("오크통 밀폐", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "레드와인")
                GridObjectMngScript.PlaceObject("레드와인", x, y);

            // 도토리 카드
            else
                GridObjectMngScript.PlaceObject(_cardName, x, y);
        }
        else if (_dotoriFlag == SO_Dotori.DotoriSide)
            ChangeToDotori(_cardName);
        else if (_dotoriFlag == SO_Dotori.DotoriBackSide)
            DotoriReturn(_cardName);
    }

    public bool GetUsable(string _cardName) {
        SO_CardType type = GetCardItem(_cardName).type;
        if (type == SO_CardType.Placable && (GridObjectMngScript.NextGridObject == null ||
            GridObjectMngScript.NextGridObjectIndex + CardMngScript.GetPlacablePutCardNum() >= GridObjectMngScript.GridObjects.Count))
            return false;
        else if ((type == SO_CardType.Spell || type == SO_CardType.NonTargetSpell || type == SO_CardType.DotoriTargetSpell) && !CanUseSpell(_cardName))
            return false;
        return true;
    }
}
