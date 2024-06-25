using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> 인벤토리 슬롯 클래스 </summary>
public class UIInventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image IconImage;
    public TextMeshProUGUI AmountText;

    [HideInInspector] public Item _item;
    [HideInInspector] public int SlotIndex = -1;

    private UIInventory _uiInventory;
    private RectTransform _rectTransform;

    public void Init(UIInventory uiInventory)
    {
        _uiInventory = uiInventory;
        _rectTransform = GetComponent<RectTransform>();
    }
    public bool GetisNull()
    {
        return _item == null;
    }


    public void UpdateUI(Item item)
    {
        if(item == null)
        {
            IconImage.enabled = false;
            AmountText.enabled = false;
            _item = null;
        }
        else
        {
            _item = item;
            IconImage.enabled = true;
            IconImage.sprite = _item.Data.Icon;
            _item.SlotIndex = SlotIndex;
            if (_item is CountableItem)
            {
                AmountText.enabled = true;
                AmountText.text = _item.Amount.ToString();
            }
            else
            {
                AmountText.enabled = false;
            }

        }
    }


    /// <summary> 슬롯을 우클릭했을 경우 아이템과 상호작용 하는 함수</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) //만약 해당 스크립트가 들어있는 오브젝트를 우클릭 했을경우
            return;

        if (_item == null)
            return;

        if (_item is IUsableItem) // 소비아이템이라면
        {
            IUsableItem uItem = _item as IUsableItem;
            uItem.Use();
        }

        else if (_item is CountableItem)
        {
            _uiInventory.UiDivItem.SetActive(_item);
        }

        else if (_item is IEquipmentItem) //장착아이템이라면
        {
            //장착시키는 코드를 작성
        }
    }


    public void OnBeginDrag(PointerEventData eventData) //마우스 드래그가 시작 됬을 때 실행되는 함수
    {
        if (eventData.button == PointerEventData.InputButton.Left) //만약 해당 스크립트가 들어있는 오브젝트를 좌클릭 했을경우
        {
            if (_item != null)
            {
                DragInventorySlot.Instance.DragSlot = this;
                DragInventorySlot.Instance.DragSetIcon(_item.Data.Icon);
                DragInventorySlot.Instance.transform.position = eventData.position;
            }
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (_item == null)
            return;

        DragInventorySlot.Instance.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DragInventorySlot.Instance.SetColor(0);
            DragInventorySlot.Instance.DragSlot = null;
            DragInventorySlot.Instance.transform.position = new Vector3(1000, 1000, 1000);
        }

        if (!IsOverUI())
        {
            Debug.Log("아이템 드랍");
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (DragInventorySlot.Instance.DragSlot == null)
            return;

        if (DragInventorySlot.Instance.DragSlot != this)
            return;

        if (_item == null)
            return;

            if (_item.Data.ID == DragInventorySlot.Instance.DragSlot._item.Data.ID)
            {
                GameManager.Instance.Player.Inventory.MergeItem(_item, DragInventorySlot.Instance.DragSlot._item);
                return;
            }

        _uiInventory.SlotSwap(this, DragInventorySlot.Instance.DragSlot);
    }


    private bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item == null)
            return;

        _uiInventory.UiItemDescription.ChildSetActive(true);

        Vector3 slotHalfSize = _rectTransform.sizeDelta * 0.5f;
        Vector3 getUiSize = _uiInventory.UiItemDescription.GetUISize() * 0.5f;
        Vector3 uiPos = new Vector3(getUiSize.x, -getUiSize.y) + slotHalfSize;

        _uiInventory.UiItemDescription.transform.position = transform.position + uiPos;
        _uiInventory.UiItemDescription.UpdateUI(_item);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        _uiInventory.UiItemDescription.ChildSetActive(false);
    }
}
