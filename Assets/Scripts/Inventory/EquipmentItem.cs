using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    public EquipmentItemData EquipmentItemData { get; private set; }
    public EquipmentItem(EquipmentItemData data, int amount = 1) : base(data, amount)
    {
        EquipmentItemData = data;
        Amount = amount;
    }
    public EquipmentItem Clone()
    {
        return new EquipmentItem(EquipmentItemData);
    }
}
