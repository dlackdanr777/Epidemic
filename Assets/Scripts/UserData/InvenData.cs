using System.Collections.Generic;
using UnityEngine;
using System;
public class InvenData
{
    public event Action OnUpdateHandler;

    private string _name;
    public string Name => _name;

    private GridSlot[,] _slots;
    public GridSlot[,] Slots => _slots;

    private Dictionary<string, int> _itemCountDic = new Dictionary<string, int>();
    public Dictionary<string, int> ItemCountDic => _itemCountDic;

    private int _width;
    public int Width => _width;
    private int _height;
    public int Height => _height;

    public InvenData(string name, int slotWidth, int slotHeight)
    {
        _name = name;
        _width = slotWidth;
        _height = slotHeight;
        _slots = new GridSlot[slotHeight, slotWidth];

        for (int i = 0; i < _height; ++i)
        {
            for (int j = 0; j < _width; ++j)
            {
                _slots[i, j] = new GridSlot();
            }
        }
    }


    public bool AddItem(InventoryItemData data, int indexX, int indexY)
    {
        if (!IsSlotAvailable(indexX, indexY, data.Width, data.Height))
        {
            Debug.LogError("추가 할 수 없는 슬롯입니다: " + indexY + ", " + indexX);
            return false;
        }

        Item item = data.CreateItem();
        int xx = indexX;
        int yy = indexY;
        int itemWidth = data.Width;
        int itemHeight = data.Height;

        for (int i = indexY, cntY = indexY + itemHeight; i < cntY; i++)
        {
            for (int j = indexX, cntX = indexX + itemWidth; j < cntX; j++)
            {
                _slots[i, j].Item = item;
                _slots[i, j].IsMainSlot = (xx == j && yy == i);
            }
        }

        AddCount(data.Id, 1);
        OnUpdateHandler?.Invoke();
        return true;
    }


    public bool AddItem(InventoryItemData data)
    {
        Vector2Int slotIndex = GetNullSlotIndex(data);

        if (slotIndex.x == -1 || slotIndex.y == -1)
        {
            DebugLog.LogError("인벤토리가 꽉 찼습니다.");
            return false;
        }

        Item item = data.CreateItem();
        int itemWidth = data.Width;
        int itemHeight = data.Height;

        for (int y = slotIndex.y; y < slotIndex.y + itemHeight; y++)
        {
            for (int x = slotIndex.x; x < slotIndex.x + itemWidth; x++)
            {
                _slots[y, x].Item = item;
                _slots[y, x].IsMainSlot = (x == slotIndex.x && y == slotIndex.y);
            }
        }

        AddCount(data.Id, 1);
        OnUpdateHandler?.Invoke();
        return true;
    }


    public void RemoveItem(Item item)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_slots[y, x].Item == item)
                {
                    _slots[y, x].Item = null;
                    _slots[y, x].IsMainSlot = false;
                }
            }
        }

        SubCount(item.Data.Id, 1);
        OnUpdateHandler?.Invoke();
    }

    public void RemoveItems(InventoryItemData data, int amount)
    {
        SubCount(data.Id, amount);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (amount <= 0)
                    break;

                if (_slots[y, x].Item == null || _slots[y, x].Item.Data == null || _slots[y, x].Item.Data.Id != data.Id)
                    continue;

                int minY = Mathf.Min(y + data.Height, _height);
                int minX = Mathf.Min(x + data.Width, _width);
                for (int i = y; i < minY; ++i)
                {
                    for (int j = x; j < minX; ++j)
                    {
                        _slots[i, j].Item = null;
                        _slots[i, j].IsMainSlot = false;
                        DebugLog.Log(data.Id + "위치: " + (i) + (j));
                    }
                }

                --amount;
            }
            OnUpdateHandler?.Invoke();
        }
    }

    public void RemoveItem(InventoryItemData data, int startX, int startY)
    {
        int width = data.Width;
        int height = data.Height;
        SubCount(data.Id, 1);
        if (_height < startY + height || _width < startX + width)
        {
            DebugLog.LogError("범위를 넘어섰습니다.");
            return;
        }

        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                _slots[y, x].Item = null;
                _slots[y, x].IsMainSlot = false;
            }
        }


        OnUpdateHandler?.Invoke();
    }

    public int GetItemCount(InventoryItemData data)
    {
        if (_itemCountDic.TryGetValue(data.Id, out var count))
        {
            return count < 0 ? 0 : count;
        }

        return 0;
    }


    private Vector2Int GetNullSlotIndex(InventoryItemData data)
    {
        for (int y = 0, cntY = _height - data.Height + 1; y < cntY; y++)
        {
            for (int x = 0, cntX = _width - data.Width + 1; x < cntX; x++)
            {
                if (IsSlotAvailable(x, y, data.Width, data.Height))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }


    private bool IsSlotAvailable(int startX, int startY, int width, int height)
    {
        if (_height < startY + height || _width < startX + width)
            return false;

        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (_slots[y, x].Item != null)
                    return false;
            }
        }

        return true;
    }


    private void AddCount(string id, int amount)
    {
        if (_itemCountDic.ContainsKey(id))
        {
            _itemCountDic[id] += amount;
            return;
        }

        _itemCountDic.Add(id, amount);
    }

    private void SubCount(string id, int amount)
    {
        if (_itemCountDic.ContainsKey(id))
        {
            _itemCountDic[id] -= amount;
            return;
        }

        _itemCountDic.Add(id, -amount);
    }
}