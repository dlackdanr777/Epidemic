using System;
using System.Collections.Generic;
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


    private static readonly int MAX_BULLET_COUNT = 1000;

    private static int _bulletCount;
    public static int BulletCount => _bulletCount;

    private static float _speed = 1;
    public static float Speed => Mathf.Clamp(_speed, 0.5f, 2f);

    private static int _armor;
    public static int Armor => Mathf.Clamp(_armor, 0, 100);


    private static List<InvenData> _invenDataList = new List<InvenData>();
    private static Dictionary<string, InvenData> _invenDataDic = new Dictionary<string, InvenData>();
    
    private static EquipmentItem[] _equipItems = new EquipmentItem[(int)EquipItemType.Length];


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
}
