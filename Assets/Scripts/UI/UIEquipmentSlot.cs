using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EquipItemType _equipType;
    public EquipItemType Type => _equipType;

    [SerializeField] private Image _slotImage;
    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransfrom => _rectTransform;

    private UIDragSlot _dragSlot;

    private Color _tmpColor;
    private Color _redColor;
    private Color _greenColor;


    public void Init(UIDragSlot dragSlot, Action<EquipItemType, UIEquipmentSlot> initEvent = null)
    {
        _dragSlot = dragSlot;

        _tmpColor = _slotImage.color;
        _redColor = Color.red;
        _greenColor = Color.green;
        _redColor.a = _tmpColor.a;
        _greenColor.a = _tmpColor.a;

        initEvent?.Invoke(_equipType, this);
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        if (_dragSlot.CurrentItem is EquipmentItem)
        {
            EquipmentItem item = (EquipmentItem)_dragSlot.CurrentItem;

            if(item.EquipmentItemData.Type == _equipType)
            {
                EquipmentItem _equipItem = UserInfo.GetEquipItem(_equipType);
                if(_equipItem != null)
                {
                    UserInfo.ChangeEquipItem(item);
                    _dragSlot.RemoveItemByInven();
                    _dragSlot.AddItem(_equipItem.Data);
                }
                else
                {
                    UserInfo.ChangeEquipItem(item);
                    _dragSlot.RemoveItemByInven();
                }
            }
        }

        _slotImage.color = _tmpColor;
        _dragSlot.Disabled();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        if(!(_dragSlot.CurrentItem is EquipmentItem))
        {
            _slotImage.color = _redColor;
            return;
        }

        EquipmentItem equipItem = (EquipmentItem)_dragSlot.CurrentItem;
        _slotImage.color = equipItem.EquipmentItemData.Type == _equipType ? _greenColor : _redColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        _slotImage.color = _tmpColor;
    }

}
