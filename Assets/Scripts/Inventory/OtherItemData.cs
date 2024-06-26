using UnityEngine;


[CreateAssetMenu(fileName = "Other Item Data", menuName = "Inventory/Items/Countable Item/Other Item Data")]
public class OtherItemData : CountableItemData
{
    public override Item CreateItem()
    {
        return new OtherItem(this);
    }
}
