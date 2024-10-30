
public abstract class UsableItem : Item, IUsableItem
{
    protected UsableItem(InventoryItemData data) : base(data) { }

    public abstract bool Use(); //이 아이템을 사용하면 실행될 함수

}
