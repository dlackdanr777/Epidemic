using UnityEngine;
using UnityEngine.UI;

public class UIDragSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;

    private Item _currentItem;
    public Item CurrentItem => _currentItem;

    private UIGridInventory _currentUIInven;
    public UIGridInventory CurrentUIInven => _currentUIInven;


    private bool _isUseInven;
    public bool IsUseInven => _isUseInven;
    private EquipItemType _type;


    public void Start()
    {
        gameObject.SetActive(false);
    }


    public void Enabled(Item item, UIGridInventory _uiInven)
    {
        _currentItem = item;
        _currentUIInven = _uiInven;

        _isUseInven = true;
        _rectTransform.sizeDelta = new Vector2(100 * item.Data.Width, 100 * item.Data.Height);
        _image.sprite = item.Data.Sprite;


        gameObject.SetActive(true);
    }


    public void Enabled(EquipItemType type)
    {

        _currentItem = UserInfo.GetEquipItem(type);
        if (_currentItem == null)
            return;

        _type = type;
        _isUseInven = false;
        _rectTransform.sizeDelta = new Vector2(100 * _currentItem.Data.Width, 100 * _currentItem.Data.Height);
        _image.sprite = _currentItem.Data.Sprite;

        gameObject.SetActive(true);
    }



    public void SetPos(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }


    public void Disabled()
    {
        _currentItem = null;
        _currentUIInven = null;
        _isUseInven = false;
        gameObject.SetActive(false);
    }

    public void RemoveItemByInven()
    {
        _currentUIInven.Inven.RemoveItem(_currentItem);
    }


    public bool AddItem(InventoryItemData data)
    {
        return CurrentUIInven.Inven.AddItem(data);
    }
}