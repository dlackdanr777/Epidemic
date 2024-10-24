using UnityEngine;


[CreateAssetMenu(fileName = "WeaponItemData", menuName = "Items/EquipItem/WeaponItemData")]
public class WeaponItemData : EquipmentItemData
{

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
