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
    public static event Action<EquipItemType> OnChangeEquipItemHandler;

    private static Vector3 _playerPosition;
    public static Vector3 PlayerPosition => _playerPosition;

    private static readonly int MAX_BULLET_COUNT = 1000;

    private static int _bulletCount;
    public static int BulletCount => _bulletCount;

    private static int _loadBulletCount;
    public static int LoadBulletCount => _loadBulletCount;
    private static int _currentHp;
    public static int CurrentHp => _currentHp;

    private static float _speed = 1;
    public static float Speed => Mathf.Clamp(_speed, 0.5f, 2f);

    private static int _armor;
    public static int Armor => Mathf.Clamp(_armor, 0, 100);

    private static List<InvenData> _invenDataList = new List<InvenData>();
    private static Dictionary<string, InvenData> _invenDataDic = new Dictionary<string, InvenData>();
    
    private static EquipmentItem[] _equipItems = new EquipmentItem[(int)EquipItemType.Length];



    public static void ClearData()
    {
        _playerPosition = Vector3.zero;
        _bulletCount = 0;
        _speed = 1;
        _currentHp = 100;
        _loadBulletCount = 0;
        _armor = 0;
        _loadBulletCount = 0;
        _invenDataList.Clear();
        _invenDataDic.Clear();

        for(int i = 0, cnt = _equipItems.Length; i < cnt; i++)
        {
            _equipItems[i] = null;
        }
    }


    public static void AddBulletCount(int value)
    {
        _bulletCount = Mathf.Clamp(_bulletCount + value, 0, MAX_BULLET_COUNT);
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


    public static void AddSpeedMul(float value)
    {
        _speed = Mathf.Clamp(_speed + value, 0.5f, 2);
        OnChangeSpeedHandler?.Invoke();
    }

    public static void AddArmor(int value)
    {
        _armor = Mathf.Clamp(_armor + value, 0, 100);
    }


    public static void ChangeEquipItem(EquipmentItem item)
    {
        if (_equipItems[(int)item.EquipmentItemData.Type] != null)
            _equipItems[(int)item.EquipmentItemData.Type].EquipmentItemData.Dequip();

        if (item == null)
            return;

        item.EquipmentItemData.Equip();
        _equipItems[(int)item.EquipmentItemData.Type] = item;

        OnChangeEquipItemHandler?.Invoke(item.EquipmentItemData.Type);

        DebugLog.Log(_speed);
        DebugLog.Log(_armor);
    }

    public static void NullEquipItem(EquipItemType type)
    {
        if (_equipItems[(int)type] != null)
            _equipItems[(int)type].EquipmentItemData.Dequip();

        _equipItems[(int)type] = null;
        OnChangeEquipItemHandler?.Invoke(type);
    }

    public static EquipmentItem GetEquipItem(EquipItemType type)
    {
        return _equipItems[(int)type];
    }


    public static void SaveGame(Player player)
    {
        SaveData saveData = new SaveData(player.transform.position, _bulletCount, _loadBulletCount, _currentHp,  _invenDataList);

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
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            _playerPosition = new Vector3(saveData.PlayerPositionX, saveData.PlayerPositionY, saveData.PlayerPositionZ);
            _bulletCount = saveData.BulletCount;
            _loadBulletCount = saveData.LoadBulletCount;
            _currentHp = saveData.CurrentHp;

            _invenDataList.Clear();
            _invenDataDic.Clear();
            for(int i = 0, cnt = saveData.InvenDataList.Count; i < cnt; i++)
            {
                InvenData data = saveData.InvenDataList[i].ToOriginalInvenData();
                _invenDataList.Add(data);
                _invenDataDic.Add(data.Name, data);
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
