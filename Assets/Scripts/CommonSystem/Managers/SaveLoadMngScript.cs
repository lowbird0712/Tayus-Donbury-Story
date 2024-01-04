using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData {
    public int                  dotoriNum;
    public int                  unlockedStageNum;
    public int                  travelNoteOpenPageNum;
    public List<string>         itemNameList = new List<string>();
    public SerializedDateTime   lastPlayDateTime;
    public List<QuestData>      questDataList = new List<QuestData>();
}

[Serializable]
public class SerializedDateTime {
    public int day;
    public int month;
    public int year;
    public int timerSec;

    public SerializedDateTime(DateTime _dateTime, int _timerSec) {
        day = _dateTime.Day;
        month = _dateTime.Month;
        year = _dateTime.Year;
        timerSec = _timerSec;
    }
}

[Serializable]
public class QuestData {
    public int          recipeIndex;
    public bool         cleared;
}

public class SaveLoadMngScript : MonoBehaviour {
    static public SaveLoadMngScript Inst { get; set; } = null;

    SaveData                        saveData = null;
    string                          filePath = null;

    private void Awake() => Inst = this;

    private void Start() {
        filePath = Application.persistentDataPath + "/SaveData.txt";
        Debug.Log(filePath); ////
        Load();
    }

    private void OnApplicationQuit() => Save();

    static public SaveData          SaveData => Inst.saveData;

    static SaveData MakeSaveData() {
        LinBoxMngScript.Init();
        SaveData saveData = new SaveData();
        saveData.dotoriNum = MainGameMngScript.DotoriNum.Value;
        saveData.unlockedStageNum = StageMngScript.GetUnlockedStageNum();
        saveData.travelNoteOpenPageNum = TravelNoteMngScript.GetOpenPageNum();
        saveData.itemNameList = LinBoxMngScript.GetItemNameList();
        saveData.lastPlayDateTime = new SerializedDateTime(TayuBoxMngScript.LastPlayDateTime, (int)WallpaperTimerMngScript.TimerSec);
        saveData.questDataList = TayuBoxMngScript.GetQuestDataList();
        return saveData;
    }

    static public void Save(bool _cheat = false) {
        Inst.saveData = MakeSaveData();
        //string jsonData = JsonUtility.ToJson(Inst.saveData);
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        //string byteString = Convert.ToBase64String(bytes);
        //File.WriteAllText(Inst.filePath, byteString);
        if (_cheat)
            SaveData.dotoriNum = 10000;
        string jsonData = JsonUtility.ToJson(Inst.saveData, true);
        File.WriteAllText(Inst.filePath, jsonData);
    }

    static public void Load() {
        if (!File.Exists(Inst.filePath))
            Save(true);
        //string byteString = File.ReadAllText(Inst.filePath);
        //byte[] bytes = Convert.FromBase64String(byteString);
        //string jsonData = System.Text.Encoding.UTF8.GetString(bytes);
        //Inst.saveData = JsonUtility.FromJson<SaveData>(jsonData);
        string jsonData = File.ReadAllText(Inst.filePath);
        Inst.saveData = JsonUtility.FromJson<SaveData>(jsonData);
    }

    public void TestSave() => Save();
}
