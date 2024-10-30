using UnityEngine;


/// <summary>드랍된 아이템의 정보, 기능을 가지고 있는 클래스</summary>
public class DropItem : MonoBehaviour, Iinteractive
{
    [Header("Option")]
    [SerializeField] private string _itemId;
    public string ItemId => _itemId;

    private string _itemName;
    public KeyCode InputKey => KeyCode.E;

    public void Start()
    {
        if (_itemId.StartsWith("BULLET"))
        {
            _itemName = "총알(" + _itemId.Substring(6) + ")";
            return;
        }

        _itemName = ItemManager.Instance.GetItemByID(_itemId).Data.Name;
    }


    public void DisableInteraction()
    {
        UIManager.Instance.HiddenRightText();
    }


    public void EnableInteraction()
    {
        UIManager.Instance.ShowRightText("[E] " + _itemName);
    }


    public void Interact()
    {
        if (_itemId.StartsWith("BULLET"))
        {
            int bulletCnt = int.Parse(_itemId.Substring(6));
            if (UserInfo.AddBulletCount(bulletCnt))
            {
                UIManager.Instance.ShowCenterText(_itemName + "을(를) 획득하였습니다.");
            }
            else
            {
                UIManager.Instance.ShowCenterText("소지 가능한 탄환의 수량을 초과했습니다.");
                return;
            }
        }
        else
        {
            if (!GameManager.Instance.Player.Inventory.AddItem(_itemId))
            {
                UIManager.Instance.ShowCenterText("인벤토리가 가득 찼습니다.");
                return;
            }
        }

        UIManager.Instance.ShowCenterText(_itemName + "을(를) 획득하였습니다.");

        ObjectPoolManager.Instance.DespawnDropItem(this);
    }
}
