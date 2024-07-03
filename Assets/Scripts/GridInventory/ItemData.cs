using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public Sprite Sprite;
    public int Id;
    public string Name;
    public int Width;
    public int Height;


    public Item CreateItem()
    {
        return new Item(this);
    }

}

public class Item
{
    private ItemData _itemData;
    public ItemData ItemData => _itemData;

    public Item(ItemData itemData)
    {
        _itemData = itemData;
    }

}
