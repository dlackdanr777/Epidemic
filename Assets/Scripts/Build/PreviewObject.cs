using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    private List<Collider> _colliders = new List<Collider>();
    private const int IGNORE_RAYCAST_LAYER = 2;

    private int _itemID;
    private int _itemAmount;

    private List<Renderer> _renderers = new List<Renderer>();

    private void Start()
    {
        _renderers.Add(transform.GetComponent<Renderer>());
        foreach (Transform tfChild in transform)
            _renderers.Add(tfChild.GetComponent<Renderer>());
    }

    private void FixedUpdate()
    {
        ChangeColor();
    }

    public void SetItem(int id, int Amount)
    {
        _itemID = id;
        _itemAmount = Amount;
    }

    /// <summary> 겹치는 오브젝트가 있는지, 재료 아이템 보유 여부를 판단해 색을 변경하는 함수 </summary>
    private void ChangeColor()
    {
        int itemCount = GameManager.Instance.Player.Inventory.FindItemCountByID(_itemID);

        if (_colliders.Count > 0 || itemCount < _itemAmount)
            SetColor(_redMaterial);
        else
            SetColor(_greenMaterial);
    }

    /// <summary> 오브젝트의 마테리얼들을 변경하는 함수 </summary>
    private void SetColor(Material mat)
    {
        foreach (var renderer in _renderers)
            renderer.material = mat;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 9 && other.gameObject.layer != 17 && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
        {
            _colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9 && other.gameObject.layer != 17 &&  other.gameObject.layer != IGNORE_RAYCAST_LAYER)
        {
            _colliders.Remove(other);
        }
    }

    public bool isBuildable()
    {
        return _colliders.Count == 0;
    }
}
