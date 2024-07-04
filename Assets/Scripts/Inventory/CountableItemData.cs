using UnityEngine;

public abstract class CountableItemData : ItemData
{
    [SerializeField] private int _maxCount;
    public int MaxAmount => _maxCount;
}

