using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum EquipItemType
{
    Head,
    UpperBody,
    LowerBody,
    Shoes,
    Weapon,
    BodyArmor,
    Length,
}


public static class UserInfo
{
    public static event Action OnChangeSpeedHandler;
    public static event Action OnChangeEquipItemHandler;
    public static event Action OnChangeBulletHandler;
    public static event Action OnChangeLoadBulletHandler;

    private static int _bulletCount;
    public static int BulletCount => _bulletCount;

    private static int _loadBulletCount;
    public static int LoadBulletCount => _loadBulletCount;
    private static float _currentHp;
    public static float CurrentHp => _currentHp;

    private static List<InvenData> _invenDataList = new List<InvenData>();
    private static Dictionary<string, InvenData> _invenDataDic = new Dictionary<string, InvenData>();
    
    private static EquipmentItem[] _equipItems = new EquipmentItem[(int)EquipItemType.Length];

    private static SaveData _saveData;
    public static SaveData SaveData => _saveData;

    public static void ClearData()
    {
        _bulletCount = 100;
        _currentHp = int.MaxValue;
        _loadBulletCount = int.MaxValue;
        _invenDataList.Clear();
        _invenDataDic.Clear();

        for (int i = 0, cnt = _equipItems.Length; i < cnt; i++)
        {
            _equipItems[i] = null;
        }
    }


    public static bool AddBulletCount(int value)
    {
        if (ConstValue.MAX_BULLET_COUNT < _bulletCount + value)
            return false;

        _bulletCount = _bulletCount + value;
        OnChangeBulletHandler?.Invoke();
        return true;
    }

    public static void AddBulletCountNoLimit(int value)
    {
        _bulletCount = Mathf.Max(0, _bulletCount + value);
        OnChangeBulletHandler?.Invoke();
    }


    public static void ChangeLoadBulletCount(int value)
    {
        _loadBulletCount = value;
        OnChangeLoadBulletHandler?.Invoke();
    }


    public static bool IsAddInven(string name)
    {
        if (_invenDataDic.ContainsKey(name))
            return true;

        return false;
    }


    public static void AddInven(string name, int slotWidth, int slotHeight)
    {
        if(_invenDataDic.ContainsKey(name))
        {
            DebugLog.LogError("이미 추가된 인벤입니다.");
            return;
        }    

        InvenData data = new InvenData(name, slotWidth, slotHeight);
        _invenDataList.Add(data);
        _invenDataDic.Add(name, data);
    }

    public static InvenData GetInvenData(string name)
    {
        if(_invenDataDic.TryGetValue(name, out InvenData data))
            return data;

        DebugLog.LogError("존재하지 않는 인벤토리 이름 입니다." + name);
        return null;
    }


    public static void ChangeEquipItem(EquipmentItem item)
    {
        if (_equipItems[(int)item.EquipmentItemData.Type] != null)
            _equipItems[(int)item.EquipmentItemData.Type].EquipmentItemData.Dequip();

        if (item == null)
            return;

        item.EquipmentItemData.Equip();
        _equipItems[(int)item.EquipmentItemData.Type] = item;
        OnChangeEquipItemHandler?.Invoke();
    }

    public static void NullEquipItem(EquipItemType type)
    {
        if (_equipItems[(int)type] != null)
            _equipItems[(int)type].EquipmentItemData.Dequip();

        _equipItems[(int)type] = null;
        OnChangeEquipItemHandler?.Invoke();
    }

    public static EquipmentItem GetEquipItem(EquipItemType type)
    {
        return _equipItems[(int)type];
    }


    public static void SaveGame(GameStateSaveData gameStateSaveData, Player player, List<Enemy> enemyList, List<DropItem> dropItemList, List<Door> doorList, List<BuildObject> buildObjectList)
    {
        SaveData saveData = new SaveData(gameStateSaveData, player, _bulletCount, _loadBulletCount, _invenDataList, _equipItems, enemyList, dropItemList, doorList, buildObjectList);

        string json = JsonUtility.ToJson(saveData, true);
        string path = Application.persistentDataPath + "/GameSave.json";
        File.WriteAllText(path, json);

        DebugLog.Log("게임 데이터 저장 완료: " + path);
    }


    public static void LoadGame()
    {
        string path = Application.persistentDataPath + "/GameSave.json";
        
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _saveData = JsonUtility.FromJson<SaveData>(json);

            _bulletCount = _saveData.BulletCount;
            _loadBulletCount = _saveData.LoadBulletCount;
            _currentHp = _saveData.CurrentHp;

            _invenDataList.Clear();
            _invenDataDic.Clear();
            for(int i = 0, cnt = _saveData.InvenDataList.Count; i < cnt; i++)
            {
                InvenData data = _saveData.InvenDataList[i].ToOriginalInvenData();
                _invenDataList.Add(data);
                _invenDataDic.Add(data.Name, data);
            }

            for(int i = 0, cnt = _equipItems.Length; i < cnt; ++i)
            {
                _equipItems[i] = null;
            }

            for(int i = 0, cnt = _saveData.EquipItemDataList.Count; i < cnt; ++i)
            {
                Item item = ItemManager.Instance.GetItemByID(_saveData.EquipItemDataList[i]);

                if(item == null)
                {
                    DebugLog.LogError("해당하는 아이템이 존재하지 않습니다: " + _saveData.EquipItemDataList[i]);
                    continue;
                }

                if(item is EquipmentItem equipItem)
                    ChangeEquipItem(equipItem);
            }

            DebugLog.Log("게임 데이터 불러오기 완료");
        }
        else
        {
            DebugLog.LogError("저장된 파일이 해당 경로에 없습니다: " + path);
            return;
        }
    }

    public static bool IsSaveFileExists()
    {
        string path = Application.persistentDataPath + "/GameSave.json";
        if (File.Exists(path))
            return true;

        return false;
    }

    public static void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/GameSave.json";
        if(File.Exists(path))
        {
            File.Delete(path);
            DebugLog.Log("저장 파일이 삭제되었습니다.");
        }
        else
        {
            DebugLog.Log("삭제할 저장 파일이 없습니다.");
        }
    }
}
