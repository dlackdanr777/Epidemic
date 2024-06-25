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
        return new PotionItem(Clone());
    }

    private PotionItemData Clone()
    {
        PotionItemData data = new PotionItemData();
        data._value = _value;
        data._id = _id;
        data._amount = _amount;
        data._maxAmount = _maxAmount;
        data._name = _name;
        data._description = _description;
        data._icon = _icon;
        return data;
    }

}
