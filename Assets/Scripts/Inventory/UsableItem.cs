
public abstract class UsableItem : Item, IUsableItem
{
    protected UsableItem(ItemData data) : base(data) { }

    public abstract bool Use(); //�� �������� ����ϸ� ����� �Լ�

}
