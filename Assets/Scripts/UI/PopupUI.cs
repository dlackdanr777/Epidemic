using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour, IPointerDownHandler
{
    protected GameObject[] _childObj;

    [HideInInspector] public bool UIEnable;
    public Button CloseButton;
    public event Action OnFocus;

    public virtual void Awake()
    {
        Init();
    }

    public virtual void ChildSetActive(bool value)
    {
        UIEnable = value;
        for (int i = 0, count = _childObj.Length; i < count; i++)
        {
            _childObj[i].SetActive(value);
        }
    }

    public virtual void ChildSetActive()
    {
        UIEnable = !UIEnable;
        for (int i = 0, count = _childObj.Length; i < count; i++)
        {
            _childObj[i].SetActive(UIEnable);
        }
    }

    public void Init()
    {
        _childObj = new GameObject[transform.childCount];
        for (int i = 0, count = transform.childCount; i < count; i++)
        {
            _childObj[i] = transform.GetChild(i).gameObject;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(OnFocus != null)
        {
            OnFocus();
        }
    }
}
