using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipShowItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private UIItemDescription _itemDescription;

    private EquipmentItem _item;
    public EquipmentItem Item => _item;




    public void Init(UIItemDescription itemDescription)
    {
        _canvas = GetComponentInParent<Canvas>();
        _itemDescription = itemDescription;

    }


    public void SetItem(EquipmentItem item, Vector2 pos, float sizeX, float sizeY)
    {
        _item = item;

        _rectTransform.sizeDelta = new Vector2(sizeX * Item.Data.Width, sizeY * Item.Data.Height);
        _rectTransform.anchoredPosition = pos;
        _image.sprite = Item.Data.Sprite;
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