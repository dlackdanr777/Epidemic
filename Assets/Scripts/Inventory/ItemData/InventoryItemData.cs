using UnityEngine;

public abstract class InventoryItemData : BasicData
{
    [Range(1, 5)] [SerializeField] protected int _width = 1;
    public int Width => _width;
    [Range(1, 5)][SerializeField] protected int _height = 1;
    public int Height => _height;

    public abstract Item CreateItem();

}
