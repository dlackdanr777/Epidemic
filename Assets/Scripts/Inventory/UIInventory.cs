using UnityEngine;
using UnityEngine.UI;
using Muks.PcUI;
using Muks.Tween;

/// <summary> 인벤토리 UI 관리 클래스 </summary>
public class UIInventory : PcUIView
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


    [Header("Animation Options")]
    [SerializeField] private GameObject _animeUI;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Space]
    [SerializeField] private float _showDuration;
    [SerializeField] private TweenMode _showTweenMode;

    [Space]
    [SerializeField] private float _hideDuration;
    [SerializeField] private TweenMode _hideTweenMode;


    private UIInventorySlot[] _slots;
    private Inventory _inventory => Inventory.Instance;

    private Vector2 _tmpPos;


    public override void Init()
    {
        _sortButton.onClick.AddListener(_inventory.SortInventory);
        SetSlots();
        _uiDivItem.Init(this);
        _uiItemDescription.Init();

        Inventory.OnUpdateHandler += UpdateUI;
        UpdateUI();

        gameObject.SetActive(false);
    }

    public override void Show()
    {
        VisibleState = VisibleState.Appearing;
        gameObject.SetActive(true);
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;
        _animeUI.transform.position = _tmpPos != Vector2.zero ? _tmpPos : _animeUI.transform.position;

        UpdateUI();

        TweenData tween = _canvasGroup.TweenAlpha(1, _showDuration, _showTweenMode);
        tween.OnComplete(() =>
        {
            VisibleState = VisibleState.Appeared;
            _canvasGroup.interactable = true;
        });

    }

    public override void Hide()
    {
        VisibleState = VisibleState.Disappearing;
        gameObject.SetActive(true);
        _uiDivItem.gameObject.SetActive(false);
        _uiItemDescription.gameObject.SetActive(false);
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 1;

        _tmpPos = _animeUI.transform.position;

        TweenData tween = _canvasGroup.TweenAlpha(0, _hideDuration, _hideTweenMode);
        tween.OnComplete(() =>
        {
            VisibleState = VisibleState.Disappeared;
            _canvasGroup.interactable = true;
            gameObject.SetActive(false);
        });
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


    public void UpdateUI()
    {
        for (int i = 0, count = _slots.Length; i < count; i++)
        {
            _slots[i].UpdateUI(_inventory.InventoryItems[i]);
        }
    }


}
