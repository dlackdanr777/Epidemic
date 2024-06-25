using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary> 인벤토리 관련 기능을 가진 클래스 </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private UIInventory UIInventory;
    [SerializeField] private List<Item> _inventoryItems = new List<Item>();


    /// <summary> _inventoryItems.Count를 반환하는 함수 </summary>
    public int GetItemCount()
    {
        return _inventoryItems.Count;
    }


    /// <summary> 보유 아이템 리스트[index]를 반환하는 함수 </summary>
    public Item GetItemByIndex(int index)
    {
        return _inventoryItems[index];
    }


    /// <summary> 아이템 id값으로 인벤토리에 아이템을 추가하는 함수 </summary>
    public bool AddItemByID(int ID, int amount = 1)
    {
        Item item = ItemManager.Instance.GetItemByID(ID);
        if (item == null)
        {
            Debug.Log("[Item Databse] 존재하지 않는 ItemID 입니다.");
            return false;
        }

        if (item is CountableItem)
        {
            if (_inventoryItems.Exists(x => x.Data.ID == item.Data.ID))
            {
                Item countableItem = FindExtraAountItem(item);
                if (countableItem != null)
                {
                    return AddItemAmount(countableItem as CountableItem, amount);
                }
            }
            return AddItemToNewSlot(item, amount);
        }
        return AddItemToNewSlot(item);
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


        List<Item> items = _inventoryItems.FindAll(x => x.Data.ID == ID);
        foreach (Item item in items)
        {
            if (item.Amount < amount)
                continue;

            item.Amount -= amount;
            UIInventory.UpdateSlotUI(UIInventory.GetSlotByIndex(item.SlotIndex));

            if (item.Amount <= 0)
                RemoveItem(item);

            return true;
        }

        return false;
    }


    /// <summary> 아이템 클래스로 아이템을 추가시키는 함수 </summary>
    private bool AddItemAmount(CountableItem item, int amount = 1)
    {

        UIInventorySlot updateSlot = UIInventory.GetSlotByIndex(item.SlotIndex);
        if (item.Amount + amount <= item.CountableData.MaxAmount)
        {
            item.Amount += amount;
            UIInventory.UpdateSlotUI(updateSlot);
            return true;
        }

        if (!IsInventoryFull())
        {
            Item newItem = ItemManager.Instance.GetItemByID(item.Data.ID);
            int restAmount = item.Amount + amount - item.CountableData.MaxAmount;
            item.Amount = item.CountableData.MaxAmount;
            UIInventory.UpdateSlotUI(updateSlot);
            return AddItemToNewSlot(newItem, restAmount);
        }
        else
        {
            Debug.Log("인벤토리가 꽉차있습니다.");
            return false;
        }
    }


    /// <summary> 아이템을 새로운 슬롯에 추가시키는 함수 </summary>
    private bool AddItemToNewSlot(Item item, int amount = 1)
    {
        if (item == null)
        {
            Debug.Log("아이템이 존재하지 않습니다.");
            return false;
        }

        if (!IsInventoryFull())
        {
            item.Amount = amount;
            _inventoryItems.Add(item);
            int slotIndex = UIInventory.GetNullSlot();
            UIInventorySlot nullSlot = UIInventory.GetSlotByIndex(slotIndex);

            UIInventory.ChangeSlotItem(nullSlot, item);
            return true;
        }
        else
        {
            Debug.Log("인벤토리 슬롯이 차있습니다.");
            return false;
        }
    }


    /// <summary> 최대 수량이 아닌 아이템을 찾는 함수 </summary>
    public Item FindExtraAountItem(Item item)
    {
        if(item is CountableItem)
        {
            List<Item> items = _inventoryItems.FindAll(x => x.Data.ID == item.Data.ID);
            for (int i = 0, count = items.Count; i < count; i++)
            {
                CountableItem CountableItem = items[i] as CountableItem;
                if (items[i].Amount < CountableItem.CountableData.MaxAmount)
                {
                    return items[i];
                }
            }
        }
        return null;
    }


    /// <summary> List에 해당 id를 가진 아이템이 몇개있는지를 반환하는 함수 </summary>
    public int FindExtraAountCountByID(int ID)
    {
        List<Item> items = _inventoryItems.FindAll(x => x.Data.ID == ID);
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
    public List<Item> FindExtraAountListByID(int ID)
    {
        List<Item> items = _inventoryItems.FindAll(x => x.Data.ID == ID);
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
        List<Item> items = _inventoryItems.FindAll(x => x.Data.ID == ID);
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
        if (!(mainItem is CountableItem))
        {
            Debug.Log("장비 아이템은 합칠 수 없습니다.");
            int mainItemIndex = mainItem.SlotIndex;
            int subItemIndex = subItem.SlotIndex;
            UIInventory.SlotSwap(UIInventory.GetSlotByIndex(mainItemIndex), UIInventory.GetSlotByIndex(subItemIndex));
        }

        if (mainItem.Data.ID != subItem.Data.ID)
        {
            Debug.Log("두 아이템의 ID가 서로 다릅니다.");
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
                int mainItemIndex = mainItem.SlotIndex;
                int subItemIndex = subItem.SlotIndex;
                UIInventory.SlotSwap(UIInventory.GetSlotByIndex(mainItemIndex), UIInventory.GetSlotByIndex(subItemIndex));
            }
            else
            {
                subItem.Amount -= CountableItem.CountableData.MaxAmount - mainItem.Amount;
                mainItem.Amount = CountableItem.CountableData.MaxAmount;
            }
        }

        UIInventory.UpdateSlotUI(UIInventory.GetSlotByIndex(mainItem.SlotIndex));
        UIInventory.UpdateSlotUI(UIInventory.GetSlotByIndex(subItem.SlotIndex));
    }


    /// <summary> 아이템을 삭제하는 함수 </summary>
    public void RemoveItem(Item item)
    {
        if (!_inventoryItems.Contains(item))
            return;

        UIInventory.GetSlotByIndex(item.SlotIndex).UpdateUI(null);
        _inventoryItems.Remove(item);
    }


    /// <summary> 아이템을 삭제하는 함수 </summary>
    public void RemoveItem(int ID)
    {
        Item item = ItemManager.Instance.GetItemByID(ID);
        if (item == null)
            return;

        if (!_inventoryItems.Contains(item))
            return;

        UIInventory.GetSlotByIndex(item.SlotIndex).UpdateUI(null);
        _inventoryItems.Remove(item);
    }


    /// <summary> 아이템의 수량을 감소시키는 함수 </summary>
    public void SubItem(Item item, int amount = 1)
    {
        if (!_inventoryItems.Contains(item))
            return;

        item.Amount -= amount;

        if (item.Amount <= 0)
            RemoveItem(item);
        else
            UIInventory.GetSlotByIndex(item.SlotIndex).UpdateUI(item);
    }


    /// <summary> 아이템의 수량을 감소시키는 함수 </summary>
    public void SubItemByID(int ID, int amount = 1)
    {
        Item item = _inventoryItems.Find(x => x.Data.ID == ID);
        if (!_inventoryItems.Contains(item))
            return;

        item.Amount -= amount;
        if (item.Amount <= 0)
            RemoveItem(item);
        else
            UIInventory.GetSlotByIndex(item.SlotIndex).UpdateUI(item);
    }


    /// <summary> 아이템을 둘로 나누는 함수 </summary>
    public void DivItem(Item item, int Amount)
    {
        if (item != null)
        {
            Debug.Log("아이템이 없습니다.");
            return;
        }

        if (IsInventoryFull())
        {
            Debug.Log("인벤토리가 꽉 찼습니다.");
            return;
        }


        if (item.Amount <= Amount)
        {
            Debug.Log("아이템 수량보다 나눌 갯수가 큽니다.");
            return;
        }

        Item newItem = item.Data.CreateItem();
        item.Amount -= Amount;
        AddItemToNewSlot(newItem, Amount);
        UIInventory.UpdateSlotUI(UIInventory.GetSlotByIndex(item.SlotIndex));
    }


    /// <summary> 인벤토리를 정렬하는 함수 </summary>
    public void SortInventory()
    {
        int[] itemIDs = _inventoryItems.Select(x => x.Data.ID).Distinct().ToArray(); //인벤토리에 있는 Item ID를 중복을 제거하고 뽑아낸다.
        foreach (int ID in itemIDs) //ID의 수만큼 반복한다.
        {
            if (!(ItemManager.Instance.GetItemByID(ID) is CountableItem)) //만약 갯수를 셀 수 있는 아이템이 아니라면
                continue;

            while (1 < FindExtraAountCountByID(ID)) //최대수량이 아닌 해당ID의 아이템 의 갯수가 1이상일때 반복 
            {
                List<Item> items = FindExtraAountListByID(ID); //최대수량이 아닌 아이템리스트를 뽑아내서
                MergeItem(items[0], items[1]); //앞쪽부터 병합시킨다.
            }
        }

        _inventoryItems = _inventoryItems.OrderBy(x => x.Data.Name).ThenByDescending(x => x.Amount).ToList(); //이름으로 오름차순, 갯수로 내림차순 정렬
        for (int i = 0, count =  _inventoryItems.Count; i < count; i++)
        {
            _inventoryItems[i].SlotIndex = i; //UI상의 장소 인덱스를 앞쪽부터 순서대로 입력시킨다.
        }
        UIInventory.UpdateUI();
    }


    /// <summary> 인벤토리가 찼으면 참, 아니면 거짓을 반환 </summary>
    public bool IsInventoryFull()
    {
        if(UIInventory.GetNullSlot() == -1)
            return true;

        return false;
    }

    /// <summary> UI를 오픈하는 함수 </summary>
    public void OpenUI()
    {
        UIInventory.ChildSetActive();
        UIInventory.UpdateUI();
    }

}
