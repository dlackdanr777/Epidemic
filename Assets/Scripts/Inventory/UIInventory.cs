using UnityEngine;
using UnityEngine.UI;


/// <summary> 인벤토리 UI 관리 클래스 </summary>
public class UIInventory : PopupUI
{
    [Header("Components")]
    [SerializeField] private Transform _gridLayout; //슬롯들을 자식으로 가지고있는 오브젝트
    [SerializeField] private Button _sortButton;
    [SerializeField] private UIInventorySlot _slotPrefab;

    [SerializeField] private UIDivItem _uiDivItem; 
    public UIDivItem UIDivItem => _uiDivItem;

    [SerializeField] private DragInventorySlot _dragSlot;
    public DragInventorySlot DragInvenSlot => _dragSlot;

    [SerializeField] private UIItemDescription _uiItemDescription;

    public UIItemDescription UIItemDescription => _uiItemDescription;

    private UIInventorySlot[] _slots;
    private Inventory _inventory => Inventory.Instance;


    public override void Awake()
    {
        base.Awake();
        _sortButton.onClick.AddListener(_inventory.SortInventory);
        SetSlots();
        _uiDivItem.Init(this);

        Inventory.OnUpdateHandler += UpdateUI;
        UpdateUI();
    }

    private void SetSlots()
    {
        int itemCount = Inventory.Instance.GetItemCount();
        _slots = new UIInventorySlot[itemCount];
        for(int i = 0; i < itemCount; i++)
        {
            UIInventorySlot slot = Instantiate(_slotPrefab, _gridLayout);
            slot.Init(this, i);
            _slots[i] = slot;
        }
    }


    public override void ChildSetActive(bool value)
    {
        UIDivItem.ChildSetActive(false);
        UIItemDescription.ChildSetActive(false);
        base.ChildSetActive(value);
    }


    public void UpdateUI()
    {
        for (int i = 0, count = _slots.Length; i < count; i++)
        {
            _slots[i].UpdateUI(_inventory.InventoryItems[i]);
        }
    }
}
