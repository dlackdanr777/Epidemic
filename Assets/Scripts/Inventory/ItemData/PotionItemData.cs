using UnityEngine;

[CreateAssetMenu(fileName = "Potion Item Data", menuName = "Items/Usable Item/Potion Item Data")]
public class PotionItemData : InventoryItemData, IPlayerRecoveryHealth
{
    public int RecoveryValue => _value;

    [SerializeField] protected int _value;

    public override Item CreateItem()
    {
        return new PotionItem(this);
    }
}
