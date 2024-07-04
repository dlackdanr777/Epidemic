using System;
using System.Collections.Generic;
using UnityEngine;

public class InvenData
{
    public event Action OnUpdateHandler;

    private string _name;
    public string Name => _name;

    private GridSlot[,] _slots;
    public GridSlot[,] Slots => _slots;

    private Dictionary<string, int> _itemCountDic = new Dictionary<string, int>();

    private int _width;
    private int _height;

    public InvenData(string name, int slotWidth, int slotHeight)
    {
        _name = name;
        _width = slotWidth;
        _height = slotHeight;
        _slots = new GridSlot[slotHeight, slotWidth];

        for(int i = 0; i < _height; ++i)
        {
            for(int j = 0; j < _width; ++j)
            {
                _slots[i, j] = new GridSlot();
            }
        }
    }


    public bool AddItem(ItemData data, int indexX, int indexY)
    {
        if (!IsSlotAvailable(indexX, indexY, data.Width, data.Height))
        {
            Debug.LogError("추가 할 수 없는 슬롯입니다.");
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

        AddCount(data.ID, 1);
        return true;
    }


    public bool AddItem(ItemData data)
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

        AddCount(data.ID, 1);
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

        SubCount(item.Data.ID, 1);
    }

    public void RemoveItems(ItemData data, int amount)
    {
        SubCount(data.ID, amount);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (amount <= 0)
                    break;

                if (_slots[y, x].Item.Data.ID != data.ID)
                    continue;

                for (int i = y, cntI = y + data.Height - 1; i < cntI; ++i)
                {
                    for (int j = x, cntJ = x + data.Width - 1; j < cntJ; ++j)
                    {
                        _slots[y, x].Item = null;
                        _slots[y, x].IsMainSlot = false;
                    }
                }

                --amount;
            }
        }
    }

    public void RemoveItem(ItemData data, int startX, int startY)
    {
        int width = data.Width;
        int height = data.Height;
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

        AddCount(data.ID, 1);
    }

    public int GetItemCount(ItemData data)
    {
        if(_itemCountDic.TryGetValue(data.ID, out var count))
        {
            return count < 0 ? 0 : count;
        }

        return 0;
    }


    private Vector2Int GetNullSlotIndex(ItemData data)
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
        if(_itemCountDic.ContainsKey(id))
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

public static class UserInfo
{
    private static readonly int MAX_BULLET_COUNT = 1000;

    private static int _bulletCount;
    public static int BulletCount => _bulletCount;

    private static List<InvenData> _invenDataList = new List<InvenData>();
    private static Dictionary<string, InvenData> _invenDataDic = new Dictionary<string, InvenData>();


    public static void AddBulletCount(int value)
    {
        _bulletCount = Mathf.Clamp(_bulletCount + value, 0, MAX_BULLET_COUNT);
    }


    public static void AddInven(string name, int slotWidth, int slotHeight)
    {
        if(_invenDataDic.ContainsKey(name))
        {
            DebugLog.LogError("이미 추가된 인벤입니다.");
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

        DebugLog.LogError("존재하지 않는 인벤토리 이름 입니다." + name);
        return null;
    }
}
