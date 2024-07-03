using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGridInventory : MonoBehaviour
{
    public static Action StartDragHandler;
    public static Action EndDragHandler;

    [SerializeField] private GridInventory _inven;
    public GridInventory Inven => _inven;
    [SerializeField] private UIDragSlot _dragSlot;

    [Header("SlotOptions")]
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [Space]
    [SerializeField] private Transform _slotParent;
    [SerializeField] private Transform _dragSlotParent;
    [SerializeField] private UIInventorySlot _slotPrefab;
    [SerializeField] private UIGridItemSlot _gridItemSlotPrefab;



    private float _slotSizeX;
    private float _slotSizeY;
    private Vector2 _slotTmpPos;


    private UIInventorySlot[,] _slots;
    private List<UIGridItemSlot> _gridItemSlotList = new List<UIGridItemSlot>();
    private Queue<UIGridItemSlot> _gridItemSlotQueue = new Queue<UIGridItemSlot>();



    private void Start()
    {    
        Init();
        InitGridItemSlot();

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

        _slots = new UIInventorySlot[_gridHeight, _gridWidth];

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                _slots[y, x] = Instantiate(_slotPrefab, _slotParent);
                _slots[y, x].Init(this, _dragSlot, x, y);
            }
        }

        _slotTmpPos = _slots[0, 0].transform.position; 
    }


    public void ChangeSlotEffect(int x, int y, Item item)
    {
        int width = item.ItemData.Width;
        int height = item.ItemData.Height;
        for (int i = y, cntY = Mathf.Clamp(y + height, y, _gridHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _gridWidth); j < cntX; ++j)
            {
                if(_inven.InventoryGrid[i, j].Item == null)
                {
                    _slots[i, j].SetColor(true);
                    continue;
                }

                if (_inven.InventoryGrid[i,j].Item == item)
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
        int width = item.ItemData.Width;
        int height = item.ItemData.Height;

        if (_gridHeight < y + height || _gridWidth < x + width)
            return false;

        for (int i = y, cntY = Mathf.Clamp(y + height, y, _gridHeight); i < cntY; ++i)
        {
            for (int j = x, cntX = Mathf.Clamp(x + width, x, _gridWidth); j < cntX; ++j)
            {
                if (_inven.InventoryGrid[i, j].Item == null)
                    continue;

                if (_inven.InventoryGrid[i, j].Item == item)
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

        for (int i = 0, cnt = _gridItemSlotList.Count; i < cnt; ++i)
        {
            _gridItemSlotList[i].StartDrag();
        }
    }


    public void EndDrag()
    {
        if (!gameObject.activeSelf)
            return;

        for (int i = 0, cnt = _gridItemSlotList.Count; i < cnt; ++i)
        {
            _gridItemSlotList[i].EndDrag();
        }
    }


    private void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;

        _gridItemSlotQueue.Clear();
        for (int i = 0, cnt = _gridItemSlotList.Count; i < cnt; i++)
        {
            _gridItemSlotList[i].gameObject.SetActive(false);
            _gridItemSlotQueue.Enqueue(_gridItemSlotList[i]);
        }

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                GridSlot item = _inven.InventoryGrid[y, x];

                if (item.Item != null && item.IsMainSlot)
                {
                    UIGridItemSlot dragSlot = DequeueGridItemSlot();
                    Vector2 pos = _slotTmpPos + new Vector2(x * _slotSizeX, -y * _slotSizeY);
                    pos += new Vector2((item.Item.ItemData.Width - 1) * (_slotSizeX * 0.5f), -(item.Item.ItemData.Height - 1) * (_slotSizeY * 0.5f));

                    dragSlot.SetPos(pos);
                    dragSlot.SetItem(item, x, y , _slotSizeX, _slotSizeY);
                }
            }
        }
    }

    private void InitGridItemSlot()
    {
        if (0 < _gridItemSlotList.Count && _gridItemSlotList[0] != null)
            return;

        _gridItemSlotList.Clear();

        for (int i = 0; i < 10; ++i)
        {
            UIGridItemSlot slot = Instantiate(_gridItemSlotPrefab, _dragSlotParent);

            slot.gameObject.SetActive(false);
            slot.Init(this, _dragSlot, _slotSizeX, _slotSizeY);
            _gridItemSlotList.Add(slot);
            _gridItemSlotQueue.Enqueue(slot);
        }
    }

    private UIGridItemSlot DequeueGridItemSlot()
    {
        UIGridItemSlot slot;
        if (_gridItemSlotQueue.Count <= 0)
        {
            slot = Instantiate(_gridItemSlotPrefab, _dragSlotParent);

            slot.gameObject.SetActive(false);
            slot.Init(this, _dragSlot, _slotSizeX, _slotSizeY);
            _gridItemSlotList.Add(slot);
            _gridItemSlotQueue.Enqueue(slot);
        }
        
        slot = _gridItemSlotQueue.Dequeue();
        slot.gameObject.SetActive(true);
        return slot;
    }
}
