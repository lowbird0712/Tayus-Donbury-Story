using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item {
    public string name;
    public string explain;
    public string bgmName;
    public Sprite background;
}

public class LinBoxMngScript : MonoBehaviour {
    static public LinBoxMngScript Inst { get; set; } = null;

    [SerializeField] Image[]                usingImages;
    [SerializeField] UseItemButtonScript[]  useItemButtons;
    [SerializeField] TextMeshProUGUI[]      itemNameTexts;
    [SerializeField] TextMeshProUGUI[]      itemExplainTexts;

    List<Item>                              itemList = new List<Item>();
    List<LocalizedProduct>                  localizedItemListJP = new List<LocalizedProduct>();
    int                                     topItemIndex;
    int                                     usingIndex;

    private void Awake() => Inst = this;
    void Start() => gameObject.SetActive(false);

    public int TopItemIndex {
        get => topItemIndex;
        set {
            topItemIndex = value;
            for (int i = topItemIndex; i < topItemIndex + itemNameTexts.Length; i++) {
                if (i >= itemList.Count) {
                    useItemButtons[i - topItemIndex].GetComponent<Button>().interactable = false;
                    itemNameTexts[i - topItemIndex].text = "";
                    itemExplainTexts[i - topItemIndex].text = "";
                    usingImages[i - topItemIndex].enabled = false;
                    continue;
                }
                useItemButtons[i - topItemIndex].GetComponent<Button>().interactable = true;
                useItemButtons[i - topItemIndex].ItemIndex = i;
                switch (LocalizeManager.Inst.currentLanguageIndex) {
                    case 1:
                        itemNameTexts[i - topItemIndex].text = Inst.itemList[i].name;
                        itemExplainTexts[i - topItemIndex].text = Inst.itemList[i].explain;
                        break;
                    case 2:
                        itemNameTexts[i - topItemIndex].text = Inst.localizedItemListJP[i].name;
                        itemExplainTexts[i - topItemIndex].text = Inst.localizedItemListJP[i].explain;
                        break;
                }
                if (Inst.itemList[i - topItemIndex].name == CampMngScript.BackGroundName
                    || Inst.localizedItemListJP[i - topItemIndex].name == CampMngScript.BackGroundName
                    || Inst.itemList[i - topItemIndex].name == CampMngScript.BGMName
                    || Inst.localizedItemListJP[i - topItemIndex].name == CampMngScript.BGMName)
                    usingImages[i - topItemIndex].enabled = true;
                else
                    usingImages[i - topItemIndex].enabled = false;
            }
        }
    }

    static public Item GetItem(int _index) => Inst.itemList[_index];

    static public void RefreshUsingImage(int _index, bool _using) {
        if (_using) {
            Inst.usingImages[Inst.usingIndex].enabled = false;
            Inst.usingIndex = _index - Inst.topItemIndex;
        }
        Inst.usingImages[Inst.usingIndex].enabled = _using;
    }


    static public List<string> GetItemNameList() {
        List<string> list = new List<string>();
        foreach (var item in Inst.itemList)
            list.Add(item.name);
        return list;
    }

    static public void Init() {
        List<Product>           products = CampingShopMngScript.ProductList;
        List<LocalizedProduct>  localizedProductsJP = CampingShopMngScript.LocalizedProductJP;
        Inst.itemList.Clear();
        for (int i = 0; i < products.Count; i++) {
            if (products[i].state != ProductState.PS_BOUGHT)
                continue;
            Item item = new Item();
            LocalizedProduct localizedItemJP = new LocalizedProduct();
            item.name = products[i].name;
            item.explain = products[i].explain;
            localizedItemJP.name = localizedProductsJP[i].name;
            localizedItemJP.explain = localizedProductsJP[i].explain;
            item.bgmName = products[i].bgmName;
            item.background = products[i].background;
            Inst.itemList.Add(item);
            Inst.localizedItemListJP.Add(localizedItemJP);
        }
        Inst.TopItemIndex = 0;
    }

    public void LocalizeRefresh() => Inst.TopItemIndex = Inst.TopItemIndex;

    public void GoToLeft() {
        if (topItemIndex >= 3)
            TopItemIndex -= 3;
    }

    public void GoToRight() {
        if (topItemIndex + 3 < itemList.Count)
            TopItemIndex += 3;
    }
}
