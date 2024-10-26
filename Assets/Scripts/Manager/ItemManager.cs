using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject("ItemManager");
                _instance = obj.AddComponent<ItemManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }


    private static ItemManager _instance;
    private static ItemData[] _items;
    private static Dictionary<string, ItemData> _itemDic = new Dictionary<string, ItemData>();

    public Item GetItemByID(string ID) 
    { 
        if(_itemDic.TryGetValue(ID, out ItemData data))
        {
            return data.CreateItem();
        }
        Debug.Log("[Item Database] 상에 일치하는 ID가 없습니다." + ID);
        return null;
    }

    public ItemData GetItemDataByID(string ID)
    {
        if (_itemDic.TryGetValue(ID, out ItemData data))
        {
            return data;
        }

        Debug.Log("[Item Database] 상에 일치하는 ID가 없습니다." + ID);
        return null;
    }

    public string GetItemEffectText(ItemData data)
    {
        string effectDescription = string.Empty;
        if (data is IPlayerRecoveryHealth healthData)
        {
            int recoveryValue = healthData.RecoveryValue;
            string symbol = recoveryValue < 0 ? string.Empty : "+";
            if (recoveryValue != 0)
                effectDescription += Utility.SetStringColor(recoveryValue < 0 ? ColorType.Negative : ColorType.Positive, "체력 회복량 " + symbol + recoveryValue) + "\n";
        }

        if(data is IPlayerChangeArmor armorData)
        {
            int addArmor = armorData.Armor;
            string symbol = addArmor < 0 ? string.Empty : "+";
            if (addArmor != 0)
                effectDescription += Utility.SetStringColor(addArmor < 0 ? ColorType.Negative : ColorType.Positive, "방어력 " + symbol + addArmor) + "\n";
        }

        if (data is IPlayerChangeSpeed speedData)
        {
            float speedValue = speedData.Speed;
            string symbol = speedValue < 0 ? string.Empty : "+";
            if (speedValue != 0)
                effectDescription += Utility.SetStringColor(speedValue < 0 ? ColorType.Negative : ColorType.Positive, "이동속도 " + symbol + speedValue);
        }


        return effectDescription;
    }



    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
        _items = Resources.LoadAll<ItemData>("Item");
        _itemDic.Clear();
        for(int i = 0, cnt = _items.Length; i < cnt; i++)
        {
            _itemDic.Add(_items[i].ID, _items[i]);
        }
    }
}
