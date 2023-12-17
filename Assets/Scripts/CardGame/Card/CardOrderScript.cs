using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardOrderScript : MonoBehaviour {
    static int                  mostFrontOrder = 100;

    [SerializeField] Renderer[] backRenderers;
    [SerializeField] Renderer[] middleRenderers;
    [SerializeField] string     sortingLayerName;

    int                         originOrder;

    public void SetSortingLayerName(string _layerName) => sortingLayerName = _layerName;
    public void SetOriginOrder() => SetOrder(originOrder);
    public void SetMostFrontOrder(bool _mostFront) => SetOrder(_mostFront ? mostFrontOrder : originOrder);

    public void SetOrder(int _order) {
        int mulOrder = _order * 10;

        foreach(var renderer in backRenderers) {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder;
        }

        foreach (var renderer in middleRenderers) {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder + 1;
            if (renderer.name == "Name") {
                foreach (TMP_SubMesh subMesh in renderer.GetComponentsInChildren<TMP_SubMesh>())
                    subMesh.renderer.sortingOrder = mulOrder + 1;
            }
        }
    }

    public void SetOriginOrder(int _originOrder) {
        originOrder = _originOrder;
        SetOrder(originOrder);
    }
}
