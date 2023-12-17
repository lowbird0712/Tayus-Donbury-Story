using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardPutCoverScript : DotweenMovingScript {
    public void RightSwipe() {
        Vector3 targetPos = transform.position + Vector3.right * CardMngScript.OneCardPutX;
        Vector3 targetScale = transform.localScale - Vector3.right * CardMngScript.OneCardPutWidth;
        PRS targetPRS = new PRS(targetPos, Quaternion.identity, targetScale);
        MoveTransform(targetPRS, Utils.cardPutCoverDotweenTime, Ease.InOutQuad);
    }
}
