using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridObjectScript : MonoBehaviour {
    [SerializeField] int[]          position = new int[2];
    [SerializeField] GameObject     usableSpellObject;
    [SerializeField] Animator       effectAnimator;     //// 별도의 오브젝트로 분리
    [SerializeField] Animator       objectAnimator;
    [SerializeField] Transform      objectPlateGroupTransform;
    [SerializeField] SpriteRenderer objectRenderer;
    [SerializeField] SpriteRenderer effectRenderer;
    [SerializeField] SpriteRenderer plateRenderer;
    [SerializeField] TextMeshPro    countDownText;

    string                          objectName;
    CurrentObjectItem               currentObjectItem;
    bool                            hasObject = false;
    bool                            isNewCooking = false;
    bool                            isDotori = false;
    bool                            isHeatPlate = false;
    int                             countDown = -1;

    public int[]                    Position => position;
    public CurrentObjectItem        CurrentObjectItem => currentObjectItem;
    public string                   ObjectName { get => objectName; set { objectName = value; } }
    public bool                     HasObject { get => hasObject; set { hasObject = value; } }
    public bool                     IsNewCooking { get => isNewCooking; set { isNewCooking = value; } }
    public bool                     IsDotori { get => isDotori; set { isDotori = value; } }

    private void Start() => objectAnimator.enabled = false;

    public int                      CountDown {
        get => countDown;
        set {
            countDown = value;
            if (countDown > 0)
                countDownText.text = countDown.ToString();
        }
    }

    private void OnMouseDown() {
        if (hasObject && !CardGameMngScript.Inst.isLoading) {
            ObjectItem item = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName);
            string text = "";
            text += GridObjectMngScript.GridObjectSO.GetLocalizedString(objectName) + "\n";
            if (item.tool != null) {
                if (item.tool.neededSpiceNames != null) {
                    text += LocalizeManager.Localize("Needed Spices") + " :\n";
                    foreach (var spiceName in item.tool.neededSpiceNames) {
                        if (currentObjectItem.currentSpiceNames.Contains(spiceName))
                            text += GridObjectMngScript.GridObjectSO.GetLocalizedString(spiceName) + "(O)" + "\n";
                        else
                            text += GridObjectMngScript.GridObjectSO.GetLocalizedString(spiceName) + "\n";
                    }
                }
                if (currentObjectItem.currentObjectNums.Count != 0) {
                    text += LocalizeManager.Localize("Needed Ingredients") + " :\n";
                    for (int i = 0; i < currentObjectItem.currentObjectNums.Count; i++)
                        text += $"{GridObjectMngScript.GridObjectSO.GetLocalizedString(item.tool.neededObjectNames[i])}({currentObjectItem.currentObjectNums[i]}/{item.tool.neededObjectNums[i]})\n";
                }
                if (currentObjectItem.currentSpellNames.Count != 0) {
                    text += LocalizeManager.Localize("Usable Spell Cards") + " :\n";
                    foreach (var spellName in currentObjectItem.currentSpellNames)
                        text += GridObjectMngScript.GridObjectSO.GetLocalizedString(spellName) + "\n";
                }
            }
            else if (countDown != -1)
                text += LocalizeManager.Localize("Remaining Countdowns") + " : " + countDown + "\n";
            text += "\n\n" + GridObjectMngScript.GridObjectSO.GetLocalizedString(item.explain);
            CardGameMngScript.CardExplainPanel.Show(text);
        }
    }

    public void UseSpell(string _spellName) {
        effectAnimator.SetTrigger(CardMngScript.CardItemSO.GetSpellAnimationKey(_spellName));
        usableSpellObject.SetActive(false);
    }
    public void StartCooking() {
        string nextName = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName).tool.nextObjectName;
        ObjectItem nextItem = GridObjectMngScript.GridObjectSO.GetObjectItem(nextName);
        if (nextItem.animationKey != null) {
            objectAnimator.enabled = true;
            objectAnimator.SetTrigger(nextItem.animationKey);
        }
        countDownText.enabled = true;
        objectName = nextName;
        currentObjectItem.currentSpellNames = null;
        isNewCooking = true;
        CountDown = nextItem.cooking.originCountDown;
        isDotori = nextItem.isDotori;
        GridObjectMngScript.GridObjectSO.ExecuteObjectFunc(objectName, SO_Timing.Start, position[0], position[1]);
        if (nextItem.cooking != null && nextItem.cooking.BGMindex != -1) {
            GridObjectMngScript.CookBGMStack++;
            SoundMngScript.PlayCookBGM(nextItem.cooking.BGMindex);
        }
    }

    public void EndCooking() {
        string      nextName = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName).cooking.nextObjectName;
        ObjectItem  nextItem = GridObjectMngScript.GridObjectSO.GetObjectItem(nextName);
        objectAnimator.enabled = false;
        if (nextName == null)
            StartCoroutine(RemoveObject());
        else {
            objectRenderer.sprite = nextItem.sprite;
            countDownText.enabled = false;
            objectName = nextName;
            countDown = -1;
            currentObjectItem = new CurrentObjectItem(objectName);
            GridObjectMngScript.GridObjectSO.ExecuteObjectFunc(objectName, SO_Timing.Start, position[0], position[1]);
        }
        GridObjectMngScript.CookBGMStack--;
        if (GridObjectMngScript.CookBGMStack == 0)
            SoundMngScript.StopCookBGM();
    }

    public void InputObject(string _objectName) {
        if (_objectName == "조미료")
            return;
        if (GridObjectMngScript.GridObjectSO.GetObjectItem(_objectName).isSpice)
            currentObjectItem.currentSpiceNames.Add(_objectName);
        else
            currentObjectItem.currentObjectNums[GridObjectMngScript.GridObjectSO.GetObjectItem(objectName).tool.neededObjectNames.IndexOf(_objectName)]++;
        if (currentObjectItem.CurrentSpellNameUpdate())
            usableSpellObject.SetActive(true);
    }

    public bool AdjacentObjectCheck(string _objectName) {
        foreach (var gridObject in GridObjectMngScript.GetAdjacentGridObjects(position)) {
            if (_objectName == gridObject.ObjectName)
                return true;
        }
        return false;
    }

    public IEnumerator SetObject(string _objectName) {
        bool        isHeatPlateTmp = isHeatPlate;
        ObjectItem  item = GridObjectMngScript.GridObjectSO.GetObjectItem(_objectName);

        if ((item.tool == null && !isHeatPlate) || item.tool.useHeatPlate == isHeatPlate) {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
            objectRenderer.enabled = false;
            plateRenderer.sprite = isHeatPlate ? GridObjectMngScript.GridPutSprite : GridObjectMngScript.GridHeatSprite;
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
        }
        else {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime);
            plateRenderer.sprite = isHeatPlateTmp ? GridObjectMngScript.GridPutSprite : GridObjectMngScript.GridHeatSprite;
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(180, Vector3.up), Vector3.one), true);
        }

        if (item.sprite != null) {
            objectRenderer.sprite = item.sprite;
            objectName = _objectName;
            if (item.cooking != null) {
                countDownText.enabled = true;
                CountDown = item.cooking.originCountDown;
            }
            else if (item.isDotori)
                isDotori = true;
            else
                isDotori = false;
            if (item.tool != null && item.tool.useHeatPlate)
                isHeatPlate = true;
            else
                isHeatPlate = false;
        }
        else {
            objectRenderer.sprite = CardMngScript.CardItemSO.GetCardItem(_objectName).sprite;
            objectName = _objectName;
            isDotori = true;
            isHeatPlate = false;
        }

        objectRenderer.transform.localPosition = new Vector3(0, 0.32f);
        objectRenderer.transform.localPosition += new Vector3(item.xPivot, 0);
        hasObject = true;
        if (currentObjectItem == null)
            currentObjectItem = new CurrentObjectItem(objectName);
        GridObjectMngScript.NextGridObjectUpdate();

        if ((item.tool == null && !isHeatPlate) || item.tool.useHeatPlate == isHeatPlateTmp) {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                               Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
            objectRenderer.enabled = true;
            plateRenderer.sprite = isHeatPlate ? GridObjectMngScript.GridHeatSprite : GridObjectMngScript.GridPutSprite;
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                               Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
        }
        else {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime);
        }

        SoundMngScript.PlaySFX(1);
        GridObjectMngScript.GridObjectSO.ExecuteObjectFunc(objectName, SO_Timing.Start, position[0], position[1]);
        if (objectName == "도토리 솥(준비)" && currentObjectItem.currentObjectNums[0] == 1)
            isDotori = false;
    }

    public IEnumerator RemoveObject() {
        bool isHeatPlateTmp = isHeatPlate;

        if (!isHeatPlate) {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
            objectRenderer.enabled = false;
            plateRenderer.sprite = GridObjectMngScript.GridHeatSprite;
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
        }
        else {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime);
            plateRenderer.sprite = GridObjectMngScript.GridPutSprite;
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(180, Vector3.up), Vector3.one), true);
        }

        if (countDown != -1) {
            countDownText.enabled = false;
            countDown = -1;
        }

        objectRenderer.sprite = null;
        objectName = null;
        hasObject = false;
        isDotori = false;
        isHeatPlate = false;
        currentObjectItem = null;
        GridObjectMngScript.NextGridObjectUpdate();

        if (!isHeatPlateTmp) {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                               Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
            objectRenderer.enabled = true;
            plateRenderer.sprite = isHeatPlate ? GridObjectMngScript.GridHeatSprite : GridObjectMngScript.GridPutSprite;
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                               Utils.cardExecDotweenTime / 2, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime / 2);
        }
        else {
            objectPlateGroupTransform.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime);
        }
    }
}
