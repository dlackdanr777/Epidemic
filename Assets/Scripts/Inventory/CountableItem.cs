
public class CountableItem : Item
{
    public int Amonut;

    public CountableItemData CountableItemData { get; private set; }

    public CountableItem(CountableItemData Data) : base(Data)
    {
        CountableItemData = Data;
    }
}
