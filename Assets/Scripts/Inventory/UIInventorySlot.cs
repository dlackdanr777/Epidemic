using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> �κ��丮 ���� Ŭ���� </summary>
public class UIInventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image IconImage;
    public TextMeshProUGUI AmountText;

    private Item _item;
    public Item Item => _item;

    private int _slotIndex = -1;

    private UIInventory _uiInventory;
    private RectTransform _rectTransform;

    public void Init(UIInventory uiInventory, int slotIndex)
    {
        _uiInventory = uiInventory;
        _rectTransform = GetComponent<RectTransform>();

        _slotIndex = slotIndex;
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
            IconImage.sprite = Item.Data.Icon;
            if (Item is CountableItem)
            {
                AmountText.enabled = true;
                AmountText.text = Item.Amount.ToString();
            }
            else
            {
                AmountText.enabled = false;
            }

        }
    }


    /// <summary> ������ ��Ŭ������ ��� �����۰� ��ȣ�ۿ� �ϴ� �Լ�</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) //���� �ش� ��ũ��Ʈ�� ����ִ� ������Ʈ�� ��Ŭ�� �������
            return;

        if (Item == null)
            return;

        if (Item is IUsableItem) // �Һ�������̶��
        {
            IUsableItem uItem = Item as IUsableItem;
            uItem.Use();
        }

        else if (Item is CountableItem)
        {
            _uiInventory.UIDivItem.SetActive(Item);
        }

        else if (Item is IEquipmentItem) //�����������̶��
        {
            //������Ű�� �ڵ带 �ۼ�
        }
    }


    public void OnBeginDrag(PointerEventData eventData) //���콺 �巡�װ� ���� ���� �� ����Ǵ� �Լ�
    {
        if (eventData.button == PointerEventData.InputButton.Left) //���� �ش� ��ũ��Ʈ�� ����ִ� ������Ʈ�� ��Ŭ�� �������
        {
            if (Item != null)
            {
                _uiInventory.DragInvenSlot.SetDragItem(_item);
                _uiInventory.DragInvenSlot.transform.position = eventData.position;
            }
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (Item == null)
            return;

        _uiInventory.DragInvenSlot.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _uiInventory.DragInvenSlot.ResetDragItem();
        }

        if (!IsOverUI())
        {
            Debug.Log("������ ���");
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Item dragItem = _uiInventory.DragInvenSlot.GetItem();
        if (dragItem == null || dragItem == _item)
            return;

        if (Item == null)
        {
            Inventory.Instance.ChangeItemSlot(dragItem, _slotIndex);
            return;
        }

        if (Item.Data.ID == dragItem.Data.ID)
        {
            Inventory.Instance.MergeItem(Item, dragItem);
            return;
        }

        Inventory.Instance.SwapSlot(Item, dragItem);
    }


    private bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Item == null)
            return;

        _uiInventory.UIItemDescription.ChildSetActive(true);

        Vector3 slotHalfSize = _rectTransform.sizeDelta * 0.5f;
        Vector3 getUiSize = _uiInventory.UIItemDescription.GetUISize() * 0.5f;
        Vector3 uiPos = new Vector3(getUiSize.x, -getUiSize.y) + slotHalfSize;

        _uiInventory.UIItemDescription.transform.position = transform.position + uiPos;
        _uiInventory.UIItemDescription.UpdateUI(Item);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        _uiInventory.UIItemDescription.ChildSetActive(false);
    }
}
