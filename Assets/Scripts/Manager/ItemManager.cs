using System;
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
    private ItemDatabase _itemDatabase;

    public Item GetItemByID(string ID) 
    { 
        if(Array.Find(_itemDatabase.Items, x => x.ID == ID) != null) //만약 받은 ID와 같은 ID가 있으면
        {
            return Array.Find(_itemDatabase.Items, x => x.ID == ID).CreateItem();
        }
        else
        {
            Debug.Log("[Item Database] 상에 일치하는 ID가 없습니다." + ID);
            return null;
        }
    }

    public ItemData GetItemDataByID(string ID)
    {
        if (Array.Find(_itemDatabase.Items, x => x.ID == ID) != null) //만약 받은 ID와 같은 ID가 있으면
        {
            return Array.Find(_itemDatabase.Items, x => x.ID == ID);
        }
        else
        {
            Debug.Log("[Item Database] 상에 일치하는 ID가 없습니다." + ID);
            return null;
        }
    }

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
        _itemDatabase = Resources.Load<ItemDatabase>("Item/ItemDatabase");
    }
}
