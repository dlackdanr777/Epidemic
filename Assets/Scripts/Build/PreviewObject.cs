using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    private List<Collider> _colliders = new List<Collider>();
    private const int IGNORE_RAYCAST_LAYER = 2;

    private List<Renderer> _renderers = new List<Renderer>();

    private BuildData _buildData;
    public BuildData BuildData => _buildData;

    private bool _isOverlap;
    public bool IsOverlap => _isOverlap;
    private bool _isGiveItem;

    public bool IsGiveItem => _isGiveItem;
    public void SetBuildData(BuildData buildData)
    {
        _buildData = buildData;
    }


    private void Start()
    {
        _renderers.Add(transform.GetComponent<Renderer>());
        foreach (Transform tfChild in transform)
            _renderers.Add(tfChild.GetComponent<Renderer>());
    }

    private void FixedUpdate()
    {
        if (_buildData == null)
            return;

        ChangeColor();
    }

    private void OnEnable()
    {
        _colliders.Clear();
    }


    /// <summary> 겹치는 오브젝트가 있는지, 재료 아이템 보유 여부를 판단해 색을 변경하는 함수 </summary>
    private void ChangeColor()
    {
        _isGiveItem = true;
        for (int i = 0, cnt = BuildData.NeedItemData.Length; i < cnt; ++i)
        {
            int itemCount = GameManager.Instance.Player.Inventory.GetItemCountByID(BuildData.NeedItemData[i].NeedItemId);

            if (itemCount < BuildData.NeedItemData[i].NeedItemAmount)
            {
                _isGiveItem = false;
                break;
            }
        }

        _isOverlap = 0 < _colliders.Count;
        if (_isOverlap || !_isGiveItem)
        {
            SetColor(_redMaterial);
            DebugLog.Log("건축 불가능");
        }
        else
        {
            SetColor(_greenMaterial);
            DebugLog.Log("건축 가능");
        }

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
}
