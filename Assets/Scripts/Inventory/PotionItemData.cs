using UnityEngine;

[CreateAssetMenu(fileName = "Potion Item Data", menuName = "Items/Usable Item/Potion Item Data")]
public class PotionItemData : ItemData
{
    public int Value => _value;

    [SerializeField] protected int _value;

    public override Item CreateItem()
    {
        return new PotionItem(this);
    }
}
