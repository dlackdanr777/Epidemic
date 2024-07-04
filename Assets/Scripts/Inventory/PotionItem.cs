
public class PotionItem : UsableItem
{
    /// <summary>아이템의 정보를 가진 스크립터블 오브젝트 </summary>
    public PotionItemData PotionData { get; }


    public PotionItem(PotionItemData Data) : base(Data)
    {
        PotionData = Data;
    }


    public override bool Use() //이 아이템을 사용하면 실행될 함수
    {
        GameManager.Instance.Player.Inventory.RemoveItem(this);
        GameManager.Instance.Player.RecoverHp(Data.Name, PotionData.Value);
        UIManager.Instance.ShowCenterText(Data.Name + "를(을) 사용했습니다.");
        return true;
    }

}
