using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeScript : MonoBehaviour
{
    [SerializeField] bool   disabled;
    public string           textKey;

    void LocalizeChanged() {
        if (GetComponent<TextMeshProUGUI>())
            GetComponent<TextMeshProUGUI>().text = Localize(textKey);
        else
            GetComponent<Text>().text = Localize(textKey);
    }

    void Start()
    {
        LocalizeChanged();
        LocalizeManager.Inst.LocalizeChanged += LocalizeChanged;
        gameObject.SetActive(!disabled);
    }

    private void OnDestroy() => LocalizeManager.Inst.LocalizeChanged -= LocalizeChanged;

    string Localize(string _textKey) {
        int keyIndex = LocalizeManager.Inst.languageDatas[0].value.FindIndex(x => x.ToLower() == _textKey.ToLower());
        return LocalizeManager.Inst.languageDatas[LocalizeManager.Inst.currentLanguageIndex].value[keyIndex].Replace("\r", "");
    }
}
