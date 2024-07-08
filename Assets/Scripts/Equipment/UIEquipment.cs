using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class UIEquipment : MonoBehaviour
{
    [SerializeField] private UIEquipmentSlot[] _slots;
    [SerializeField] private UIDragSlot _dragSlot;
    [SerializeField] private UIItemDescription _itemDescription;
    [SerializeField] private UIEquipShowItemSlot _showItemSlotPrefab;
    [SerializeField] private Transform _showItemSlotParent;

    private List<UIEquipmentSlot> _slotList = new List<UIEquipmentSlot>();
    private List<UIEquipShowItemSlot> _showItemSlotList = new List<UIEquipShowItemSlot>();

    private void Awake()
    {
        InitShowItemSlot();
        UpdateUI();
        UserInfo.OnChangeEquipItemHandler += (type) => UpdateUI();

        for (int i = 0, cnt = _slots.Length; i < cnt; i++)
        {
            _slots[i].Init(_dragSlot);
        }

        for(int i = 0, cntI = (int)EquipItemType.Length; i < cntI; i++)
        {
            for (int j = 0, cntJ = _slots.Length; j < cntJ; j++)
            {
                if (_slots[j].Type != (EquipItemType)i)
                    continue;

                _slotList.Add(_slots[j]);
                break;
            }
        }
    }

    private void InitShowItemSlot()
    {
        if (0 < _showItemSlotList.Count && _showItemSlotList[0] != null)
            return;

        _showItemSlotList.Clear();

        for (int i = 0; i < (int)EquipItemType.Length; ++i)
        {
            UIEquipShowItemSlot slot = Instantiate(_showItemSlotPrefab, _showItemSlotParent);

            slot.gameObject.SetActive(false);
            slot.Init(_itemDescription);
            _showItemSlotList.Add(slot);
        }
    }

    private void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;

        EquipmentItem item;

        for (int i = 0, cnt = (int)EquipItemType.Length; i < cnt; i++)
        {
            item = UserInfo.GetEquipItem((EquipItemType)i);

            UIEquipShowItemSlot dragSlot = _showItemSlotList[i];

            if (item == null)
            {
                dragSlot.gameObject.SetActive(false);
                continue;
            }

            dragSlot.gameObject.SetActive(true);
            dragSlot.SetItem(item, _slotList[i].RectTransfrom.anchoredPosition, 100, 100);
            _slotList[i].UpdateUI();
        }
    }

}
