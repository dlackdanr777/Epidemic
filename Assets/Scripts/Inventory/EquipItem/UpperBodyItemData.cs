using UnityEngine;


[CreateAssetMenu(fileName = "UpperBodyItemData", menuName = "Items/EquipItem/UpperBodyItemData")]
public class UpperBodyItemData : EquipmentItemData, IPlayerChangeArmor, IPlayerChangeSpeed
{
    [Range(0, 100)][SerializeField] private int _addArmor;
    public int Armor => _addArmor;

    [Range(-1f, 1f)][SerializeField] private float _addSpeedMul;
    public float Speed => _addSpeedMul;

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
