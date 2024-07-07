using UnityEngine;

public abstract class EquipmentItemData : ItemData, IEquipmentItem
{
    [Space]
    [Header("장비 아이템 정보")]
    [SerializeField] private EquipItemType _type;
    public EquipItemType Type => _type;


    public abstract void Equip();

    public abstract void Dequip();
}
