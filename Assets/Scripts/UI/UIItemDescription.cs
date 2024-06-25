using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescription : PopupUI
{
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Image _image;

    private Vector3 _uiSize;

    private void Start()
    {
        _uiSize = GetComponent<RectTransform>().sizeDelta;
    }
    public void UpdateUI(Item item)
    {
        if(item != null)
        {
            _itemNameText.text = item.Data.Name;
            _itemAmountText.text = item.Amount.ToString();
            _itemDescriptionText.text = item.Data.Description;
            _image.sprite = item.Data.Icon;
        }
        else
        {
            Debug.Log("아이템 정보가 없습니다.");
        }
    }

    public Vector3 GetUISize()
    {
        return _uiSize;
    }
}
