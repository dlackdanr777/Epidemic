using UnityEngine;


[CreateAssetMenu(fileName = "Other Item Data", menuName = "Inventory/Items/Countable Item/Other Item Data")]
public class OtherItemData : CountableItemData
{
    public override Item CreateItem()
    {
        return new OtherItem(Clone());
    }

    private OtherItemData Clone()
    {
        OtherItemData data = new OtherItemData();
        data._id = _id;
        data._amount = _amount;
        data._maxAmount = _maxAmount;
        data._name = _name;
        data._description = _description;
        data._icon = _icon;
        return data;
    }
}
