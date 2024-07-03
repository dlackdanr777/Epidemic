using System;
using UnityEngine;

public class GridInventory : MonoBehaviour
{
    public event Action OnUpdateHandler;

    private static readonly int _gridWidth = 10;
    private static readonly int _gridHight = 10;
    public int GridWidth =>  _gridWidth;
    public int GridHight => _gridHight;

    private GridSlot[,] _inventoryGrid = new GridSlot[_gridWidth, _gridHight];
    public GridSlot[,] InventoryGrid => _inventoryGrid;

    [SerializeField] private ItemData _item;
    [SerializeField] private ItemData _item2;

    private void Awake()
    {
        for(int y = 0; y < _gridHight; y++)
        {
            for(int x = 0; x < _gridWidth; x++)
            {
                _inventoryGrid[y, x] = new GridSlot();
            }
        }

        AddItem(_item);
        AddItem(_item);
        AddItem(_item);
        AddItem(_item);
        AddItem(_item2);
        AddItem(_item);
    }


    public bool AddItem(ItemData itemData)
    {
        Vector2Int slotIndex = GetNullSlotIndex(itemData);

        if (slotIndex.x == -1 || slotIndex.y == -1)
        {
            Debug.LogError("인벤토리가 꽉 찼습니다.");
            return false;
        }

        Item item = itemData.CreateItem();
        int itemWidth = itemData.Width;
        int itemHeight = itemData.Height;

        for (int y = slotIndex.y; y < slotIndex.y + itemHeight; y++)
        {
            for (int x = slotIndex.x; x < slotIndex.x + itemWidth; x++)
            {
                _inventoryGrid[y, x].Item = item;
                _inventoryGrid[y, x].IsMainSlot = (x == slotIndex.x && y == slotIndex.y);
            }
        }

        OnUpdateHandler?.Invoke();
        return true;
    }

    public bool AddItem(ItemData itemData, int x, int y)
    {
        if (!IsSlotAvailable(x, y, itemData.Width, itemData.Height))
        {
            Debug.LogError("추가 할 수 없는 슬롯입니다.");
            return false;
        }

        Item item = itemData.CreateItem();
        int xx = x;
        int yy = y;
        int itemWidth = itemData.Width;
        int itemHeight = itemData.Height;

        for (int i = y, cntY = y + itemHeight; i < cntY; i++)
        {
            for (int j = x, cntX = x + itemWidth; j < cntX; j++)
            {
                _inventoryGrid[i, j].Item = item;
                _inventoryGrid[i, j].IsMainSlot = (xx == j && yy == i);
            }
        }

        OnUpdateHandler?.Invoke();
        return true;
    }


    public void RemoveItem(Item item)
    {
        for(int y = 0; y < _gridHight; y++)
        {
            for(int x = 0; x < _gridWidth; x++)
            {
                if (_inventoryGrid[y,x].Item == item)
                {
                    _inventoryGrid[y, x].Item = null;
                    _inventoryGrid[y, x].IsMainSlot = false;
                }
            }
        }

        OnUpdateHandler?.Invoke();
    }


    public void RemoveItem(int startX, int startY, int width, int height)
    {
        if (_gridHight < startY + height || _gridWidth < startX + width)
        {
            Debug.LogError("범위를 넘어섰습니다.");
            return;
        }

        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                _inventoryGrid[y, x].Item = null;
                _inventoryGrid[y, x].IsMainSlot = false;
            }
        }

        OnUpdateHandler?.Invoke();
    }



    public Vector2Int GetNullSlotIndex(ItemData item)
    {
        for (int y = 0, cntY = _gridHight - item.Height + 1; y < cntY; y++)
        {
            for (int x = 0, cntX = _gridWidth - item.Width + 1; x < cntX; x++)
            {
                if (IsSlotAvailable(x, y, item.Width, item.Height))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    private bool IsSlotAvailable(int startX, int startY, int width, int height)
    {
        if(_gridHight < startY + height || _gridWidth < startX + width)
            return false;

        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (_inventoryGrid[y, x].Item != null)
                    return false;
            }
        }

        return true;
    }


}
