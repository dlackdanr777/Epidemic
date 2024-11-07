using UnityEngine;


/// <summary>드랍된 아이템의 정보, 기능을 가지고 있는 클래스</summary>
public class DropItem : MonoBehaviour, Iinteractive
{
    [Header("Option")]
    [SerializeField] private string _itemId;
    public string ItemId => _itemId;

    [Range(1, 100)] [SerializeField] private int _count = 1;
    public int Count => _count;

    private string _itemName;
    public KeyCode InputKey => KeyCode.E;

    public void Start()
    {
        if (_itemId.StartsWith("BULLET"))
        {
            _itemName = "탄환";
            return;
        }
        _itemName = ItemManager.Instance.GetItemByID(_itemId).Data.Name;
    }


    public void SetCount(int count)
    {
        _count = count;
    }


    public void DisableInteraction()
    {
        UIManager.Instance.HiddenRightText();
    }


    public void EnableInteraction()
    {
        string cntStr = _count <= 1 ? string.Empty : "(" + _count.ToString() + ")";
        UIManager.Instance.ShowRightText("[E] " + _itemName + cntStr);
    }


    public void Interact()
    {
        string cntStr = _count <= 1 ? string.Empty : "("+ _count.ToString() + ")";
        if (_itemId.StartsWith("BULLET"))
        {
            if (!UserInfo.AddBulletCount(_count))
            {
                UIManager.Instance.ShowCenterText("소지 가능한 탄환의 수량을 초과했습니다.");
                int addCount = UserInfo.GetAddBulletCount();
                if (0 < addCount)
                {
                    UserInfo.AddBulletCount(addCount);
                    _count -= addCount;
                }

                return;
            }
            else
            {
                UIManager.Instance.ShowCenterText(_itemName + cntStr + " 을(를) 획득하였습니다.");
                ObjectPoolManager.Instance.DespawnDropItem(this);
                return;
            }
        }

        else
        {
            for(int i = 0; i < _count; i++)
            {
                if (!GameManager.Instance.Player.Inventory.AddItem(_itemId))
                {
                    UIManager.Instance.ShowCenterText("인벤토리가 가득 찼습니다.");
                    return;
                }

                _count -= i;
            }

            UIManager.Instance.ShowCenterText(_itemName + cntStr + " 을(를) 획득하였습니다.");
            ObjectPoolManager.Instance.DespawnDropItem(this);
            return;
        }
    }
}
