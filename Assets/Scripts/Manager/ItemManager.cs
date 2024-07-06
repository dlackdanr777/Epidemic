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
        if(Array.Find(_itemDatabase.Items, x => x.ID == ID) != null) //���� ���� ID�� ���� ID�� ������
        {
            return Array.Find(_itemDatabase.Items, x => x.ID == ID).CreateItem();
        }
        else
        {
            Debug.Log("[Item Database] �� ��ġ�ϴ� ID�� �����ϴ�." + ID);
            return null;
        }
    }

    public ItemData GetItemDataByID(string ID)
    {
        if (Array.Find(_itemDatabase.Items, x => x.ID == ID) != null) //���� ���� ID�� ���� ID�� ������
        {
            return Array.Find(_itemDatabase.Items, x => x.ID == ID);
        }
        else
        {
            Debug.Log("[Item Database] �� ��ġ�ϴ� ID�� �����ϴ�." + ID);
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
