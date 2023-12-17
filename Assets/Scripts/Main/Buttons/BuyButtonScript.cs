using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButtonScript : SFXButtonScript {
    int         productIndex;
    int         price;

    public int  Price { get => price; set => price = value; }
    public int  ProductIndex { get => productIndex; set => productIndex = value; }

    public void BuyButton() {
        if (MainGameMngScript.DotoriNum.Value < price) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        MainGameMngScript.DotoriNum.Value -= price;
        CampingShopMngScript.Buy(productIndex);
    }
}
