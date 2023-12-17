using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemButtonScript : SFXButtonScript {
    int itemIndex;

    public int ItemIndex { get => itemIndex; set => itemIndex = value; }

    public void UseItem() {
        Item item = LinBoxMngScript.GetItem(itemIndex);
        bool isUsing = CampMngScript.UseItem(item);
        LinBoxMngScript.RefreshUsingImage(itemIndex, isUsing);
    }
}
