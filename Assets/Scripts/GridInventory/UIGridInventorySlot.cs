using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIGridInventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;

    private UIGridInventory _uiInven;
    private UIDragSlot _dragSlot;

    private Color _tmpColor;
    private Color _redColor;
    private Color _greenColor;

    private int _indexX;
    private int _indexY;


    public void Init(UIGridInventory uiInven, UIDragSlot dragSlot, int indexX, int indexY)
    {
        _uiInven = uiInven;
        _dragSlot = dragSlot;
        _indexX = indexX;
        _indexY = indexY;

        _tmpColor = _image.color;
        _redColor = Color.red;
        _greenColor = Color.green;
        _redColor.a = _tmpColor.a;
        _greenColor.a = _tmpColor.a;
    }

    public void SetColor(bool dropEnabled)
    {
        _image.color = dropEnabled ? _greenColor : _redColor;
    }


    public void ResetColor()
    {
        _image.color = _tmpColor;
    }

    public Vector2 GetAnchoredPosition()
    {
        return _rectTransform.anchoredPosition;
    }

    public void SetAnchoredPosition(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        _uiInven.ResetSlotEffect(_indexX, _indexY, _dragSlot.CurrentItem.Item.Data.Width, _dragSlot.CurrentItem.Item.Data.Height);


        if(_uiInven.ItemMoveEnabled(_indexX, _indexY, _dragSlot.CurrentItem.Item))
        {
            ItemData data = _dragSlot.CurrentItem.Item.Data;
            _dragSlot.CurrentUIInven.Inven.RemoveItem(data, _dragSlot.IndexX, _dragSlot.IndexY);
            _uiInven.Inven.AddItem(data, _indexX, _indexY);
        }

        _dragSlot.Disabled();
        UIGridInventory.EndDragHandler?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        _uiInven.ChangeSlotEffect(_indexX, _indexY, _dragSlot.CurrentItem.Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_dragSlot.CurrentItem == null)
            return;

        _uiInven.ResetSlotEffect(_indexX, _indexY, _dragSlot.CurrentItem.Item.Data.Width, _dragSlot.CurrentItem.Item.Data.Height);
    }
}
