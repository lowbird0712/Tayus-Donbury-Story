using UnityEngine;
using TMPro;

public class PanelScript : MonoBehaviour {
    [SerializeField] protected TMP_Text         textTMP;
    [SerializeField] protected TextMeshProUGUI  text;

    void Start() => ScaleZero();

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;

    [ContextMenu("ScaleOne")]
    public void ScaleOne() => transform.localScale = Vector3.one;

    virtual public void Show() {
        ScaleOne();
    }

    virtual public void Show(string _msg) {
        if (textTMP != null)
            textTMP.text = _msg;
        else
            text.text = _msg;
        ScaleOne();
    }
}
