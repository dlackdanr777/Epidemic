using UnityEngine;
using UnityEngine.UI;

public class UIDragSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;

    private UIShowItemSlot _currentSlot;

    public GridSlot CurrentItem => _currentSlot != null ? _currentSlot.Item : null;
    public UIGridInventory CurrentUIInven => _currentSlot != null ? _currentSlot.UIInven : null;

    public int IndexX => _currentSlot != null ? _currentSlot.IndexX: -1;   
    public int IndexY => _currentSlot != null ? _currentSlot.IndexY : -1;


    public void Start()
    {
        gameObject.SetActive(false);
    }


    public void Enabled(UIShowItemSlot slot)
    {
        _currentSlot = slot;
        _rectTransform.sizeDelta = slot.RectTransform.sizeDelta;
        _image.sprite = slot.Item.Item.Data.Sprite;

        gameObject.SetActive(true);
    }

    public void SetPos(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }


    public void Disabled()
    {
        _currentSlot = null;
        gameObject.SetActive(false);
    }
}