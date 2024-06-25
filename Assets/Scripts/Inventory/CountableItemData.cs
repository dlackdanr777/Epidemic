using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    public int MaxAmount => _maxAmount;
    public int _amount;
    [SerializeField] protected int _maxAmount = 99;
}

