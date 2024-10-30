using UnityEngine;


/// <summary>����� �������� ����, ����� ������ �ִ� Ŭ����</summary>
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
            _itemName = "�Ѿ�(" + _itemId.Substring(6) + ")";
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
                UIManager.Instance.ShowCenterText(_itemName + "��(��) ȹ���Ͽ����ϴ�.");
            }
            else
            {
                UIManager.Instance.ShowCenterText("���� ������ źȯ�� ������ �ʰ��߽��ϴ�.");
                return;
            }
        }
        else
        {
            if (!GameManager.Instance.Player.Inventory.AddItem(_itemId))
            {
                UIManager.Instance.ShowCenterText("�κ��丮�� ���� á���ϴ�.");
                return;
            }
        }

        UIManager.Instance.ShowCenterText(_itemName + "��(��) ȹ���Ͽ����ϴ�.");

        ObjectPoolManager.Instance.DespawnDropItem(this);
    }
}
