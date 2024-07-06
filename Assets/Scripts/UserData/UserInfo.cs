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
    Length,
}


public static class UserInfo
{
    private static readonly int MAX_BULLET_COUNT = 1000;

    private static int _bulletCount;
    public static int BulletCount => _bulletCount;

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
}
