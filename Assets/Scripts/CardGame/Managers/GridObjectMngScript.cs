using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectMngScript : MonoBehaviour {
    public static GridObjectMngScript                   Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] SO_GridObjectScript                SO_GridObject;
    [SerializeField] List<GridObjectScript>             gridObjects;
    [SerializeField] Sprite                             gridPutSprite;
    [SerializeField] Sprite                             gridHeatSprite;

    GridObjectScript                                    nextGridObject;
    int                                                 nextGridObjectIndex = 0;
    int                                                 cookBGMStack = 0;

    public static SO_GridObjectScript                   GridObjectSO => Inst.SO_GridObject;
    public static List<GridObjectScript>                GridObjects => Inst.gridObjects;
    public static Sprite                                GridPutSprite => Inst.gridPutSprite;
    public static Sprite                                GridHeatSprite => Inst.gridHeatSprite;
    public static GridObjectScript                      NextGridObject => Inst.nextGridObject;
    public static int                                   NextGridObjectIndex => Inst.nextGridObjectIndex;
    public static int                                   CookBGMStack { get => Inst.cookBGMStack; set => Inst.cookBGMStack = value; }

    void Start() => nextGridObject = gridObjects[nextGridObjectIndex];

    public static void                                  StartCooking(int _x, int _y) => Inst.gridObjects[4 * _x + _y].StartCooking();
    public static void                                  Spell(string _spellName, int _x, int _y) => Inst.gridObjects[4 * _x + _y].UseSpell(_spellName);
    public static void                                  PlaceObject(string _objectName, int _x, int _y) => Inst.StartCoroutine(Inst.gridObjects[4 * _x + _y].SetObject(_objectName));
    public static List<GridObjectScript>                GetAdjacentGridObjects() => GetAdjacentGridObjects(NextGridObject.Position[0], NextGridObject.Position[1]);
    public static List<GridObjectScript>                GetAdjacentGridObjects(int[] _position) => GetAdjacentGridObjects(_position[0], _position[1]);

    void InputObject(int[] _objPosition, int[] _toolPosition) {
        GridObjectScript obj = gridObjects[4 * _objPosition[0] + _objPosition[1]];
        GridObjectScript tool = gridObjects[4 * _toolPosition[0] + _toolPosition[1]];
        string toolName = tool.ObjectName;
        StartCoroutine(obj.RemoveObject());
        StartCoroutine(tool.SetObject(toolName));
        tool.InputObject(obj.ObjectName);
    }

    void MakeDonbury(GridObjectScript _riceGrid, GridObjectScript _donburyGrid, string _menu) {
        StartCoroutine(_riceGrid.RemoveObject());
        if (_donburyGrid != null)
            StartCoroutine(_donburyGrid.RemoveObject());
        CardGameMngScript.CurrentStageInfo[_menu]++;
    }

    string DonburyCheck(ref GridObjectScript _donburyGrid) {
        if (CardGameMngScript.StageNum <= 2)
            return "";
        else if (CardGameMngScript.StageNum <= 5)
            return "";

        List<string> menus = new List<string>(CardGameMngScript.StageInfo.Keys);
        string objectName = null;
        foreach (var gridObject in gridObjects) {
            objectName = gridObject.ObjectName;
            if (menus.Contains(objectName) && CardGameMngScript.StageInfo[objectName] > CardGameMngScript.CurrentStageInfo[objectName]) {
                _donburyGrid = gridObject;
                return objectName;
            }
        }
        return null;
    }

    public static List<GridObjectScript> GetAdjacentGridObjects(int _x, int _y) {
        List<GridObjectScript> objects = new List<GridObjectScript>();
        if (_y > 0)
            objects.Add(Inst.gridObjects[4 * _x + (_y - 1)]);
        if (_x > 0)
            objects.Add(Inst.gridObjects[4 * (_x - 1) + _y]);
        if (_y < 3)
            objects.Add(Inst.gridObjects[4 * _x + (_y + 1)]);
        if (_x < 3)
            objects.Add(Inst.gridObjects[4 * (_x + 1) + _y]);

        return objects;
    }

    public static void NextGridObjectUpdate() {
        for (int i = 0; i < Inst.gridObjects.Count; i++) {
            if (!Inst.gridObjects[i].HasObject) {
                Inst.nextGridObjectIndex = i;
                Inst.nextGridObject = Inst.gridObjects[i];
                return;
            }
        }
        Inst.nextGridObjectIndex = -1;
        Inst.nextGridObject = null;
    }

    public static string GetRandomNeededSpice() {
        foreach (var gridObject in GetAdjacentGridObjects()) {
            if (!gridObject.HasObject)
                continue;
            ObjectItem objectItem = GridObjectSO.GetObjectItem(gridObject.ObjectName);
            if (objectItem.tool == null)
                continue;
            if (objectItem.tool.neededSpiceNames == null)
                continue;
            if (objectItem.tool.neededSpiceNames.Count != gridObject.CurrentObjectItem.currentSpiceNames.Count) {
                int index = -1;
                string spice = null;
                do {
                    index = Random.Range(0, objectItem.tool.neededSpiceNames.Count);
                    spice = objectItem.tool.neededSpiceNames[index];
                    if (gridObject.AdjacentObjectCheck(spice)) {
                        spice = null;
                        if (objectItem.tool.neededSpiceNames.Count == gridObject.CurrentObjectItem.currentSpiceNames.Count + 1)
                            return "조미료";
                    }
                } while (gridObject.CurrentObjectItem.currentSpiceNames.Contains(spice) || spice == null);
                return spice;
            }
        }
        return "조미료";
    }

    public static int GetDotoriObjectNum() {
        int dotoriNum = 0;
        foreach (var grid in Inst.gridObjects) {
            if (grid.IsDotori)
                dotoriNum++;
        }
        return dotoriNum;
    }

    public static List<GridObjectScript> GetDotoriObjects() {
        List<GridObjectScript> objects = new List<GridObjectScript>();
        foreach (var obj in Inst.gridObjects) {
            if (obj.IsDotori)
                objects.Add(obj);
        }

        return objects;
    }

    public static void UseDotoriObject(int _useNum) {
        int dotoriNum = 0;
        foreach (var grid in Inst.gridObjects) {
            if (grid.IsDotori) {
                dotoriNum++;
                Inst.StartCoroutine(grid.RemoveObject());
            }
            if (dotoriNum == _useNum)
                return;
        }
    }

    public static IEnumerator ExecuteGridObjectCo() {
        GridObjectScript    grid = null;
        GridObjectScript    donburyGrid = null;
        string              menu = null;

        for (int i = 0; i < 16; i++) {
            grid = Inst.gridObjects[i];

            if (grid.CountDown != -1) { // 요리 중인 오브젝트
                if (grid.IsNewCooking)
                    grid.IsNewCooking = false;
                else {
                    grid.CountDown -= 1;
                    if (grid.CountDown == 0) {
                        GridObjectSO.ExecuteObjectFunc(grid.ObjectName, SO_Timing.End, grid.Position[0], grid.Position[1]);
                        grid.EndCooking();
                    }
                    else
                        GridObjectSO.ExecuteObjectFunc(grid.ObjectName, SO_Timing.Effect, grid.Position[0], grid.Position[1]);
                    yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
                }
            }
            else if (grid.ObjectName == "밥솥(완료)" && (menu = Inst.DonburyCheck(ref donburyGrid)) != null) { // 밥솥(완료)
                Inst.MakeDonbury(grid, donburyGrid, menu);
                yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
            }
            else if (Inst.SO_GridObject.GetObjectItem(grid.ObjectName).tool == null) { // 도구가 아닌 오브젝트
                List<GridObjectScript> objects = GetAdjacentGridObjects(grid.Position[0], grid.Position[1]);
                foreach (var obj in objects) {
                    if (!obj.HasObject)
                        continue;

                    ObjectSubItemTool tool = Inst.SO_GridObject.GetObjectItem(obj.ObjectName).tool;
                    if (tool == null)
                        continue;
                    if (!tool.IsNeeded(grid.ObjectName, true, obj) && !tool.IsNeeded(grid.ObjectName, false, obj))
                        continue;

                    Inst.InputObject(grid.Position, obj.Position);
                    yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
                }
            }
        }

        CardGameMngScript.IsCoroutine[1] = false;
    }
}
