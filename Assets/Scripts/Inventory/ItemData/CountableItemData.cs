using UnityEngine;

public abstract class CountableItemData : InventoryItemData
{
    [SerializeField] private int _maxCount;
    public int MaxAmount => _maxCount;
}

