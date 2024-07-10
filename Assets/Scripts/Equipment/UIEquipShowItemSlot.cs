using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipShowItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private UIItemDescription _itemDescription;
    private UIDragSlot _dragSlot;

    private EquipmentItem _item;
    public EquipmentItem Item => _item;

    private Vector2 _dragOffset;


    public void Init(UIItemDescription itemDescription, UIDragSlot dragSlot)
    {
        _canvas = GetComponentInParent<Canvas>();
        _itemDescription = itemDescription;
        _dragSlot = dragSlot;

    }


    public void SetItem(EquipmentItem item, Vector2 pos, Vector2 sizeDelta)
    {
        _item = item;
        _nameText.text = item.Data.Name;
        _rectTransform.sizeDelta = sizeDelta;
        _rectTransform.anchoredPosition = pos;
        _image.sprite = Item.Data.Sprite;
        _canvasGroup.alpha = 1;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;
        _dragOffset = (new Vector2(_rectTransform.sizeDelta.x, -_rectTransform.sizeDelta.y) * 0.5f) + new Vector2(-50, 50);
        _dragSlot.Enabled(_item.EquipmentItemData.Type);
    }


    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        _dragSlot.SetPos(localPoint + _dragOffset);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.interactable = true;
        _canvasGroup.alpha = 1;
        _dragSlot.Disabled();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (_item == null)
            return;

        if (!GameManager.Instance.Player.Inventory.AddItem(Item.Data))
            return;

        UserInfo.NullEquipItem(_item.EquipmentItemData.Type);
        _itemDescription.End();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 pos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        pos += new Vector2(_rectTransform.sizeDelta.x * 0.5f, _rectTransform.sizeDelta.y * 0.5f);
        _itemDescription.UpdateUI(Item, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _itemDescription.End();
    }
}