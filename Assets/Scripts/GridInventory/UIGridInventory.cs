using System;
using System.Collections.Generic;
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
    private Vector2 _slotTmpPos;

    private UIGridInventorySlot[,] _slots;
    private List<UIShowItemSlot> _showItemSlotList = new List<UIShowItemSlot>();
    private Queue<UIShowItemSlot> _showItemSlotQueue = new Queue<UIShowItemSlot>();



    private void Start()
    {    
        Init();
        InitShowItemSlot();

        UpdateUI();

        _inven.OnUpdateHandler += UpdateUI;
        StartDragHandler += StartDrag;
        EndDragHandler += EndDrag;

        SceneManager.activeSceneChanged += (scene1, scene2) =>
        {
            _inven.OnUpdateHandler -= UpdateUI;
            StartDragHandler -= StartDrag;
            EndDragHandler -= EndDrag;
        };
    }


    public void SetInven(GridInventory inventory)
    {
        if(_inven != null)
            _inven.OnUpdateHandler -= UpdateUI;

        _inven = inventory;
        _inven.OnUpdateHandler += UpdateUI;
        UpdateUI();
    }

    public void Init()
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
    }


    public void ChangeSlotEffect(int x, int y, Item item)
    {
        int width = item.Data.Width;
        int height = item.Data.Height;
        for (int i = y, cntY = Mathf.Clamp(y + height, y, _gridHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _gridWidth); j < cntX; ++j)
            {
                if(_inven.GridSlots[i, j].Item == null)
                {
                    _slots[i, j].SetColor(true);
                    continue;
                }

                if (_inven.GridSlots[i,j].Item == item)
                {
                    _slots[i, j].SetColor(true);
                    continue;
                }

                _slots[i, j].SetColor(false);
            }
        }
    }


    public void ResetSlotEffect(int x, int y, int width, int height)
    {
        for (int i = y, cntY = Mathf.Clamp(y + height, y, _gridHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _gridWidth); j < cntX; ++j)
            {
                _slots[i, j].ResetColor();
            }
        }
    }


    public bool ItemMoveEnabled(int x, int y, Item item)
    {
        int width = item.Data.Width;
        int height = item.Data.Height;

        if (_gridHeight < y + height || _gridWidth < x + width)
            return false;

        for (int i = y, cntY = Mathf.Clamp(y + height, y, _gridHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _gridWidth); j < cntX; ++j)
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
    }


    private void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;

        _showItemSlotQueue.Clear();
        for (int i = 0, cnt = _showItemSlotList.Count; i < cnt; i++)
        {
            _showItemSlotList[i].gameObject.SetActive(false);
            _showItemSlotQueue.Enqueue(_showItemSlotList[i]);
        }

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
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
