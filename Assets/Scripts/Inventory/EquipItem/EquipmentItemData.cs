using UnityEngine;

public abstract class EquipmentItemData : ItemData, IEquipmentItem
{
    [Space]
    [Header("��� ������ ����")]
    [SerializeField] private EquipItemType _type;
    public EquipItemType Type => _type;


    public abstract void Equip();

    public abstract void Dequip();
}
