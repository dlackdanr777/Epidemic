using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EquipItemType _equipType;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _slotImage;
    [SerializeField] private TextMeshProUGUI _text;
    
    
    private UIDragSlot _dragSlot;
    private EquipmentItem _currentItem;

    private Color _tmpColor;
    private Color _redColor;
    private Color _greenColor;


    public void Init(UIDragSlot dragSlot)
    {
        _dragSlot = dragSlot;

        _tmpColor = _slotImage.color;
        _redColor = Color.red;
        _greenColor = Color.green;
        _redColor.a = _tmpColor.a;
        _greenColor.a = _tmpColor.a;
    }

    public void UpdateUI()
    {
        EquipmentItem item = UserInfo.GetEquipItem(_equipType);

        if (item == null)
        {
            _itemImage.gameObject.SetActive(false);
            _text.text = string.Empty;
            return;
        }

        _itemImage.gameObject.SetActive(true);
        _itemImage.sprite = item.Data.Sprite;
        _text.text = item.Data.Name;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        DebugLog.Log("드랍");
        if (_dragSlot.CurrentItem.Item is EquipmentItem)
        {
            EquipmentItem item = (EquipmentItem)_dragSlot.CurrentItem.Item;

            if(item.EquipmentItemData.Type == _equipType)
            {
                UserInfo.ChangeEquipItem(item);
            }
        }

        _slotImage.color = _tmpColor;
        _dragSlot.Disabled();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        DebugLog.Log("엔터");

        if(!(_dragSlot.CurrentItem.Item is EquipmentItem))
        {
            _slotImage.color = _redColor;
            return;
        }

        EquipmentItem equipItem = (EquipmentItem)_dragSlot.CurrentItem.Item;

        _slotImage.color = equipItem.EquipmentItemData.Type == _equipType ? _greenColor : _redColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        DebugLog.Log("나가기");
        _slotImage.color = _tmpColor;
    }

}
