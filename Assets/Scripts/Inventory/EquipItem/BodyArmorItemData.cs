using UnityEngine;


[CreateAssetMenu(fileName = "BodyArmorItemData", menuName = "Items/EquipItem/BodyArmorItemData")]
public class BodyArmorItemData : EquipmentItemData, IPlayerChangeArmor, IPlayerChangeSpeed
{
    [Range(0, 100)][SerializeField] private int _addArmor;
    public int Armor => _addArmor;

    [Range(-1f, 1f)][SerializeField] private float _addSpeed;
    public float Speed => _addSpeed;


    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
