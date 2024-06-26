
using System;

[Serializable]
public class Item
{
    public int Amount;
    public ItemData Data { get; private set; }

    public Item(ItemData data, int amount = 1)
    {
        Data = data;
        Amount = amount;
    }
}
