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

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
        _items = Resources.LoadAll<ItemData>("Item");

        for(int i = 0, cnt = _items.Length; i < cnt; i++)
        {
            _itemDic.Add(_items[i].ID, _items[i]);
        }
    }
}
