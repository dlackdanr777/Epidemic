
public class OtherItem : CountableItem
{
    public OtherItem(OtherItemData Data, int Amount = 1) : base(Data, Amount) { }

    public override CountableItem Clone()
    {
        return new OtherItem(CountableData as OtherItemData);
    }

}
