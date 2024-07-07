using UnityEngine;


[CreateAssetMenu(fileName = "UpperBodyItemData", menuName = "Items/EquipItem/UpperBodyItemData")]
public class UpperBodyItemData : EquipmentItemData
{
    [Range(0, 100)][SerializeField] private int _addArmor;
    [Range(-1f, 1f)][SerializeField] private float _addSpeedMul;

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }

    public override void Equip()
    {
        UserInfo.AddArmor(_addArmor);
        UserInfo.AddSpeedMul(_addSpeedMul);
    }

    public override void Dequip()
    {
        UserInfo.AddArmor(-_addArmor);
        UserInfo.AddSpeedMul(-_addSpeedMul);
    }


}
