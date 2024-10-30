using UnityEngine;


[CreateAssetMenu(fileName = "BootsItemData", menuName = "Items/EquipItem/BootsItemData")]
public class BootsItemData : EquipmentItemData, IPlayerChangeSpeed
{
    [Range(-1f, 1f)][SerializeField] private float _addSpeed;
    public float Speed => _addSpeed;


    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
