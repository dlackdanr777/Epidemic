using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion Item Data", menuName = "Inventory/Items/Countable Item/Usable Item/Potion Item Data")]
public class PotionItemData : CountableItemData
{
    public int Value => _value;

    [SerializeField] protected int _value;

    public override Item CreateItem()
    {
        return new PotionItem(this);
    }
}
