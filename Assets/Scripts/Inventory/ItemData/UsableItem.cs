
public abstract class UsableItem : Item, IUsableItem
{
    protected UsableItem(InventoryItemData data) : base(data) { }

    public abstract bool Use(); //�� �������� ����ϸ� ����� �Լ�

}
