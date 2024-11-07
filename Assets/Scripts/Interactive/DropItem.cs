using UnityEngine;


/// <summary>����� �������� ����, ����� ������ �ִ� Ŭ����</summary>
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
            _itemName = "źȯ";
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
                UIManager.Instance.ShowCenterText("���� ������ źȯ�� ������ �ʰ��߽��ϴ�.");
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
                UIManager.Instance.ShowCenterText(_itemName + cntStr + " ��(��) ȹ���Ͽ����ϴ�.");
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
                    UIManager.Instance.ShowCenterText("�κ��丮�� ���� á���ϴ�.");
                    return;
                }

                _count -= i;
            }

            UIManager.Instance.ShowCenterText(_itemName + cntStr + " ��(��) ȹ���Ͽ����ϴ�.");
            ObjectPoolManager.Instance.DespawnDropItem(this);
            return;
        }
    }
}
