using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ProductState {
    PS_UNBOUGHT,
    PS_BOUGHT
}

public class Product {
    public string           name;
    public string           explain;
    public string           bgmName;
    public Sprite           background;
    public int              price;
    public ProductState     state;
}

public class LocalizedProduct {
    public string           name;
    public string           explain;
}

public class CampingShopMngScript : MonoBehaviour {
    static public CampingShopMngScript      Inst { get; set; } = null;

    [Header("UI 파츠")]
    [SerializeField] BuyButtonScript[]      buyButtons;
    [SerializeField] Text                   localizeText;
    [SerializeField] TextMeshProUGUI[]      productNameTexts;
    [SerializeField] TextMeshProUGUI[]      productExplainTexts;
    [SerializeField] TextMeshProUGUI[]      productPriceTexts;
    [SerializeField] Image[]                boughtImages;

    [Header("배경 일러스트")]
    [SerializeField] Sprite[]               backgrounds;

    List<Product>                           productList = new List<Product>();
    List<LocalizedProduct>                  localizedPcoductListJP = new List<LocalizedProduct>();
    int                                     topProductIndex;

    private void Awake() => Inst = this;    
    void Start() {
        Init();
        gameObject.SetActive(false);
    }

    static public List<Product>             ProductList => Inst.productList;
    static public List<LocalizedProduct>    LocalizedProductJP => Inst.localizedPcoductListJP;

    public int TopProductIndex {
        get => Inst.topProductIndex;
        set {
            Inst.topProductIndex = value;
            for (int i = TopProductIndex; i < TopProductIndex + Inst.productNameTexts.Length; i++) {
                if (i >= Inst.productList.Count) {
                    Inst.buyButtons[i - TopProductIndex].GetComponent<Button>().interactable = false;
                    Inst.productNameTexts[i - TopProductIndex].text = "";
                    Inst.productExplainTexts[i - TopProductIndex].text = "";
                    Inst.productPriceTexts[i - TopProductIndex].text = "";
                    Inst.boughtImages[i - TopProductIndex].enabled = false;
                    continue;
                }
                switch (Inst.productList[i].state) {
                    case ProductState.PS_UNBOUGHT:
                        Inst.buyButtons[i - TopProductIndex].GetComponent<Button>().interactable = true;
                        Inst.buyButtons[i - TopProductIndex].Price = Inst.productList[i].price;
                        Inst.buyButtons[i - TopProductIndex].ProductIndex = i;
                        switch (LocalizeManager.Inst.currentLanguageIndex) {
                            case 1:
                                Inst.productNameTexts[i - TopProductIndex].text = Inst.productList[i].name;
                                Inst.productExplainTexts[i - TopProductIndex].text = Inst.productList[i].explain;
                                break;
                            case 2:
                                Inst.productNameTexts[i - TopProductIndex].text = Inst.localizedPcoductListJP[i].name;
                                Inst.productExplainTexts[i - TopProductIndex].text = Inst.localizedPcoductListJP[i].explain;
                                break;
                        }
                        Inst.productPriceTexts[i - TopProductIndex].text = localizeText.text + " : " + Inst.productList[i].price.ToString();
                        Inst.boughtImages[i - TopProductIndex].enabled = false;
                        break;
                    case ProductState.PS_BOUGHT:
                        Inst.buyButtons[i - TopProductIndex].GetComponent<Button>().interactable = false;
                        switch (LocalizeManager.Inst.currentLanguageIndex) {
                            case 1:
                                Inst.productNameTexts[i - TopProductIndex].text = Inst.productList[i].name;
                                Inst.productExplainTexts[i - TopProductIndex].text = Inst.productList[i].explain;
                                break;
                            case 2:
                                Inst.productNameTexts[i - TopProductIndex].text = Inst.localizedPcoductListJP[i].name;
                                Inst.productExplainTexts[i - TopProductIndex].text = Inst.localizedPcoductListJP[i].explain;
                                break;
                        }
                        Inst.productPriceTexts[i - TopProductIndex].text = localizeText.text + " : " + Inst.productList[i].price.ToString();
                        Inst.boughtImages[i - TopProductIndex].enabled = true;
                        break;
                }
            }
        }
    }

    static public void Buy(int _index) {
        Inst.productList[_index].state = ProductState.PS_BOUGHT;
        Inst.TopProductIndex = Inst.TopProductIndex;
    }

    void Init() {
        Product product = new Product();
        product.name = "소고기 덮밥 기본 베이스 - 모닥불 앞에서..";
        product.explain = "미니모리 숲길에 자리를 잡았다. 도토리로 지핀 모닥불이 따뜻하다.";
        product.background = backgrounds[0];
        product.price = 100;
        productList.Add(product);

        product = new Product();
        product.name = "소고기 덮밥 기본 업그레이드 1 - 저녁 식사 준비!";
        product.explain = "상자를 펼쳐 요리할 준비를 한다. 오늘의 메뉴는 소고기 덮밥이다.";
        product.background = backgrounds[1];
        product.price = 200;
        productList.Add(product);

        product = new Product();
        product.name = "소고기 덮밥 기본 업그레이드 2 - 도토리와 간장 향기";
        product.explain = "솥과 냄비가 끓는다. 뚜껑 사이로 맛있는 냄새가 새어나온다.";
        product.background = backgrounds[2];
        product.price = 200;
        productList.Add(product);

        product = new Product();
        product.name = "소고기 덮밥 기본 업그레이드 3-1 - 토핑은 정석대로!";
        product.explain = "소고기 덮밥을 그냥 먹기에는 심심하기에 토핑을 준비한다. 그렇지만 꺼내는 토핑의 종류는 항상 같다.";
        product.background = backgrounds[3];
        product.price = 300;
        productList.Add(product);

        LocalizedProduct localizedProduct = new LocalizedProduct();
        localizedProduct.name = "牛丼基本ベース - 焚き火の前で..";
        localizedProduct.explain = "ミニモリの森道に居場所を取った. どんぐりで焚いた焚き火が温かい.";
        localizedPcoductListJP.Add(localizedProduct);

        localizedProduct = new LocalizedProduct();
        localizedProduct.name = "牛丼基本Upgrade1 - 夕飯の準備!";
        localizedProduct.explain = "箱を展開し料理する準備をする. 今日のメニューは牛丼だ.";
        localizedPcoductListJP.Add(localizedProduct);

        localizedProduct = new LocalizedProduct();
        localizedProduct.name = "牛丼基本Upgrade2 - どんぐりと醤油の香り";
        localizedProduct.explain = "釜と鍋が煮ている. 蓋の間で美味しい匂いが湧き出す.";
        localizedPcoductListJP.Add(localizedProduct);

        localizedProduct = new LocalizedProduct();
        localizedProduct.name = "牛丼基本Upgrade3-1 - トッピングは定番どうり!";
        localizedProduct.explain = "牛丼をそのまま食べるには地味だからトッピングを用意する. だが, 出すトッピングの種類はいつも同じ.";
        localizedPcoductListJP.Add(localizedProduct);

        List<string>    inventoryList = SaveLoadMngScript.SaveData.itemNameList;
        int             inventoryIndex = 0;
        if (inventoryList.Count > 0) {
            foreach (var obj in productList) {
                if (obj.name != inventoryList[inventoryIndex])
                    continue;
                obj.state = ProductState.PS_BOUGHT;
                inventoryIndex++;
                if (inventoryIndex >= inventoryList.Count)
                    break;
            }
        }
        TopProductIndex = 0;
    }

    public void LocalizeRefresh() => Inst.TopProductIndex = Inst.TopProductIndex;

    public void GoToLeft() {
        if (TopProductIndex >= 3)
            TopProductIndex -= 3;
    }

    public void GoToRight() {
        if (TopProductIndex + 3 < productList.Count)
            TopProductIndex += 3;
    }
}
