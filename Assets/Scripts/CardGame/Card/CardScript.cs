using UnityEngine;
using TMPro;

public class CardScript : DotweenMovingScript {
    [SerializeField] SpriteRenderer cardRenderer;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer cardTypeRenderer;
    [SerializeField] SpriteRenderer dotoriRenderer;
    [SerializeField] TMP_Text       nameTMP;
    [SerializeField] TMP_Text       neededDotoriNumTMP;
    [SerializeField] Sprite         cardFrontSprite;
    [SerializeField] Sprite         cardBackSprite;
    [SerializeField] Sprite         cardPutSprite;
    [SerializeField] Sprite         cardPutBackSprite;
    [SerializeField] Sprite         cardTypeNormalSprite;
    [SerializeField] Sprite         cardTypeDotoriSprite;
    [SerializeField] Sprite         dotoriSprite;
    [SerializeField] GameObject     notUsableImage;

    string                          cardName;
    string                          tmpCardName;
    int                             selectedGridNum;
    int                             neededDotoriNum;
    bool                            isFront = true;
    bool                            isDotori = false;
    bool                            isPut;
    PRS                             originPRS;

    public string                   CardName => cardName;
    public int                      SelectedGridNum { get => selectedGridNum; set { selectedGridNum = value; } }
    public bool                     IsFront => isFront;
    public bool                     IsDotori => isDotori;
    public bool                     IsPut { get => isPut; set { isPut = value; } }
    public PRS                      OriginPRS { get => originPRS; set => originPRS = value; }

    #region Card & Mouse
    private void OnMouseDown() => CardMngScript.CardMouseDown(this);

    private void OnMouseUpAsButton() => CardMngScript.CardMouseUp();
    #endregion

    public void Setup(string _cardName) {
        cardName = _cardName;
        CardItem cardItem = CardMngScript.GetCardItem(cardName);
        cardRenderer.sprite = cardFrontSprite;
        spriteRenderer.sprite = cardItem.sprite;
        nameTMP.text = cardName;
        neededDotoriNum = cardItem.neededDotoriNum;
        neededDotoriNumTMP.text = neededDotoriNum.ToString();
        if (cardItem.isDotori) {
            isDotori = true;
            cardTypeRenderer.sprite = cardTypeDotoriSprite;
        }
        else
            cardTypeRenderer.sprite = cardTypeNormalSprite;
    }

    public void Swap() {
        if (isFront) {
            cardRenderer.sprite = cardBackSprite;
            spriteRenderer.sprite = null;
            cardTypeRenderer.sprite = null;
            dotoriRenderer.sprite = null;
            nameTMP.text = "";
            neededDotoriNumTMP.text = "";
            isFront = false;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else {
            var cardItem = CardMngScript.GetCardItem(cardName);
            cardRenderer.sprite = cardFrontSprite;
            spriteRenderer.sprite = cardItem.sprite;
            cardTypeRenderer.sprite = isDotori ? cardTypeDotoriSprite : cardTypeNormalSprite;
            dotoriRenderer.sprite = dotoriSprite;
            nameTMP.text = cardName;
            neededDotoriNumTMP.text = neededDotoriNum.ToString();
            isFront = true;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public void ChangeToPutCard() {
        cardRenderer.sprite = isFront ? cardPutSprite : cardPutBackSprite;
        gameObject.GetComponent<BoxCollider2D>().size = cardRenderer.sprite.bounds.size;
        gameObject.GetComponent<CardOrderScript>().SetSortingLayerName("Default");
        spriteRenderer.transform.localPosition = new Vector3(0.4f, 0.15f, 0);
        spriteRenderer.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
        spriteRenderer.transform.localScale = new Vector3(0.9f, 0.9f, 0);
        cardTypeRenderer.enabled = false;
        dotoriRenderer.enabled = false;
        nameTMP.enabled = false;
        neededDotoriNumTMP.enabled = false;
        if (!CardMngScript.HasPutCardSpace())
            notUsableImage.SetActive(true);
    }

    public void ChangeToCard() {
        cardRenderer.sprite = isFront ? cardFrontSprite : cardBackSprite;
        gameObject.GetComponent<BoxCollider2D>().size = cardRenderer.sprite.bounds.size;
        gameObject.GetComponent<CardOrderScript>().SetSortingLayerName("OnFloor");
        spriteRenderer.transform.localPosition = new Vector3(0, 0.5f, 0);
        spriteRenderer.transform.localRotation = Quaternion.identity;
        spriteRenderer.transform.localScale = new Vector3(1.5f, 1.5f, 0);
        cardTypeRenderer.enabled = true;
        dotoriRenderer.enabled = true;
        nameTMP.enabled = true;
        neededDotoriNumTMP.enabled = true;
        notUsableImage.SetActive(false);
    }
}
