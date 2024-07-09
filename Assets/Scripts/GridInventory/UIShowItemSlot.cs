using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShowItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private UIGridInventory _uiInven;
    public UIGridInventory UIInven => _uiInven;
    private UIDragSlot _dragSlot;
    private UIItemDescription _itemDescription;

    private Vector2 _dragOffset;

    private GridSlot _item;
    public GridSlot Item => _item;

    private int _indexX;
    public int IndexX => _indexX;
    private int _indexY;
    public int IndexY => _indexY;
    private float _slotSizeX;
    private float _slotSizeY;




    public void Init(UIGridInventory uiInven, UIDragSlot dragSlot, UIItemDescription itemDescription, float slotSizeX, float slotSizeY)
    {
        _canvas = GetComponentInParent<Canvas>();
        _uiInven = uiInven;
        _dragSlot = dragSlot;
        _itemDescription = itemDescription;
        _slotSizeX = slotSizeX;
        _slotSizeY = slotSizeY;
    }

    public void SetPos(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }

    public void SetItem(GridSlot item, int indexX, int indexY, float sizeX, float sizeY)
    {
        _item = item;
        _indexX = indexX;
        _indexY = indexY;

        _rectTransform.sizeDelta = new Vector2(sizeX * item.Item.Data.Width, sizeY * item.Item.Data.Height);
        _image.sprite = item.Item.Data.Sprite;
        EndDrag();
    }

    public void StartDrag()
    {
        _canvasGroup.blocksRaycasts = false;
    }

    public void EndDrag()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;
       _dragOffset = (new Vector2(_rectTransform.sizeDelta.x, -_rectTransform.sizeDelta.y) * 0.5f) + new Vector2(-_slotSizeX * 0.5f, _slotSizeY * 0.5f);
        _dragSlot.Enabled(_item.Item, _uiInven);
        UIGridInventory.StartDragHandler?.Invoke();
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
        UIGridInventory.EndDragHandler?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        _uiInven.SlotRightClicked(_item.Item);


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 pos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        pos += new Vector2(_rectTransform.sizeDelta.x * 0.5f, _rectTransform.sizeDelta.y * 0.5f);
        _itemDescription.UpdateUI(_item.Item, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _itemDescription.End();
    }
}