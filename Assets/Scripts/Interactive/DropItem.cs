using UnityEngine;


/// <summary>드랍된 아이템의 정보, 기능을 가지고 있는 클래스</summary>
public class DropItem : MonoBehaviour, Iinteractive
{
    [Header("Option")]
    [SerializeField] private int _itemId;
    [SerializeField] private int _amount;

    private string _itemName;
    public KeyCode InputKey => KeyCode.E;

    public void Start()
    {
        _itemName = ItemManager.Instance.GetItemByID(_itemId).Data.Name;
    }


    public void DisableInteraction()
    {
        UIManager.Instance.HiddenRightText();
    }


    public void EnableInteraction()
    {
        UIManager.Instance.ShowRightText("[E] "+ _itemName + "x"+ _amount);
    }


    public void Interact()
    {
        GameManager.Instance.Player.Inventory.AddItemByID(_itemId, _amount);
        UIManager.Instance.ShowCenterText(_itemName + "을(를) " + _amount + "개 획득하였습니다.");
        Destroy(gameObject);
    }
}
