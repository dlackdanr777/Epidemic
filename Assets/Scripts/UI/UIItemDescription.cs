using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescription : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Image _image;

    private Vector3 _uiSize;

    public void Awake()
    {
        _uiSize = _rectTransform.sizeDelta;
        gameObject.SetActive(false);
    }

    public void UpdateUI(Item item, Vector2 pos)
    {
        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }
            

        gameObject.SetActive(true);

        _rectTransform.position = pos + new Vector2(_uiSize.x * 0.5f, -_uiSize.y * 0.5f);
        _itemNameText.text = item.Data.Name;
        _itemDescriptionText.text = item.Data.Description;
        _image.sprite = item.Data.Sprite;

        if (item is OtherItem)
            _itemTypeText.text = "��Ÿ ������";

        else if (item is PotionItem)
            _itemTypeText.text = "ȸ�� ������";

        else if (item is EquipmentItem)
            _itemTypeText.text = "���� ������";
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetUISize()
    {
        return _uiSize;
    }
}
