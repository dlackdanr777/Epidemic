using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGridInventory : MonoBehaviour
{
    public static Action StartDragHandler;
    public static Action EndDragHandler;

    [Header("Components")]
    [SerializeField] private GridInventory _inven;
    public GridInventory Inven => _inven;
    [SerializeField] private UIItemDescription _itemDescription;
    [SerializeField] private UIDragSlot _dragSlot;

    [Header("SlotOptions")]
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [Space]
    [SerializeField] private Transform _slotParent;
    [SerializeField] private Transform _showItemSlotParent;
    [SerializeField] private UIGridInventorySlot _slotPrefab;
    [SerializeField] private UIShowItemSlot _showItemSlotPrefab;

    private float _slotSizeX;
    private float _slotSizeY;
    private int _invenWidth;
    private int _invenHeight;
    private Vector2 _slotTmpPos;

    private UIGridInventorySlot[,] _slots;
    private List<UIShowItemSlot> _showItemSlotList = new List<UIShowItemSlot>();
    private Queue<UIShowItemSlot> _showItemSlotQueue = new Queue<UIShowItemSlot>();



    public void Init()
    {
        InitInven();
        InitShowItemSlot();
        UpdateUI();

        SetInven(_inven);

        _inven.OnUpdateHandler += UpdateUI;
        StartDragHandler += StartDrag;
        EndDragHandler += EndDrag;
        EndDragHandler += ResetSlotEffect;

        LoadingSceneManager.OnChangeSceneHandler += OnChangeSceneEvent;
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnChangeSceneEvent()
    {
        _inven.OnUpdateHandler -= UpdateUI;
        StartDragHandler -= StartDrag;
        EndDragHandler -= EndDrag;
        LoadingSceneManager.OnChangeSceneHandler -= OnChangeSceneEvent;
    }


    public void SetInven(GridInventory inventory)
    {
        _inven = inventory;

        for (int i = 0; i < _gridHeight; ++i)
        {
            for(int j = 0; j < _gridWidth; ++j)
            {
                if(_inven.GridHight <= i || _inven.GridWidth <= j)
                {
                    _slots[i, j].gameObject.SetActive(false);
                    continue;
                }
                _slots[i, j].gameObject.SetActive(true);
            }
        }

         _invenWidth = _inven.GridWidth;
        _invenHeight = _inven.GridHight;

        UpdateUI();
    }

    public void InitInven()
    {
        _slotSizeX = _slotPrefab.GetComponent<RectTransform>().sizeDelta.x;
        _slotSizeY = _slotPrefab.GetComponent<RectTransform>().sizeDelta.y;

        _slots = new UIGridInventorySlot[_gridHeight, _gridWidth];

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                _slots[y, x] = Instantiate(_slotPrefab, _slotParent);
                _slots[y, x].Init(this, _dragSlot, x, y);

                Vector2 pos = new Vector2(_slotSizeX * 0.5f, -_slotSizeY * 0.5f);
                pos += new Vector2(x * _slotSizeX, -y * _slotSizeY);
                _slots[y, x].SetAnchoredPosition(pos);
            }
        }

        if(_inven != null )
        {
            _invenWidth = _inven.GridWidth;
            _invenHeight = _inven.GridHight;
        }
    }


    public void ChangeSlotEffect(int x, int y, Item item)
    {
        int width = item.Data.Width;
        int height = item.Data.Height;
        bool dropEnabled = true;

        if (_invenHeight < y + height || _invenWidth < x + width)
        {
            for (int i = y, cntY = Mathf.Clamp(y + height, y, _invenHeight); i < cntY; ++i)
            {
                for (int j = x, cntX = Mathf.Clamp(x + width, x, _invenWidth); j < cntX; ++j)
                {
                    _slots[i, j].SetColor(false);
                }
            }
            return;
        }

        for (int i = y, cntY = Mathf.Clamp(y + height, y, _invenHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _invenWidth); j < cntX; ++j)
            {

                if(_inven.GridSlots[i, j].Item == null || _inven.GridSlots[i, j].Item == item)
                    continue;

                dropEnabled = false;
                break;
            }

            if (!dropEnabled)
                break;
        }

        for (int i = y, cntY = Mathf.Clamp(y + height, y, _invenHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _invenWidth); j < cntX; ++j)
            {
                _slots[i, j].SetColor(dropEnabled);
            }
        }


    }


    public void ResetSlotEffect(int x, int y, int width, int height)
    {
        for (int i = y, cntY = Mathf.Clamp(y + height, y, _invenHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _invenWidth); j < cntX; ++j)
            {
                _slots[i, j].ResetColor();
            }
        }
    }

    public void ResetSlotEffect()
    {
        for (int i = 0; i < _invenHeight; ++i)
        {
            for (int j = 0; j < _invenWidth; ++j)
            {
                _slots[i, j].ResetColor();
            }
        }
    }


    public bool ItemMoveEnabled(int x, int y, Item item)
    {
        int width = item.Data.Width;
        int height = item.Data.Height;

        if (_invenHeight < y + height || _invenWidth < x + width)
            return false;

        for (int i = y, cntY = Mathf.Clamp(y + height, y, _invenHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _invenWidth); j < cntX; ++j)
            {
                if (_inven.GridSlots[i, j].Item == null)
                    continue;

                if (_inven.GridSlots[i, j].Item == item)
                    continue;

                return false;
            }
        }

        return true;
    }


    public void StartDrag()
    {
        if (!gameObject.activeSelf)
            return;

        for (int i = 0, cnt = _showItemSlotList.Count; i < cnt; ++i)
        {
            _showItemSlotList[i].StartDrag();
        }
    }


    public void EndDrag()
    {
        if (!gameObject.activeSelf)
            return;

        for (int i = 0, cnt = _showItemSlotList.Count; i < cnt; ++i)
        {
            _showItemSlotList[i].EndDrag();
        }
    }


    public void SlotRightClicked(Item item)
    {
        if(item == null)
        {
            DebugLog.Log("사용할 아이템이 없습니다.");
            return;
        }

        if(item is IUsableItem)
        {
            IUsableItem usableItem = (IUsableItem)item;
            usableItem.Use();
            DebugLog.Log("사용 실행");
        }

        if(item is EquipmentItem)
        {
            DebugLog.Log("장착 아이템");
            EquipmentItem equipmentItem = (EquipmentItem)item;
            EquipmentItem changeItem = UserInfo.GetEquipItem(equipmentItem.EquipmentItemData.Type);

            UserInfo.ChangeEquipItem(equipmentItem);
            _inven.RemoveItem(equipmentItem);

            if(changeItem != null)
                _inven.AddItem(changeItem.Data);
        }
    }


    private void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;

        _showItemSlotQueue.Clear();
        DebugLog.Log("업데이트 됨");
        for (int i = 0, cnt = _showItemSlotList.Count; i < cnt; i++)
        {
            _showItemSlotList[i].gameObject.SetActive(false);
            _showItemSlotQueue.Enqueue(_showItemSlotList[i]);
        }

        for (int y = 0; y < _invenHeight; y++)
        {
            for (int x = 0; x < _invenWidth; x++)
            {
                GridSlot item = _inven.GridSlots[y, x];

                if (item.Item != null && item.IsMainSlot)
                {
                    UIShowItemSlot dragSlot = DequeueGridItemSlot();
                    Vector2 pos = _slots[y, x].GetAnchoredPosition() + new Vector2((item.Item.Data.Width - 1) * _slotSizeX * 0.5f, (item.Item.Data.Height - 1) * -_slotSizeY * 0.5f);
                    dragSlot.SetPos(pos);
                    dragSlot.SetItem(item, x, y , _slotSizeX, _slotSizeY);
                }
            }
        }
        EndDrag();
    }

    private void InitShowItemSlot()
    {
        if (0 < _showItemSlotList.Count && _showItemSlotList[0] != null)
            return;

        _showItemSlotList.Clear();

        for (int i = 0; i < 10; ++i)
        {
            UIShowItemSlot slot = Instantiate(_showItemSlotPrefab, _showItemSlotParent);

            slot.gameObject.SetActive(false);
            slot.Init(this, _dragSlot, _itemDescription, _slotSizeX, _slotSizeY);
            _showItemSlotList.Add(slot);
            _showItemSlotQueue.Enqueue(slot);
        }
    }



    private UIShowItemSlot DequeueGridItemSlot()
    {
        UIShowItemSlot slot;
        if (_showItemSlotQueue.Count <= 0)
        {
            slot = Instantiate(_showItemSlotPrefab, _showItemSlotParent);

            slot.gameObject.SetActive(false);
            slot.Init(this, _dragSlot, _itemDescription, _slotSizeX, _slotSizeY);
            _showItemSlotList.Add(slot);
            _showItemSlotQueue.Enqueue(slot);
        }
        
        slot = _showItemSlotQueue.Dequeue();
        slot.gameObject.SetActive(true);
        return slot;
    }
}
