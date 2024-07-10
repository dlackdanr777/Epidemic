using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum EquipItemType
{
    Head,
    UpperBody,
    LowerBody,
    Shoes,
    Weapon,
    Backpack,
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
    public static float Speed => _speed;

    private static int _armor;
    public static int  Armor => _armor;



    private static List<InvenData> _invenDataList = new List<InvenData>();
    private static Dictionary<string, InvenData> _invenDataDic = new Dictionary<string, InvenData>();
    
    private static EquipmentItem[] _equipItems = new EquipmentItem[(int)EquipItemType.Length];


    public static void AddBulletCount(int value)
    {
        _bulletCount = Mathf.Clamp(_bulletCount + value, 0, MAX_BULLET_COUNT);
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
