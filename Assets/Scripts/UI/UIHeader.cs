using UnityEngine;
using UnityEngine.EventSystems;

public class UIHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform _targetTrUI;

    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;

    private void Awake()
    {
        if(_targetTrUI == null)
        {
            _targetTrUI = transform.parent;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startingPoint = _targetTrUI.position;
        _moveBegin = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        _targetTrUI.position = _startingPoint + _moveOffset;
    }
}
