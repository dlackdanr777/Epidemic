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
    public float PlayerRotationW;
    public float PlayerRotationX;
    public float PlayerRotationY;
    public float PlayerRotationZ;

    public float CameraMouseInputX;
    public float CameraMouseInputY;

    public int BulletCount;
    public int LoadBulletCount;
    public float CurrentHp;
    public List<SaveInvenData> InvenDataList = new List<SaveInvenData>();
    public List<string> EquipItemDataList = new List<string>();

    public SaveData(Player player, int bulletCount, int loadBulletCount, List<InvenData> invenDataList, EquipmentItem[] equipItems)
    {
        PlayerPositionX = player.transform.position.x;
        PlayerPositionY = player.transform.position.y;
        PlayerPositionZ = player.transform.position.z;
        PlayerRotationW = player.transform.rotation.w;
        PlayerRotationX = player.transform.rotation.x;
        PlayerRotationY = player.transform.rotation.y;
        PlayerRotationZ = player.transform.rotation.z;

        Vector2 mouseInput = player.PlayerCamera.GetMouseInput;
        CameraMouseInputX = mouseInput.x;
        CameraMouseInputY = mouseInput.y;

        BulletCount = bulletCount;
        LoadBulletCount = loadBulletCount;
        CurrentHp = player.Hp;
        
        for(int i = 0, cnt = equipItems.Length; i < cnt; ++i)
        {
            if (equipItems[i] == null)
                continue;

            EquipItemDataList.Add(equipItems[i].Data.ID);
        }

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
