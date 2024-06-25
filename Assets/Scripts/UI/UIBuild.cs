using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuild : PopupUI
{
    [SerializeField] private Transform _gridLayout; //슬롯들을 자식으로 가지고있는 오브젝트
    [SerializeField] private BuildSystem _buildSystem;
    [SerializeField] private UIBuildSlot _slotPrefab;
    private UIBuildSlot[] _buildSlot;



    private void Start()
    {
        SetSlots();
    }

    private void SetSlots()
    {
        //건축물 정보 구조체 배열의 길이를 가져온다.
        int craftItemLength = _buildSystem.GetCraftItemLength();
        _buildSlot = new UIBuildSlot[craftItemLength];

        for (int i = 0, count = craftItemLength; i < count ; i++)
        {
            _buildSlot[i] = Instantiate(_slotPrefab, Vector3.zero, Quaternion.identity);
            _buildSlot[i].transform.parent = _gridLayout;
            _buildSlot[i].UpdateUI(_buildSystem.GetCraftItem(i));

            int index = i;
            _buildSlot[i].Button.onClick.AddListener(() => OnButtonClicked(index));
        }
    }


    private void OnButtonClicked(int index)
    {
        _buildSystem.SelectCraftItem(index);
        CloseButton.onClick?.Invoke();
    }

    public override void ChildSetActive(bool value)
    {
        if (value)
            _buildSystem.BuildDisable();

        base.ChildSetActive(value);
    }

}
