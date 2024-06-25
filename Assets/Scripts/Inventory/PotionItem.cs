
public class PotionItem : CountableItem, IUsableItem
{
    /// <summary>아이템의 정보를 가진 스크립터블 오브젝트 </summary>
    public PotionItemData PotionData { get; }


    public PotionItem(PotionItemData Data, int Amount = 1) : base(Data, Amount)
    {
        PotionData = Data;
    }


    /// <summary>현재 아이템의 정보를 가진 복제본을 반환하는 함수 </summary>
    public override CountableItem Clone()
    {
        return new PotionItem(CountableData as PotionItemData);
    }


    public bool Use() //이 아이템을 사용하면 실행될 함수
    {
        GameManager.Instance.Player.Inventory.SubItem(this, 1);
        GameManager.Instance.Player.RecoverHp(Data.Name, PotionData.Value);
        UIManager.Instance.ShowCenterText(Data.Name + "를(을) 사용했습니다.");
        return true;
    }

}
