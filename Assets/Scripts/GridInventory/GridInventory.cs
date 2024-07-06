using System;
using UnityEngine;

public class GridInventory : MonoBehaviour
{
    public event Action OnUpdateHandler;

    [SerializeField] private int _gridWidth = 8;
    [SerializeField] private int _gridHight = 10;

    [SerializeField] private string _invenName;

    public int GridWidth =>  _gridWidth;
    public int GridHight => _gridHight;

    private InvenData _invenData;
    public GridSlot[,] GridSlots => _invenData.Slots;

    [SerializeField] private ItemData _item;
    [SerializeField] private ItemData _item2;

    private void Awake()
    {
        UserInfo.AddInven(_invenName, _gridWidth, _gridHight);
        _invenData = UserInfo.GetInvenData(_invenName);

        AddItem(_item);
        AddItem(_item);
        AddItem(_item);
        AddItem(_item);
        AddItem(_item2);
        AddItem(_item);
    }


    public bool AddItem(ItemData data)
    {
        bool enabled = _invenData.AddItem(data);

        if(enabled)
            OnUpdateHandler?.Invoke();

        return enabled;
    }


    public bool AddItem(string id)
    {
        Item item = ItemManager.Instance.GetItemByID(id);

        bool enabled = _invenData.AddItem(item.Data);

        if (enabled)
            OnUpdateHandler?.Invoke();

        return enabled;
    }

    public bool AddItem(ItemData data, int indexX, int indexY)
    {
        bool enabled = _invenData.AddItem(data, indexX, indexY);

        if (enabled)
            OnUpdateHandler?.Invoke();

        return enabled;
    }


    public void RemoveItem(Item item)
    {
        _invenData.RemoveItem(item);
        OnUpdateHandler?.Invoke();
    }


    public void RemoveItem(ItemData item, int startX, int startY)
    {
        _invenData.RemoveItem(item, startX, startY);
        OnUpdateHandler?.Invoke();
    }

    public bool UseItemByID(string id, int amount)
    {
        if (amount <= 0)
        {
            DebugLog.LogError("������ ���� �Դϴ�." + amount);
            return false;
        }

        ItemData data = ItemManager.Instance.GetItemDataByID(id);

        if(data == null)
        {
            DebugLog.LogError("�������� �ʴ� ������ id �Դϴ�." + id);
            return false;
        }

        int cnt = _invenData.GetItemCount(data);
        if (cnt < amount)
        {
            DebugLog.LogError("�������� ������ �����ϴ�.[" + id + "]  ���� ����: " + cnt + "  �ʿ� ����" + amount);
            return false;
        }

        _invenData.RemoveItems(data, amount);
        OnUpdateHandler?.Invoke();
        return true;
    }


    public int GetItemCountByID(string id)
    {
        ItemData data = ItemManager.Instance.GetItemDataByID(id);

        if (data == null)
        {
            DebugLog.LogError("�������� �ʴ� ������ id �Դϴ�." + id);
            return -1;
        }

        return _invenData.GetItemCount(data);
    }
}