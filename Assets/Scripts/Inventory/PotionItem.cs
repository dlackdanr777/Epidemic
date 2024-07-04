
public class PotionItem : UsableItem
{
    /// <summary>�������� ������ ���� ��ũ���ͺ� ������Ʈ </summary>
    public PotionItemData PotionData { get; }


    public PotionItem(PotionItemData Data) : base(Data)
    {
        PotionData = Data;
    }


    public override bool Use() //�� �������� ����ϸ� ����� �Լ�
    {
        GameManager.Instance.Player.Inventory.RemoveItem(this);
        GameManager.Instance.Player.RecoverHp(Data.Name, PotionData.Value);
        UIManager.Instance.ShowCenterText(Data.Name + "��(��) ����߽��ϴ�.");
        return true;
    }

}
