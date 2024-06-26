using UnityEngine;
using UnityEngine.UI;

public class DragInventorySlot : MonoBehaviour
{
    [SerializeField] private Image _icon;

    private Item _currentItem;


    public Item GetItem()
    {
        return _currentItem;
    }


    public void SetDragItem(Item item)
    {
        _currentItem = item;
        _icon.sprite = item.Data.Icon;
        SetColor(1);
    }

    public void ResetDragItem()
    {
        _currentItem = null;
        _icon.sprite = null;
        SetColor(0);
        transform.position = new Vector3(1000, 1000, 1000);
    }


    public void SetColor(float alpha)
    {
        Color color = _icon.color;
        color.a = alpha;
        _icon.color = color;
    }

}
