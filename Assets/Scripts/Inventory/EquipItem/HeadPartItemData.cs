using UnityEngine;


[CreateAssetMenu(fileName = "HeadPartItemData", menuName = "Items/EquipItem/HeadPartItemData")]
public class HeadPartItemData : EquipmentItemData, IPlayerChangeArmor
{
    [Range(0, 100)][SerializeField] private int _addArmor;
    public int Armor => _addArmor;

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
