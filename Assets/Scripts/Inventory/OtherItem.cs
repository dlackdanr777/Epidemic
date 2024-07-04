
public class OtherItem : Item
{

    public OtherItemData OtherItemData { get; private set; }

    public OtherItem(OtherItemData Data) : base(Data)
    {
        OtherItemData = Data;
    }

    public OtherItem Clone()
    {
        return new OtherItem(OtherItemData);
    }
}
