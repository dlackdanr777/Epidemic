using UnityEngine;

public class UIEquipment : MonoBehaviour
{
    [SerializeField] private UIEquipmentSlot[] _slots;
    [SerializeField] private UIDragSlot _dragSlot;

    private void Start()
    {
        UpdateUI();
        UserInfo.OnChangeEquipItemHandler += (type) => UpdateUI();

        for (int i = 0, cnt = _slots.Length; i < cnt; i++)
        {
            _slots[i].Init(_dragSlot);
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;

        for(int i = 0, cnt =  _slots.Length; i < cnt; i++)
        {
            _slots[i].UpdateUI();
        }
    }

}
