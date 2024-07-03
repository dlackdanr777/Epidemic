using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGridItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private UIGridInventory _uiInven;
    public UIGridInventory UIInven => _uiInven;
    private UIDragSlot _dragSlot;

    private Vector2 _dragOffset;

    private GridSlot _item;
    public GridSlot Item => _item;

    private int _indexX;
    public int IndexX => _indexX;
    private int _indexY;
    public int IndexY => _indexY;
    private float _slotSizeX;
    private float _slotSizeY;



    public void Init(UIGridInventory uiInven, UIDragSlot dragSlot, float slotSizeX, float slotSizeY)
    {
        _canvas = GetComponentInParent<Canvas>();
        _uiInven = uiInven;
        _dragSlot = dragSlot;
        _slotSizeX = slotSizeX;
        _slotSizeY = slotSizeY;
    }

    public void SetPos(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetItem(GridSlot item, int indexX, int indexY, float sizeX, float sizeY)
    {
        _item = item;
        _indexX = indexX;
        _indexY = indexY;

        _rectTransform.sizeDelta = new Vector2(sizeX * item.Item.ItemData.Width, sizeY * item.Item.ItemData.Height);
        _image.sprite = item.Item.ItemData.Sprite;
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
        _dragSlot.Enabled(this);
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
}