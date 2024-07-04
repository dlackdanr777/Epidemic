

using UnityEngine;

[CreateAssetMenu(fileName = "Other Item Data", menuName = "Items/OtherItem/Other Item Data")]
public class OtherItemData : ItemData
{

    public override Item CreateItem()
    {
        return new OtherItem(this);
    }
}

