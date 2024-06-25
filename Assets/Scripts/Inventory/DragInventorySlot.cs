using UnityEngine;
using UnityEngine.UI;

public class DragInventorySlot : SingletonHandler<DragInventorySlot>
{
    public UIInventorySlot DragSlot;

    [SerializeField]
    private Image _icon;

    public void DragSetIcon(Sprite Icon)
    {
        _icon.sprite = Icon;
        SetColor(1);
    }

    public void SetColor(float alpha)
    {
        Color color = _icon.color;
        color.a = alpha;
        _icon.color = color;
    }

}
