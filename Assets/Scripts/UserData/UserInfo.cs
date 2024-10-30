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

    private static List<SaveEnemyData> _enemyDataList = new List<SaveEnemyData>();
    public static List<SaveEnemyData> EnemyDataList => _enemyDataList;

    private static List<SaveDropItemData> _dropItemDataList = new List<SaveDropItemData>();
    public static List<SaveDropItemData> DropItemDataList => _dropItemDataList;

    private static List<SaveDoorData> _doorDataList = new List<SaveDoorData>();
    public static List<SaveDoorData> DoorDataList => _doorDataList;

    private static List<SaveBuildObjectData> _buildObjectDataList = new List<SaveBuildObjectData>();
    public static List<SaveBuildObjectData> BuildObjectDataList => _buildObjectDataList;


    private static Vector3 _playerPosition;
    public static Vector3 PlayerPosition => _playerPosition;

    private static Quaternion _playerRotation;
    public static Quaternion PlayerRotation => _playerRotation;

    private static Vector2 _mouseInput;
    public static Vector2 MouseInput => _mouseInput;

    public static void ClearData()
    {
        _playerPosition = Vector3.zero;
        _playerRotation = Quaternion.identity;
        _mouseInput = Vector2.zero;
        _bulletCount = 100;
        _currentHp = int.MaxValue;
        _loadBulletCount = int.MaxValue;
        _invenDataList.Clear();
        _invenDataDic.Clear();
        _enemyDataList.Clear();

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
            DebugLog.LogError("�̹� �߰��� �κ��Դϴ�.");
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

        DebugLog.LogError("�������� �ʴ� �κ��丮 �̸� �Դϴ�." + name);
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


    public static void SaveGame(Player player, List<Enemy> enemyList, List<DropItem> dropItemList, List<Door> doorList, List<BuildObject> buildObjectList)
    {
        SaveData saveData = new SaveData(player, _bulletCount, _loadBulletCount, _invenDataList, _equipItems, enemyList, dropItemList, doorList, buildObjectList);

        string json = JsonUtility.ToJson(saveData, true);
        string path = Application.persistentDataPath + "/GameSave.json";
        File.WriteAllText(path, json);

        DebugLog.Log("���� ������ ���� �Ϸ�: " + path);
    }


    public static void LoadGame()
    {
        string path = Application.persistentDataPath + "/GameSave.json";
        
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            _playerPosition = new Vector3(saveData.PlayerPositionX, saveData.PlayerPositionY, saveData.PlayerPositionZ);
            _playerRotation = new Quaternion(saveData.PlayerRotationX, saveData.PlayerRotationY, saveData.PlayerRotationZ, saveData.PlayerRotationW);
            _mouseInput = new Vector2(saveData.CameraMouseInputX, saveData.CameraMouseInputY);
            _bulletCount = saveData.BulletCount;
            _loadBulletCount = saveData.LoadBulletCount;
            _currentHp = saveData.CurrentHp;

            _enemyDataList.Clear();
            _enemyDataList = saveData.EnemyDataList;

            _dropItemDataList.Clear();
            _dropItemDataList = saveData.DropItemDataList;

            _doorDataList.Clear();  
            _doorDataList = saveData.DoorDataList;

            _buildObjectDataList.Clear();
            _buildObjectDataList = saveData.BuildObjectDataList;

            _invenDataList.Clear();
            _invenDataDic.Clear();
            for(int i = 0, cnt = saveData.InvenDataList.Count; i < cnt; i++)
            {
                InvenData data = saveData.InvenDataList[i].ToOriginalInvenData();
                _invenDataList.Add(data);
                _invenDataDic.Add(data.Name, data);
            }

            for(int i = 0, cnt = _equipItems.Length; i < cnt; ++i)
            {
                _equipItems[i] = null;
            }

            for(int i = 0, cnt = saveData.EquipItemDataList.Count; i < cnt; ++i)
            {
                Item item = ItemManager.Instance.GetItemByID(saveData.EquipItemDataList[i]);

                if(item == null)
                {
                    DebugLog.LogError("�ش��ϴ� �������� �������� �ʽ��ϴ�: " + saveData.EquipItemDataList[i]);
                    continue;
                }

                if(item is EquipmentItem equipItem)
                    ChangeEquipItem(equipItem);
            }

            DebugLog.Log("���� ������ �ҷ����� �Ϸ�");
        }
        else
        {
            DebugLog.LogError("����� ������ �ش� ��ο� �����ϴ�: " + path);
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
            DebugLog.Log("���� ������ �����Ǿ����ϴ�.");
        }
        else
        {
            DebugLog.Log("������ ���� ������ �����ϴ�.");
        }
    }
}
