
using System;

[Serializable]
public class Item
{
    public InventoryItemData Data { get; private set; }

    public Item(InventoryItemData data)
    {
        Data = data;
    }

}
