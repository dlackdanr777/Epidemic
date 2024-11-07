using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public GameStateSaveData GameStateSaveData;
    public float PlayerPositionX;
    public float PlayerPositionY;
    public float PlayerPositionZ;
    public float PlayerRotationW;
    public float PlayerRotationX;
    public float PlayerRotationY;
    public float PlayerRotationZ;
    public Vector3 PlayerPosition => new Vector3(PlayerPositionX, PlayerPositionY, PlayerPositionZ);
    public Quaternion PlayerRotation => new Quaternion(PlayerRotationX, PlayerRotationY, PlayerRotationZ, PlayerRotationW);

    public float CameraMouseInputX;
    public float CameraMouseInputY;
    public Vector2 MouseInput => new Vector2(CameraMouseInputX, CameraMouseInputY);

    public int BulletCount;
    public int LoadBulletCount;
    public float CurrentHp;
    public List<SaveInvenData> InvenDataList = new List<SaveInvenData>();
    public List<string> EquipItemDataList = new List<string>();
    public List<SaveEnemyData> EnemyDataList = new List<SaveEnemyData>();
    public List<SaveDropItemData> DropItemDataList = new List<SaveDropItemData>();
    public List<SaveDoorData> DoorDataList = new List<SaveDoorData>();
    public List<SaveBuildObjectData> BuildObjectDataList = new List<SaveBuildObjectData>();

    public SaveData(GameStateSaveData gameStateSaveData, Player player, int bulletCount, int loadBulletCount, List<InvenData> invenDataList, EquipmentItem[] equipItems, List<Enemy> enemyList, List<DropItem> dropItemList, List<Door> doorList, List<BuildObject> buildObjectList)
    {
        GameStateSaveData = gameStateSaveData;

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

            EquipItemDataList.Add(equipItems[i].Data.Id);
        }

        InvenDataList.Clear();
        for(int i = 0, cnt =  invenDataList.Count; i < cnt; ++i)
        {
            InvenDataList.Add(new SaveInvenData(invenDataList[i]));
        }

        EnemyDataList.Clear();
        for(int i = 0, cnt = enemyList.Count; i < cnt; ++i)
        {
            EnemyDataList.Add(new SaveEnemyData(enemyList[i]));
        }

        DropItemDataList.Clear();
        for(int i = 0, cnt = dropItemList.Count; i < cnt; ++i)
        {
            DropItemDataList.Add(new SaveDropItemData(dropItemList[i]));
        }

        DoorDataList.Clear();
        for(int i = 0, cnt = doorList.Count; i < cnt; ++i)
        {
            DoorDataList.Add(new SaveDoorData(doorList[i]));
        }

        BuildObjectDataList.Clear();
        for(int i = 0, cnt = buildObjectList.Count; i < cnt; ++i)
        {
            BuildObjectDataList.Add(new SaveBuildObjectData(buildObjectList[i]));
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

        ItemId = slot.Item.Data.Id;
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


[Serializable]
public class SaveEnemyData
{
    public float Hp;
    public float EnemyPositionX;
    public float EnemyPositionY;
    public float EnemyPositionZ;
    public float EnemyRotationX;
    public float EnemyRotationY;
    public float EnemyRotationZ;
    public float EnemyRotationW;

    public Vector3 EnemyPosition => new Vector3(EnemyPositionX, EnemyPositionY, EnemyPositionZ);
    public Quaternion EnemyRotation => new Quaternion(EnemyRotationX, EnemyRotationY, EnemyRotationZ, EnemyRotationW);

    public SaveEnemyData(Enemy enemy)
    {
        Hp = enemy.Hp;

        Vector3 pos = enemy.transform.position;
        Quaternion rot = enemy.transform.rotation;

        EnemyPositionX = pos.x;
        EnemyPositionY = pos.y;
        EnemyPositionZ = pos.z;

        EnemyRotationX = rot.x;
        EnemyRotationY = rot.y;
        EnemyRotationZ = rot.z;
        EnemyRotationW = rot.w;
    }
}


[Serializable]
public class SaveDropItemData
{
    public string ItemId;
    public int Count;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public float RotationW;

    public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);
    public Quaternion Rotation => new Quaternion(RotationX, RotationY, RotationZ, RotationW);

    public SaveDropItemData(DropItem dropItem)
    {
        ItemId = dropItem.ItemId;
        Count = dropItem.Count;
        Vector3 pos = dropItem.transform.position;
        Quaternion rot = dropItem.transform.rotation;

        PositionX = pos.x;
        PositionY = pos.y;
        PositionZ = pos.z;

        RotationX = rot.x;
        RotationY = rot.y;
        RotationZ = rot.z;
        RotationW = rot.w;
    }
}

[Serializable]
public class SaveDoorData
{
    public bool IsOpened;
    public float DoorHp;

    public SaveDoorData(Door door)
    {
        IsOpened = door.IsOpend;
        DoorHp = door.Barricade == null ? 0 : door.Barricade.Hp;
    }
}


[Serializable]
public class SaveBuildObjectData
{
    public string Id;
    public float Hp;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public float RotationW;

    public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);
    public Quaternion Rotation => new Quaternion(RotationX, RotationY, RotationZ, RotationW);

    public SaveBuildObjectData(BuildObject buildObject)
    {
        Id = buildObject.BuildData.Id;
        Hp = buildObject.Hp;

        Vector3 pos = buildObject.transform.position;
        Quaternion rot = buildObject.transform.rotation;

        PositionX = pos.x;
        PositionY = pos.y;
        PositionZ = pos.z;

        RotationX = rot.x;
        RotationY = rot.y;
        RotationZ = rot.z;
        RotationW = rot.w;
    }
}


[Serializable]
public class GameStateSaveData
{
    public int GameState;
    public int RoundState;
    public int CurrentRound;
    public float CurrentTime;

    public GameStateSaveData(int gameState, int roundState, int currentRound, float currentTime)
    {
        GameState = gameState;
        RoundState = roundState;
        CurrentRound = currentRound;
        CurrentTime = currentTime;
    }
}


