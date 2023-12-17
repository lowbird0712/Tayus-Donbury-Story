using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurnStartPanelScript : PanelScript {
    override public void Show() {
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, Utils.turnStartPanelUpDownDotweenTime)).SetEase(Ease.InOutQuad)
            .AppendInterval(Utils.turnStartPanelAppendDotweenTIme)
            .Append(transform.DOScale(Vector3.zero, Utils.turnStartPanelUpDownDotweenTime)).SetEase(Ease.InOutQuad);
    }

    public void Show(int _additionalInterval) {
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, Utils.turnStartPanelUpDownDotweenTime)).SetEase(Ease.InOutQuad)
            .AppendInterval(Utils.turnStartPanelAppendDotweenTIme + _additionalInterval)
            .Append(transform.DOScale(Vector3.zero, Utils.turnStartPanelUpDownDotweenTime)).SetEase(Ease.InOutQuad);
    }
}
