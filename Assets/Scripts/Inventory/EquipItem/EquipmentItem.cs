using UnityEngine;

public class EquipmentItem : Item
{
    public EquipmentItemData EquipmentItemData { get; private set; }
    

    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        EquipmentItemData = data;
    }
    public virtual EquipmentItem Clone()
    {
        return new EquipmentItem(EquipmentItemData);
    }
}
