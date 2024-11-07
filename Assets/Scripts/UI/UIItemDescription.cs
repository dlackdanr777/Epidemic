using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescription : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private TextMeshProUGUI _itemEffectText;
    [SerializeField] private Image _image;

    private Vector3 _uiSize;

    public void Awake()
    {
        _uiSize = _rectTransform.sizeDelta;
        gameObject.SetActive(false);
    }

    public void UpdateUI(Item item, Vector2 pos, Vector2 slotSize)
    {
        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }
            

        gameObject.SetActive(true);

        Vector2 targetPosition = pos;
        float screenWidth = Screen.width;
        float addPanelSizeX = _rectTransform.sizeDelta.x * 0.5f;
        float addPanelSizeY = _rectTransform.sizeDelta.y * 0.5f;
        float addSlotSizeX = slotSize.x * 0.5f;
        float addSlotSizeY = slotSize.y * 0.5f;

        targetPosition.x = screenWidth < pos.x + addPanelSizeX + addPanelSizeX + addSlotSizeX
             ? pos.x - addPanelSizeX - addSlotSizeX
             : pos.x + addPanelSizeX + addSlotSizeX;

        targetPosition.y = pos.y - addPanelSizeY - addPanelSizeY + addSlotSizeY < 0
            ? pos.y + addPanelSizeY - addSlotSizeY
            : pos.y - addPanelSizeY + addSlotSizeY;

        _rectTransform.position = targetPosition;
        _itemNameText.text = item.Data.Name;
        _itemDescriptionText.text = item.Data.Description;
        _image.sprite = item.Data.Sprite;
        _itemEffectText.text = ItemManager.Instance.GetItemEffectText(item.Data);
        _itemEffectText.gameObject.SetActive(!string.IsNullOrEmpty(_itemNameText.text));
        _itemTypeText.text = item switch
        {
            OtherItem => "기타 아이템",
            PotionItem => "회복 아이템",
            EquipmentItem => "장착 아이템",
            _ => string.Empty
        };
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
