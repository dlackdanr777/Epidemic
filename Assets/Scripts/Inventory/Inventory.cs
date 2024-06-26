using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary> �κ��丮 ���� ����� ���� Ŭ���� </summary>
public class Inventory : MonoBehaviour
{
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("Inventory");
                _instance = obj.AddComponent<Inventory>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static readonly int _inventoryItemsCount = 80;
    private static Inventory _instance;
    public static event Action OnUpdateHandler;

    [SerializeField] private Item[] _inventoryItems = new Item[_inventoryItemsCount];
    public Item[] InventoryItems => _inventoryItems;


    private void Awake()
    {
        if(_instance != null)

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary> _inventoryItems.Count�� ��ȯ�ϴ� �Լ� </summary>
    public int GetItemCount()
    {
        return _inventoryItemsCount;
    }

    /// <summary> ������ id������ �κ��丮�� �������� �߰��ϴ� �Լ� </summary>
    public bool AddItemByID(int ID, int amount = 1)
    {
        Item item = ItemManager.Instance.GetItemByID(ID);
        int nullSlotIndex = GetNullSlotIndex();
        if (item == null)
        {
            Debug.Log("[Item Databse] �������� �ʴ� ItemID �Դϴ�.");
            return false;
        }

        //ī��Ʈ�� �� �� �ִ� �������� ���
        if (item is CountableItem)
        {
            CountableItem countableItem = null;
            for(int i = 0; i < _inventoryItemsCount; ++i)
            {
                if (_inventoryItems[i] == null)
                    continue;

                if (_inventoryItems[i].Data == null)
                    continue;

                if (ID == _inventoryItems[i].Data.ID)
                {
                    int maxAmount = ((CountableItem)_inventoryItems[i]).CountableData.MaxAmount;
                    if (_inventoryItems[i].Amount == maxAmount)
                        continue;

                    countableItem = (CountableItem)_inventoryItems[i];
                    break;
                }
            }

            //�κ��丮 �ȿ� �������� �����ϴ� ���
            if(countableItem != null)
            {
                //���� ���� ������ �ִ� ������ �Ѵ� �������� ���Դٸ�?
                if(countableItem.CountableData.MaxAmount < countableItem.Amount + amount)
                {
                    if(nullSlotIndex == -1)
                    {
                        DebugLog.LogError("�κ��丮�� ��á���ϴ�.");
                        return false;
                    }

                    int addAmount = (countableItem.Amount + amount) - countableItem.CountableData.MaxAmount;
                    countableItem.Amount = countableItem.CountableData.MaxAmount;

                    CountableItem addItem = countableItem.Clone();
                    addItem.Amount = addAmount;

                    _inventoryItems[nullSlotIndex] = addItem;
                    OnUpdateHandler?.Invoke();
                    return true;
                }

                //���� ���
                else
                {
                    countableItem.Amount += amount;
                    OnUpdateHandler?.Invoke();
                    return true;
                }
            }

            else
            {
                if (nullSlotIndex == -1)
                {
                    DebugLog.LogError("�κ��丮�� ��á���ϴ�.");
                    return false;
                }

                countableItem = (CountableItem)item;
                item.Amount = amount;
                _inventoryItems[nullSlotIndex] = item;
                OnUpdateHandler?.Invoke();
                return true;
            }
        }

        //ī��Ʈ�� �� �� ���� �������� ���
        if (nullSlotIndex == -1)
        {
            DebugLog.LogError("�κ��丮�� �����ֽ��ϴ�.");
            return false;
        }

        _inventoryItems[nullSlotIndex] = item;
        OnUpdateHandler?.Invoke();
        return true;
    }


    /// <summary> �������� ����ϴ� �Լ� </summary>
    public bool UseItemByID(int ID, int amount = 1)
    {
        int itemCount = FindItemCountByID(ID);
        if (itemCount <= -1 && itemCount < amount)
        {
            Debug.Log("�����ϰ� �ִ� �������� ������ �����մϴ�.");
            return false;
        }

        List<Item> items = FindAll(ID);
        foreach (Item item in items)
        {
            if (item.Amount < amount)
                continue;

            item.Amount -= amount;

            if (item.Amount <= 0)
                RemoveItem(item);

            OnUpdateHandler?.Invoke();
            return true;
        }

        return false;
    }


    /// <summary> List�� �ش� id�� ���� �������� ��ִ����� ��ȯ�ϴ� �Լ� </summary>
    public int FindExtraAountCountByID(int ID)
    {
        List<Item> items = FindAll(ID);

        int itemCount = 0;
        for (int i = 0, count = items.Count; i < count; i++)
        {
            CountableItem CountableItem = items[i] as CountableItem;
            if (items[i].Amount < CountableItem.CountableData.MaxAmount)
            {
                itemCount++;
            }
        }
        return itemCount;
    }


    /// <summary> itemList�� �ش�ID�� �ִ������ �ƴ� �������� ã�� list�� ��ȯ�ϴ� �Լ� </summary>
    private List<Item> FindExtraAountListByID(int ID)
    {
        List<Item> items = FindAll(ID);
        List<Item> itemBox = new List<Item>();
        for (int i = 0, count = items.Count; i < count; i++)
        {
            CountableItem CountableItem = items[i] as CountableItem;
            if (items[i].Amount < CountableItem.CountableData.MaxAmount)
            {
                itemBox.Add(items[i]);
            }
        }
        return itemBox;
    }


    /// <summary> ������ ID�� ���� �κ��丮�� �ش� �������� � �ִ��� Ȯ���ϴ� �Լ� </summary>
    public int FindItemCountByID(int ID)
    {
        List<Item> items = FindAll(ID);
        int totalAmount = 0;
        if (items.Count <= 0)
            return -1;

        if (items[0] is CountableItem)
        {
            for (int i = 0, count = items.Count; i < count; i++)
            {
                CountableItem CountableItem = items[i] as CountableItem;
                totalAmount += CountableItem.Amount;
            }
        }
        else if (items[0] is EquipmentItem)
        {
            totalAmount = items.Count;
        }

        return totalAmount;

    }


    /// <summary> �ΰ��� �������� �����ִ��Լ� </summary>
    public void MergeItem(Item mainItem, Item subItem)
    {
        if (!(mainItem is CountableItem) || mainItem.Data.ID != subItem.Data.ID)
        {
            SwapSlot(mainItem, subItem); 
            OnUpdateHandler?.Invoke();
            return;
        }

        CountableItem CountableItem = mainItem as CountableItem;
        if (mainItem.Amount + subItem.Amount <= CountableItem.CountableData.MaxAmount)
        {
            mainItem.Amount += subItem.Amount;
            RemoveItem(subItem);
        }
        else
        {
            if (mainItem.Amount == CountableItem.CountableData.MaxAmount)
            {
                SwapSlot(mainItem, subItem);
            }

            else
            {
                subItem.Amount -= CountableItem.CountableData.MaxAmount - mainItem.Amount;
                mainItem.Amount = CountableItem.CountableData.MaxAmount;
            }
        }

        OnUpdateHandler?.Invoke();
    }


    /// <summary> �������� �����ϴ� �Լ� </summary>
    public void RemoveItem(Item item)
    {
        if (!_inventoryItems.Contains(item))
            return;

        for(int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == item)
            {
                _inventoryItems[i] = null;
                OnUpdateHandler.Invoke();
                return;
            }
        }
    }


    /// <summary> �������� ������ ���ҽ�Ű�� �Լ� </summary>
    public void SubItem(Item item, int amount = 1)
    {
        if (!_inventoryItems.Contains(item))
            return;

        item.Amount -= amount;

        if (item.Amount <= 0)
            RemoveItem(item);

        OnUpdateHandler?.Invoke();
    }


    /// <summary> �������� ������ ���ҽ�Ű�� �Լ� </summary>
    public void SubItemByID(int ID, int amount = 1)
    {
        Item item = Find(ID);
        if (!_inventoryItems.Contains(item))
            return;

        item.Amount -= amount;
        if (item.Amount <= 0)
            RemoveItem(item);

        OnUpdateHandler?.Invoke();
    }


    public void SwapSlot(Item item1, Item item2)
    {
        int item1Index = -1;
        int item2Index = -1;
        for(int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == null)
                continue;

            if (_inventoryItems[i] == item1)
                item1Index = i;

            else if (_inventoryItems[i] == item2)
                item2Index = i;

            if (item1Index != -1 && item2Index != -1)
                break;
        }

        _inventoryItems[item1Index] = item2;
        _inventoryItems[item2Index] = item1;
        OnUpdateHandler?.Invoke();
    }


    public void ChangeItemSlot(Item item, int index)
    {
        int itemIndex = -1;
        for (int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == null)
                continue;

            if (_inventoryItems[i] == item)
            {
                itemIndex = i;
                break;
            }
        }

        _inventoryItems[itemIndex] = null;
        _inventoryItems[index] = item;
        OnUpdateHandler?.Invoke();
    }


    /// <summary> �������� �ѷ� ������ �Լ� </summary>
    public void DivItem(Item item, int Amount)
    {
        if (item == null)
        {
            DebugLog.LogError("�������� �����ϴ�.");
            return;
        }

        int nullSlotIndex = GetNullSlotIndex();

        if (nullSlotIndex == -1)
        {
            DebugLog.LogError("�κ��丮�� �� á���ϴ�.");
            return;
        }

        if (item.Amount <= Amount)
        {
            DebugLog.LogError("������ �������� ���� ������ Ů�ϴ�.");
            return;
        }

        Item newItem = item.Data.CreateItem();
        item.Amount -= Amount;

        newItem.Amount = Amount;
        _inventoryItems[nullSlotIndex] = newItem;

        OnUpdateHandler?.Invoke();  
    }


    /// <summary> �κ��丮�� �����ϴ� �Լ� </summary>
    public void SortInventory()
    {
        HashSet<int> itemIdSet = new HashSet<int>();

        for(int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == null)
                continue;

            itemIdSet.Add(_inventoryItems[i].Data.ID);
        }

        foreach (int ID in itemIdSet) //ID�� ����ŭ �ݺ��Ѵ�.
        {
            if (!(ItemManager.Instance.GetItemByID(ID) is CountableItem)) //���� ������ �� �� �ִ� �������� �ƴ϶��
                continue;

            while (1 < FindExtraAountCountByID(ID)) //�ִ������ �ƴ� �ش�ID�� ������ �� ������ 1�̻��϶� �ݺ� 
            {
                List<Item> items = FindExtraAountListByID(ID); //�ִ������ �ƴ� �����۸���Ʈ�� �̾Ƴ���
                MergeItem(items[0], items[1]); //���ʺ��� ���ս�Ų��.
            }
        }

        _inventoryItems = _inventoryItems.OrderBy(x => x == null).ThenBy(x => x?.Data.Name).ThenByDescending(x => x?.Amount).ToArray(); //�̸����� ��������, ������ �������� ����
        OnUpdateHandler?.Invoke();
    }


    private Item Find(int Id)
    {
        for (int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == null)
                continue;

            if (_inventoryItems[i].Data.ID == Id)
                return _inventoryItems[i];
        }

        return null;
    }



    private List<Item> FindAll(int Id)
    {
        List<Item> items = new List<Item>();

        for (int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == null)
                continue;

            if (_inventoryItems[i].Data.ID == Id)
                items.Add(_inventoryItems[i]);
        }

        return items;
    }

    private int GetNullSlotIndex()
    {
        for(int i = 0; i < _inventoryItemsCount; i++)
        {
            if (_inventoryItems[i] == null)
                return i;
        }

        return -1;
    }

}
