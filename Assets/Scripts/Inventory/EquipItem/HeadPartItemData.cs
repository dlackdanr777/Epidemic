using UnityEngine;


[CreateAssetMenu(fileName = "HeadPartItemData", menuName = "Items/EquipItem/HeadPartItemData")]
public class HeadPartItemData : EquipmentItemData
{
    [Range(0, 100)][SerializeField] private int _addArmor;

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }

    public override void Equip()
    {
        UserInfo.AddArmor(_addArmor);
    }

    public override void Dequip()
    {
        UserInfo.AddArmor(-_addArmor);
    }


}
