using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;
using IEnumerator = System.Collections.IEnumerator;
using UniRx;
using UniRx.Triggers;

public class CardMngScript : MonoBehaviour {
    public static CardMngScript         Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] SO_CardItemScript  SO_cardItem;
    [SerializeField] GameObject         cardPrefab;
    
    [SerializeField] Transform          cardLeftLine;
    [SerializeField] Transform          cardRightLine;
    [SerializeField] Transform          cardSpawnPoint;
    [SerializeField] Transform          cardPutUpPoint;
    [SerializeField] GameObject         cardPutCover;
    [SerializeField] Transform          cardPutLeftLine;
    [SerializeField] Transform          cardPutRightLine;
    [SerializeField] Transform          cardPutStartingPoint;
    [SerializeField] Transform          cardExecuteEndingPoint;
    [SerializeField] CardScript         emptyCard;
    [SerializeField] Text               cardBufferNumText;
    [SerializeField] Text               spiceNumText;
    [SerializeField] Text               cardPutCountText;

    enum ECardState                     { Nothing, OnlyMouseClick, CanMouseDrag, CardPutUp }
    enum ECardMode                      { DEFAULT, MYCARDS, PUTCARDS }
    ECardState                          cardState;

    string                              deckName;
    List<string>                        cardBuffer = new List<string>();
    ReactiveProperty<int>               cardBufferNum = new ReactiveProperty<int>();
    ReactiveProperty<int>               createdSpiceNum = new ReactiveProperty<int>();
    ReactiveProperty<int>               consumedSpiceNum = new ReactiveProperty<int>();
    ReactiveProperty<int>               cardPutCount = new ReactiveProperty<int>();
    List<CardScript>                    myCards = new List<CardScript>();
    List<CardScript>                    putCards = new List<CardScript>();
    CardScript                          draggingCard;
    bool                                cardDragging;
    bool                                onCardArea;
    bool                                onCardPutArea;
    bool                                onPutUpCardArea;
    bool                                onGridObjectArea;
    bool                                onUIArea;
    bool                                cardPuttingUp;
    float                               oneCardPutWidth;
    float                               oneCardPutX;
    //int                                 cardPutCount;

    public static SO_CardItemScript     CardItemSO => Inst.SO_cardItem;
    public static CardScript            EmptyCard => Inst.emptyCard;
    public static string                DeckName { get => Inst.deckName; set => Inst.deckName = value; }
    public static List<string>          CardBuffer => Inst.cardBuffer;
    public static List<CardScript>      PutCards => Inst.putCards;
    public static bool                  OnCardArea { set => Inst.onCardArea = value; }
    public static float                 OneCardPutWidth => Inst.oneCardPutWidth;
    public static float                 OneCardPutX => Inst.oneCardPutX;

    private void Start() {
        cardPutCount.Value = CardGameMngScript.StartPutCardCount;
        this.UpdateAsObservable()
            .Subscribe(_ => {
            DetectCardArea();
            SetCardState();
            cardBufferNum.Value = cardBuffer.Count;
        });
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => {
                if (cardState == ECardState.CardPutUp && !onUIArea) {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
                    int layer = LayerMask.NameToLayer("Card");
                    if (!Array.Exists(hits, x => x.collider.gameObject.layer == layer))
                        StartCoroutine(PutDownCard());
                }
                else if (!onCardArea && !onCardPutArea && !onGridObjectArea && !onUIArea)
                    CardGameMngScript.CardExplainPanel.ScaleZero();
            });
        this.UpdateAsObservable()
            .Where(_ => cardDragging)
            .Subscribe(_ => DragCard());
        cardBufferNum
            .Subscribe(_ => UpdateCardBufferNumText());
        createdSpiceNum
            .Subscribe(_ => UpdateSpiceNumText());
        consumedSpiceNum
            .Subscribe(_ => UpdateSpiceNumText());
        cardPutCount
            .Subscribe(_ => UpdateCardPutCountText());
    }

    public static void AddCreatedSpiceNum(int _num) => Inst.createdSpiceNum.Value += _num;
    public static void AddConsumedSpiceNum(int _num) => Inst.consumedSpiceNum.Value += _num;
    void UpdateCardBufferNumText() => cardBufferNumText.text = cardBufferNum.ToString();
    void UpdateSpiceNumText() => spiceNumText.text = consumedSpiceNum + "/" + createdSpiceNum;
    void UpdateCardPutCountText() => cardPutCountText.text = cardPutCount + "/" + CardGameMngScript.MaxPutCardCount;

    #region 덱 종류 설정
    public static void Init(int _stageNum) {
        switch (_stageNum) {
            case 0:
                Inst.deckName = "밥";
                break;
            case 1:
                Inst.deckName = "밥";
                break;
            case 2:
                Inst.deckName = "밥";
                break;
            case 3:
                Inst.deckName = "맛있는 밥";
                break;
            case 4:
                Inst.deckName = "맛있는 밥";
                break;
            case 5:
                Inst.deckName = "맛있는 밥";
                break;
            case 6:
                Inst.deckName = "소고기 덮밥 기본";
                break;
            case 7:
                Inst.deckName = "소고기 덮밥 기본";
                break;
            case 8:
                Inst.deckName = "소고기 덮밥 기본";
                break;
            case 9:
                Inst.deckName = "소고기 덮밥 기본";
                break;
            case 10:
                Inst.deckName = "소고기 덮밥 기본";
                break;
            case 11:
                Inst.deckName = "소고기 덮밥 토핑 세트";
                break;
            case 12:
                Inst.deckName = "소고기 덮밥 토핑 세트";
                break;
            case 13:
                Inst.deckName = "소고기 덮밥 토핑 세트";
                break;
            case 14:
                Inst.deckName = "소고기 덮밥 토핑 세트";
                break;
            case 15:
                Inst.deckName = "소고기 덮밥 토핑 세트";
                break;
            case 16:
                Inst.deckName = "소고기 덮밥 레드와인";
                break;
            case 17:
                Inst.deckName = "소고기 덮밥 레드와인";
                break;
            case 18:
                Inst.deckName = "소고기 덮밥 레드와인";
                break;
            case 19:
                Inst.deckName = "소고기 덮밥 레드와인";
                break;
            case 20:
                Inst.deckName = "소고기 덮밥 레드와인";
                break;
        }
        Inst.SetupCardItemSO();
        Inst.SetupCardBuffer();
        InitCardPutArea(CardGameMngScript.StartPutCardCount);
    }
    #endregion

    public static CardItem GetCardItem(string _cardName) {
        return Inst.SO_cardItem.GetCardItem(_cardName);
    }

    public static void AddCardItem() {
        if (Inst.myCards.Count == CardGameMngScript.MaxCardCount)
            return;

        var cardClone = Instantiate(Inst.cardPrefab, Inst.cardSpawnPoint.position, Quaternion.identity);
        var cardItem = cardClone.GetComponent<CardScript>();
        cardItem.Setup(CardItemSO.GetLocalizedCardName(Inst.PopCard()));
        Inst.myCards.Add(cardItem);
        SoundMngScript.PlaySFX(3);

        Inst.SetCardOriginOrders(ECardMode.MYCARDS);
        Inst.AlignCards(Utils.cardDrawDotweenTime, ECardMode.MYCARDS);
    }

    public static void InsertCardItem(string _cardName, int _index) {
        if (_index >= Inst.cardBuffer.Count)
            return;

        Inst.cardBuffer.Insert(_index, _cardName);
	}

    #region Card & Mouse
    public static void CardMouseDown(CardScript _card) {
        if (Inst.cardState == ECardState.Nothing)
            return;
        else if (Inst.cardState == ECardState.CardPutUp && (Inst.onCardArea || Inst.onCardPutArea))
            Inst.StartCoroutine(Inst.PutDownCard());
        else if (Inst.cardState == ECardState.CardPutUp) {
            Inst.draggingCard = _card;
            Inst.cardDragging = true;
        }
        else if (Inst.cardState == ECardState.CanMouseDrag) {
            Inst.draggingCard = _card;
            Inst.cardDragging = true;
            string explainText = GetCardItem(_card.CardName).explain;
            CardGameMngScript.CardExplainPanel.Show(explainText);
        }
    }

    public static void CardMouseUp() {
        if (Inst.cardState == ECardState.Nothing)
            return;
        else if (Inst.cardState == ECardState.CanMouseDrag && (!Inst.onCardArea && !Inst.onCardPutArea))
            Inst.StartCoroutine(Inst.PutUpCard());
        else if (Inst.cardState == ECardState.CardPutUp && (!Inst.onCardArea && !Inst.onCardPutArea)) {
            if (CardGameMngScript.StageNum >= 3)
                Inst.StartCoroutine(Inst.SwapCard());
        }
        else if (Inst.cardDragging) {
            CardGameMngScript.CardExplainPanel.ScaleZero();
            Inst.StartCoroutine(Inst.PutDownCard());
        }
        Inst.cardDragging = false;
    }
    #endregion

    public static void InitCardPutArea(int _startCount) {
        Inst.oneCardPutWidth = Inst.cardPutCover.transform.localScale.x / CardGameMngScript.MaxPutCardCount;
        Inst.oneCardPutX = (Inst.cardPutRightLine.position - Inst.cardPutLeftLine.position).x / CardGameMngScript.MaxPutCardCount / 2;
        Inst.cardPutCover.transform.localScale -= Vector3.right * Inst.oneCardPutWidth * _startCount;
        Inst.cardPutCover.transform.localPosition += Vector3.right * Inst.oneCardPutX * _startCount;
        Inst.cardPutRightLine.position -= Vector3.right * Inst.oneCardPutX * (CardGameMngScript.MaxPutCardCount - _startCount) * 2;
    }

    public static void IncreaseCardPutCount() {
        if (Inst.cardPutCount.Value == CardGameMngScript.MaxPutCardCount)
            return;

        Inst.cardPutCount.Value++;
        Inst.cardPutRightLine.position += Vector3.right * Inst.oneCardPutX * 2;
    }

    public static int GetPlacablePutCardNum() {
        int cardNum = 0;
        foreach (var card in PutCards) {
            if (card != Inst.emptyCard && CardItemSO.GetCardItem(card.CardName).type != SO_CardType.Placable)
                continue;
            cardNum++;
        }
        return cardNum;
    }

    public static IEnumerator ExecuteCardsCo() {
        List<CardScript> putCards = PutCards;
        CardScript card;
        
        for (int i = 0; i < putCards.Count; i++) {
            card = putCards[i];
            if (card.IsDotori && !card.IsFront)
                CardItemSO.ExecuteCardFunc(card.CardName, SO_Dotori.DotoriBackSide);
            else if (!card.IsFront)
                CardItemSO.ExecuteCardFunc(card.CardName, SO_Dotori.DotoriSide);
            else
                CardItemSO.ExecuteCardFunc(card.CardName, SO_Dotori.Default);
            Vector3 pos = new Vector3(Inst.cardExecuteEndingPoint.position.x, card.transform.position.y, card.transform.position.z);
            PRS prs = new PRS(pos, Quaternion.identity, Utils.cardScale);
            card.MoveTransform(prs, Utils.cardPutUpDownDotweenTime, Ease.OutQuad);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
            Destroy(card.gameObject);
        }

        putCards.Clear();
        CardGameMngScript.IsCoroutine[0] = false;
    }

    void SetupCardItemSO() {
        SO_cardItem.SetUp(deckName);
        GridObjectMngScript.GridObjectSO.SetUp(deckName);
    }

    #region 덱 구성 설정
    void SetupCardBuffer() {
        if (deckName == "밥") {
            cardBuffer = new List<string>(6);
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
        }
        if (deckName == "맛있는 밥") {
            cardBuffer = new List<string>(9);
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
        }
        else if (deckName == "소고기 덮밥 기본") {
            cardBuffer = new List<string>();
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("냄비");
            cardBuffer.Add("냄비");
            cardBuffer.Add("양파");
            cardBuffer.Add("양파");
            cardBuffer.Add("우삼겹");
            cardBuffer.Add("우삼겹");
        }
        else if (deckName == "소고기 덮밥 토핑 세트") {
            cardBuffer = new List<string>();
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("냄비");
            cardBuffer.Add("냄비");
            cardBuffer.Add("양파");
            cardBuffer.Add("양파");
            cardBuffer.Add("우삼겹");
            cardBuffer.Add("우삼겹");
            cardBuffer.Add("토핑 추가");
            cardBuffer.Add("토핑 추가");
            cardBuffer.Add("토핑 추가");
        }
        else if (deckName == "소고기 덮밥 레드와인") {
            cardBuffer = new List<string>(28);
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("냄비");
            cardBuffer.Add("냄비");
            cardBuffer.Add("양파");
            cardBuffer.Add("양파");
            cardBuffer.Add("우삼겹");
            cardBuffer.Add("우삼겹");
            cardBuffer.Add("오크통 배치");
            cardBuffer.Add("오크통 배치");
            cardBuffer.Add("오크통 배치");
            cardBuffer.Add("오크통 배치");
            cardBuffer.Add("포도");
            cardBuffer.Add("포도");
        }

        for (int i = 0; i < cardBuffer.Count; i++) {
            int rand = Random.Range(i, cardBuffer.Count);
            string tmp = cardBuffer[i];
            cardBuffer[i] = cardBuffer[rand];
            cardBuffer[rand] = tmp;
        }
    }
    #endregion

    string PopCard() {
        if (cardBuffer.Count == 0)
            SetupCardBuffer();

        string cardName = cardBuffer[0];
        cardBuffer.RemoveAt(0);

        return cardName;
    }

    void SetCardOriginOrders(ECardMode _mode) {
        List<CardScript> target = new List<CardScript>();
        switch (_mode) {
            case ECardMode.MYCARDS:
                target = myCards;
                break;
            case ECardMode.PUTCARDS:
                target = putCards;
                break;
        }

        for (int i = 0; i < target.Count; i++) {
            var targetCard = target[i];
            targetCard?.GetComponent<CardOrderScript>().SetOriginOrder(i);
        }
    }

    void AlignCards(float _dotween, ECardMode _mode, bool _move = true) {
        List<PRS>           originPRSs = new List<PRS>();
        List<CardScript>    targetCards = new List<CardScript>();

        switch(_mode) {
            case ECardMode.MYCARDS:
                targetCards = myCards;
                originPRSs = GetCardOriginPRSs(targetCards.Count, Utils.cardScale, ECardMode.MYCARDS);
                break;
            case ECardMode.PUTCARDS:
                targetCards = putCards;
                originPRSs = GetCardOriginPRSs(targetCards.Count, Utils.cardScale, ECardMode.PUTCARDS);
                break;
        }

        for (int i = 0; i < targetCards.Count; i++) {
            var targetCard = targetCards[i];
            targetCard.OriginPRS = originPRSs[i];

            if (!targetCard.IsFront && putCards.Contains(targetCard))
                targetCard.OriginPRS.rot = Quaternion.AngleAxis(180, Vector3.up);

            if (_move)
                targetCard.MoveTransform(targetCard.OriginPRS, _dotween, Ease.Linear);  
            else
                targetCard.MoveTransform(targetCard.OriginPRS);
        }
    }

    List<PRS> GetCardOriginPRSs(int _objCount, Vector3 _scale, ECardMode _mode) {
        float[] objLerps = new float[_objCount];
        List<PRS> result = new List<PRS>(_objCount);
        Transform topLine = null;
        Transform bottomLine = null;

        switch (_mode) {
            case ECardMode.MYCARDS:
                topLine = cardLeftLine;
                bottomLine = cardRightLine;

                switch (_objCount) {
                    case 1:
                        objLerps[0] = 0.5f;
                        break;
                    case 2:
                        objLerps[0] = 0.27f;
                        objLerps[1] = 0.73f;
                        break;
                    case 3:
                        objLerps[0] = 0.1f;
                        objLerps[1] = 0.5f;
                        objLerps[2] = 0.9f;
                        break;
                    default:
                        float interval = 1.0f / (_objCount - 1);
                        for (int i = 0; i < _objCount; i++)
                            objLerps[i] = interval * i;
                        break;
                }
                break;
            case ECardMode.PUTCARDS:
                bottomLine = cardPutLeftLine;
                topLine = cardPutRightLine;

                float interval2 = (1.0f - 0.1f) / CardGameMngScript.MaxPutCardCount;
                for (int i = 0; i < _objCount; i++)
                    objLerps[i] = 0.1f + interval2 * i;
                break;
        }

        for (int i = 0; i < _objCount; i++) {
            var originPos = Vector3.Lerp(topLine.position, bottomLine.position, objLerps[i]);
            var originRot = Quaternion.identity;
            result.Add(new PRS(originPos, originRot, Utils.cardScale));
        }

        return result;
    }

    void DragCard() {
        bool flag = false;

        if (cardState == ECardState.CanMouseDrag)
            flag = true;
        else if (cardState == ECardState.CardPutUp && !onPutUpCardArea) {
            flag = true;
            cardPuttingUp = false;
            string explainText = GetCardItem(draggingCard.CardName).explain;
            CardGameMngScript.CardExplainPanel.Show(explainText);
        }

        if (flag) {
            if (draggingCard.IsPut && putCards.Contains(draggingCard))
                putCards.Remove(draggingCard);
            else if (!draggingCard.IsPut && myCards.Contains(draggingCard))
                myCards.Remove(draggingCard);

            draggingCard.GetComponent<CardOrderScript>().SetMostFrontOrder(true);
            if (draggingCard.IsFront)
                draggingCard.MoveTransform(new PRS(Utils.MousePos, Quaternion.identity, Utils.cardScale * Utils.cardDragFloat));
            else
                draggingCard.MoveTransform(new PRS(Utils.MousePos, Quaternion.AngleAxis(180, Vector3.up), Utils.cardScale * Utils.cardDragFloat));

            if (onCardArea && myCards.Count < CardGameMngScript.MaxCardCount)
                InsertEmptyCard(Utils.MousePos.x, ECardMode.MYCARDS);
            else if (onCardPutArea && putCards.Count < cardPutCount.Value) {
                draggingCard.ChangeToPutCard();
                InsertEmptyCard(Utils.MousePos.y, ECardMode.PUTCARDS);
            }
            else if (!onCardArea && myCards.Contains(emptyCard)) {
                myCards.Remove(emptyCard);
                AlignCards(Utils.cardAlignmentDotweenTime, ECardMode.MYCARDS);
            }
            else if (!onCardPutArea && putCards.Contains(emptyCard)) {
                putCards.Remove(emptyCard);
                draggingCard.ChangeToCard();
                AlignCards(Utils.cardAlignmentDotweenTime, ECardMode.PUTCARDS, false);
            }
        }
    }

    void DetectCardArea() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);

        int layer = LayerMask.NameToLayer("Card Area");
        onCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);

        layer = LayerMask.NameToLayer("Card Put Area");
        onCardPutArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer &&
            (cardPutLeftLine.position.y <= Utils.MousePos.y && Utils.MousePos.y <= cardPutRightLine.position.y));

        if (cardState == ECardState.CardPutUp) {
            layer = LayerMask.NameToLayer("Card");
            onPutUpCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer && x.collider.GetComponent<CardScript>() == draggingCard);
        }

        layer = LayerMask.NameToLayer("Grid Object Area");
        onGridObjectArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);

        layer = LayerMask.NameToLayer("UI");
        onUIArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }

    void SetCardState() {
        if (CardGameMngScript.Inst.isLoading)
            cardState = ECardState.Nothing;
        else {
            if (!CardGameMngScript.MyTurn)
                cardState = ECardState.OnlyMouseClick;
            else if (cardPuttingUp)
                cardState = ECardState.CardPutUp;
            else if (CardGameMngScript.MyTurn)
                cardState = ECardState.CanMouseDrag;
        }
    }

    void InsertEmptyCard(float _x, ECardMode _mode) {
        List<CardScript> cards = null;
        int maxCardCount = -1;

        switch(_mode) {
            case ECardMode.MYCARDS:
                cards = myCards;
                maxCardCount = CardGameMngScript.MaxCardCount;
                break;
            case ECardMode.PUTCARDS:
                cards = putCards;
                maxCardCount = cardPutCount.Value;
                break;
        }

        int     tmpIndex = GetEmptyCardIndex(_mode);
        Vector3 tmp;
        if (!cards.Contains(emptyCard))
            cards.Add(emptyCard);
        tmp = emptyCard.transform.position;
        switch(_mode) {
            case ECardMode.MYCARDS:
                tmp.x = _x;
                break;
            case ECardMode.PUTCARDS:
                tmp.y = _x;
                break;
        }
        emptyCard.transform.position = tmp;
        switch(_mode) {
            case ECardMode.MYCARDS:
                cards.Sort((card1, card2) => card1.transform.position.x.CompareTo(card2.transform.position.x));
                break;
            case ECardMode.PUTCARDS:
                cards.Sort((card1, card2) => card2.transform.position.y.CompareTo(card1.transform.position.y));
                break;
        }
        if (GetEmptyCardIndex(_mode) == tmpIndex)
            return;
        switch(_mode) {
            case ECardMode.MYCARDS:
                AlignCards(Utils.cardAlignmentDotweenTime, _mode);
                break;
            case ECardMode.PUTCARDS:
                AlignCards(Utils.cardAlignmentDotweenTime, _mode, false);
                break;
        }
    }

    int GetEmptyCardIndex(ECardMode _mode) {
        switch (_mode) {
            case ECardMode.MYCARDS:
                return myCards.FindIndex(x => x == emptyCard);
            case ECardMode.PUTCARDS:
                return putCards.FindIndex(x => x == emptyCard);
            default:
                return -1;
        }
    }

    IEnumerator PutUpCard() {
        CardGameMngScript.Inst.isLoading = true;

        CardGameMngScript.CardExplainPanel.ScaleZero();

        PRS putUpPRS;
        if (draggingCard.IsFront)
            putUpPRS = new PRS(cardPutUpPoint.position, Quaternion.identity, Utils.cardScale * Utils.cardPutUpFloat);
        else
            putUpPRS = new PRS(cardPutUpPoint.position, Quaternion.AngleAxis(180, Vector3.up), Utils.cardScale * Utils.cardPutUpFloat);
        draggingCard.MoveTransform(putUpPRS, Utils.cardPutUpDownDotweenTime, Ease.OutQuad);

        yield return new WaitForSeconds(Utils.cardPutUpDownDotweenTime);

        cardPuttingUp = true;

        CardGameMngScript.Inst.isLoading = false;
    }

    IEnumerator PutDownCard() {
        CardGameMngScript.Inst.isLoading = true;

        ECardMode mode = ECardMode.DEFAULT;

        if (cardState == ECardState.CanMouseDrag && onCardArea) {
            mode = ECardMode.MYCARDS;
            if (!myCards.Contains(emptyCard)) {
                StartCoroutine(PutUpCard());
                yield break;
            }
            draggingCard.IsPut = false;
            myCards[GetEmptyCardIndex(mode)] = draggingCard;
        }
        else if (cardState == ECardState.CanMouseDrag && onCardPutArea) {
            mode = ECardMode.PUTCARDS;
            if (!putCards.Contains(emptyCard)) {
                draggingCard.ChangeToCard();
                StartCoroutine(PutUpCard());
                yield break;
            }
            else if (draggingCard.IsFront == true && !SO_cardItem.GetUsable(draggingCard.CardName)) {
                putCards.Remove(emptyCard);
                draggingCard.ChangeToCard();
                AlignCards(Utils.cardAlignmentDotweenTime, ECardMode.PUTCARDS, false);
                StartCoroutine(PutUpCard());
                yield break;
            }
            draggingCard.IsPut = true;
            putCards[GetEmptyCardIndex(mode)] = draggingCard;
        }
        else if (cardState == ECardState.CardPutUp) {
            if (draggingCard.IsPut) {
                if (!SO_cardItem.GetUsable(draggingCard.CardName) && draggingCard.IsFront) {
                    mode = ECardMode.MYCARDS;
                    myCards.Add(draggingCard);
                }
                else {
                    mode = ECardMode.PUTCARDS;
                    putCards.Add(draggingCard);
                    draggingCard.ChangeToPutCard();
                }
            }
            else {
                mode = ECardMode.MYCARDS;
                myCards.Add(draggingCard);
            }
		}

        SetCardOriginOrders(mode);
        switch (mode) {
            case ECardMode.MYCARDS:
                AlignCards(Utils.cardPutUpDownDotweenTime, mode);
                break;
            case ECardMode.PUTCARDS:
                AlignCards(Utils.cardPutUpDownDotweenTime, mode, false);
                Quaternion  qua = draggingCard.IsFront ? Quaternion.identity : Quaternion.AngleAxis(180, Vector3.up);
                Vector3     tmp = draggingCard.transform.position;
                Vector3     pos = new Vector3(cardPutStartingPoint.position.x, tmp.y, tmp.z);
                PRS prs = new PRS(pos, qua, Utils.cardScale);
                draggingCard.MoveTransform(prs, 0, Ease.OutQuad);
                prs = new PRS(tmp, qua, Utils.cardScale);
                draggingCard.MoveTransform(prs, Utils.cardPutUpDownDotweenTime, Ease.OutQuad);
                break;
        }

        yield return new WaitForSeconds(Utils.cardPutUpDownDotweenTime / 2);

        if (mode == ECardMode.PUTCARDS)
            SoundMngScript.PlaySFX(2);
        if (!draggingCard.IsFront && myCards.Contains(draggingCard))
			draggingCard.Swap();

        yield return new WaitForSeconds(Utils.cardPutUpDownDotweenTime / 2);

        draggingCard = null;
        cardPuttingUp = false;

        CardGameMngScript.Inst.isLoading = false;
    }

    IEnumerator SwapCard() {
        CardGameMngScript.Inst.isLoading = true;

        PRS swapPRS = new PRS(draggingCard.transform.position, draggingCard.transform.rotation * Quaternion.AngleAxis(90, Vector3.up), Utils.cardScale * Utils.cardPutUpFloat);
        draggingCard.MoveTransform(swapPRS, Utils.cardSwapDotweenTime, Ease.Linear);
        SoundMngScript.PlaySFX(3);

        yield return new WaitForSeconds(Utils.cardSwapDotweenTime);

        draggingCard.Swap();
        swapPRS.rot = draggingCard.transform.rotation * Quaternion.AngleAxis(90, Vector3.up);
        draggingCard.MoveTransform(swapPRS, Utils.cardSwapDotweenTime, Ease.Linear);

        yield return new WaitForSeconds(Utils.cardSwapDotweenTime);

        CardGameMngScript.Inst.isLoading = false;
    }
}
