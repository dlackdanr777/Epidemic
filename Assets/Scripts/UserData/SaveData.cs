using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[Serializable]
public class SaveData
{
    public float PlayerPositionX;
    public float PlayerPositionY;
    public float PlayerPositionZ;
    public int BulletCount;
    public int LoadBulletCount;
    public int CurrentHp;
    public List<SaveInvenData> InvenDataList = new List<SaveInvenData>();

    public SaveData(Vector3 playerPosition, int bulletCount, int loadBulletCount, int currentHp, List<InvenData> invenDataList)
    {
        PlayerPositionX = playerPosition.x;
        PlayerPositionY = playerPosition.y;
        PlayerPositionZ = playerPosition.z;
        BulletCount = bulletCount;
        LoadBulletCount = loadBulletCount;
        CurrentHp = currentHp;
        InvenDataList.Clear();
        
        for(int i = 0, cnt =  invenDataList.Count; i < cnt; ++i)
        {
            InvenDataList.Add(new SaveInvenData(invenDataList[i]));
        }
    }

}


[Serializable]
public class SaveInvenData
{
    public string Name;
    public int Width;
    public int Height;
    public List<SaveGridSlot> SlotList;


    public SaveInvenData(InvenData invenData)
    {
        Name = invenData.Name;
        Width = invenData.Width; 
        Height = invenData.Height;

        SlotList = new List<SaveGridSlot>();
        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                SlotList.Add(new SaveGridSlot(invenData.Slots[y, x]));
            }
        }
    }

    public InvenData ToOriginalInvenData()
    {
        InvenData invenData = new InvenData(Name, Width, Height);

        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                GridSlot slot = SlotList[y * Width + x].ToOriginalSlot();
                if (slot == null)
                    continue;

                if (!slot.IsMainSlot)
                    continue;

                invenData.AddItem(slot.Item.Data, x, y);
            }
        }

        return invenData;
    }
}


[Serializable]
public class SaveGridSlot
{
    public string ItemId;
    public bool IsMainSlot;

    public SaveGridSlot(GridSlot slot)
    {
        if (slot == null)
            return;

        if (slot.Item == null)
            return;

        if (slot.Item.Data == null)
            return;

        ItemId = slot.Item.Data.ID;
        IsMainSlot = slot.IsMainSlot;
    }

    public GridSlot ToOriginalSlot()
    {
        if (string.IsNullOrWhiteSpace(ItemId))
            return null;

        GridSlot slot = new GridSlot();
        slot.IsMainSlot = IsMainSlot;
        slot.Item = ItemManager.Instance.GetItemByID(ItemId);

        if (slot.Item == null)
            throw new Exception("해당 Id를 가진 아이템이 데이터베이스에 존재하지 않습니다: " + ItemId);

        return slot;
    }
}
