
public abstract class CountableItem : Item
{

    public CountableItemData CountableData { get; private set; }

    public CountableItem(CountableItemData Data, int Amount = 1) : base(Data, Amount)
    {
        CountableData = Data;
    }

    public abstract CountableItem Clone();
}
