using System.Collections.Generic;
using UnityEngine;

public class UIEquipment : MonoBehaviour
{
    [SerializeField] private UIEquipmentSlot[] _slots;
    [SerializeField] private UIDragSlot _dragSlot;
    [SerializeField] private UIItemDescription _itemDescription;
    [SerializeField] private UIEquipShowItemSlot _showItemSlotPrefab;
    [SerializeField] private Transform _showItemSlotParent;
    [SerializeField] private AudioClip _equipSound;

    private List<UIEquipShowItemSlot> _showItemSlotList = new List<UIEquipShowItemSlot>();
    private Dictionary<EquipItemType, UIEquipmentSlot> _slotDic = new Dictionary<EquipItemType, UIEquipmentSlot>();


    private void Awake()
    {
        InitShowItemSlot();
        UserInfo.OnChangeEquipItemHandler += UpdateUI;
        UserInfo.OnChangeEquipItemHandler += OnEquipItemEvent;

        for (int i = 0, cnt = _slots.Length; i < cnt; i++)
        {
            _slots[i].Init(_dragSlot, SetSlotDic);
        }
        UpdateUI();


        LoadingSceneManager.OnChangeSceneHandler += OnChangeSceneEvent;
    }



    private void OnChangeSceneEvent()
    {
        DebugLog.Log("이벤트 실행");
        UserInfo.OnChangeEquipItemHandler -= UpdateUI;
        UserInfo.OnChangeEquipItemHandler -= OnEquipItemEvent;
        LoadingSceneManager.OnChangeSceneHandler -= OnChangeSceneEvent;
    }


    private void SetSlotDic(EquipItemType type, UIEquipmentSlot slot)
    {
        if(_slotDic.ContainsKey(type))
        {
            DebugLog.LogError("해당 타입이 이미 등록되어 있습니다: " + type);
            return;
        }

        _slotDic.Add(type, slot);
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
            slot.Init(_itemDescription, _dragSlot);
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

            if (!_slotDic.TryGetValue((EquipItemType)i, out UIEquipmentSlot slot) || item == null)
            {
                dragSlot.gameObject.SetActive(false);
                continue;
            }

            dragSlot.gameObject.SetActive(true);
            dragSlot.SetItem(item, slot.RectTransfrom.anchoredPosition, _slotDic[(EquipItemType)i].RectTransfrom.sizeDelta);
        }
    }

    private void OnEquipItemEvent()
    {
        if (this == null)
            return;

        SoundManager.Instance.PlayAudio(AudioType.Effect, _equipSound);
    }

}
