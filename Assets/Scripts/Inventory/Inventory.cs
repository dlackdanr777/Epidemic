using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary> 인벤토리 관련 기능을 가진 클래스 </summary>
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

    /// <summary> _inventoryItems.Count를 반환하는 함수 </summary>
    public int GetItemCount()
    {
        return _inventoryItemsCount;
    }

    /// <summary> 아이템 id값으로 인벤토리에 아이템을 추가하는 함수 </summary>
    public bool AddItemByID(int ID, int amount = 1)
    {
        Item item = ItemManager.Instance.GetItemByID(ID);
        int nullSlotIndex = GetNullSlotIndex();
        if (item == null)
        {
            Debug.Log("[Item Databse] 존재하지 않는 ItemID 입니다.");
            return false;
        }

        //카운트를 셀 수 있는 아이템인 경우
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

            //인벤토리 안에 아이템이 존재하는 경우
            if(countableItem != null)
            {
                //만약 현재 아이템 최대 수량을 넘는 아이템이 들어왔다면?
                if(countableItem.CountableData.MaxAmount < countableItem.Amount + amount)
                {
                    if(nullSlotIndex == -1)
                    {
                        DebugLog.LogError("인벤토리가 꽉찼습니다.");
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

                //작을 경우
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
                    DebugLog.LogError("인벤토리가 꽉찼습니다.");
                    return false;
                }

                countableItem = (CountableItem)item;
                item.Amount = amount;
                _inventoryItems[nullSlotIndex] = item;
                OnUpdateHandler?.Invoke();
                return true;
            }
        }

        //카운트를 셀 수 없는 아이템인 경우
        if (nullSlotIndex == -1)
        {
            DebugLog.LogError("인벤토리가 꽉차있습니다.");
            return false;
        }

        _inventoryItems[nullSlotIndex] = item;
        OnUpdateHandler?.Invoke();
        return true;
    }


    /// <summary> 아이템을 사용하는 함수 </summary>
    public bool UseItemByID(int ID, int amount = 1)
    {
        int itemCount = FindItemCountByID(ID);
        if (itemCount <= -1 && itemCount < amount)
        {
            Debug.Log("소지하고 있는 아이템의 갯수가 부족합니다.");
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


    /// <summary> List에 해당 id를 가진 아이템이 몇개있는지를 반환하는 함수 </summary>
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


    /// <summary> itemList에 해당ID의 최대수량이 아닌 아이템을 찾아 list로 반환하는 함수 </summary>
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


    /// <summary> 아이템 ID를 통해 인벤토리에 해당 아이템이 몇개 있는지 확인하는 함수 </summary>
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


    /// <summary> 두개의 아이템을 합쳐주는함수 </summary>
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


    /// <summary> 아이템을 삭제하는 함수 </summary>
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


    /// <summary> 아이템의 수량을 감소시키는 함수 </summary>
    public void SubItem(Item item, int amount = 1)
    {
        if (!_inventoryItems.Contains(item))
            return;

        item.Amount -= amount;

        if (item.Amount <= 0)
            RemoveItem(item);

        OnUpdateHandler?.Invoke();
    }


    /// <summary> 아이템의 수량을 감소시키는 함수 </summary>
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


    /// <summary> 아이템을 둘로 나누는 함수 </summary>
    public void DivItem(Item item, int Amount)
    {
        if (item == null)
        {
            DebugLog.LogError("아이템이 없습니다.");
            return;
        }

        int nullSlotIndex = GetNullSlotIndex();

        if (nullSlotIndex == -1)
        {
            DebugLog.LogError("인벤토리가 꽉 찼습니다.");
            return;
        }

        if (item.Amount <= Amount)
        {
            DebugLog.LogError("아이템 수량보다 나눌 갯수가 큽니다.");
            return;
        }

        Item newItem = item.Data.CreateItem();
        item.Amount -= Amount;

        newItem.Amount = Amount;
        _inventoryItems[nullSlotIndex] = newItem;

        OnUpdateHandler?.Invoke();  
    }


    /// <summary> 인벤토리를 정렬하는 함수 </summary>
    public void SortInventory()
    {
        HashSet<int> itemIdSet = new HashSet<int>();

        for(int i = 0; i < _inventoryItemsCount; ++i)
        {
            if (_inventoryItems[i] == null)
                continue;

            itemIdSet.Add(_inventoryItems[i].Data.ID);
        }

        foreach (int ID in itemIdSet) //ID의 수만큼 반복한다.
        {
            if (!(ItemManager.Instance.GetItemByID(ID) is CountableItem)) //만약 갯수를 셀 수 있는 아이템이 아니라면
                continue;

            while (1 < FindExtraAountCountByID(ID)) //최대수량이 아닌 해당ID의 아이템 의 갯수가 1이상일때 반복 
            {
                List<Item> items = FindExtraAountListByID(ID); //최대수량이 아닌 아이템리스트를 뽑아내서
                MergeItem(items[0], items[1]); //앞쪽부터 병합시킨다.
            }
        }

        _inventoryItems = _inventoryItems.OrderBy(x => x == null).ThenBy(x => x?.Data.Name).ThenByDescending(x => x?.Amount).ToArray(); //이름으로 오름차순, 갯수로 내림차순 정렬
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
