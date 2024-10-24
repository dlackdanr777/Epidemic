using System;
using UnityEngine;

[Serializable]
public struct Craft
{
    public Sprite Sprite;
    public string Name;
    public string NecessaryItemId;
    public int NecessaryItemAmount;
    public GameObject CraftPrefab;
    public PreviewObject PreviewPrefab;
}


public class BuildSystem : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Craft[] _craftItem;
    [SerializeField] private Camera _camera;
    [SerializeField] private AudioClip _buildSuccessClip;
    
    private int _craftItemIndex = -1;
    private PreviewObject _PreviewObj;
    private RaycastHit _hit;
    private LayerMask _layerMask;
    private Ray _ray;

    private bool _buildingEnable;
    public bool BuildingEnable => _buildingEnable;

    public void Start()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    public void Update()
    {

        if (_buildingEnable)
        {
            if (Input.GetMouseButtonDown(1))
                BuildDisable();

        }

        BuildEnable();
    }

    public int GetCraftItemLength()
    {
        return _craftItem.Length;
    }

    public Craft GetCraftItem(int index)
    {
        return _craftItem[index];
    }

    public void SelectCraftItem(int index)
    {
        if (index < 0 && index >= _craftItem.Length) { Debug.Log("인덱스범위 벗어남"); return; }

        if (_PreviewObj != null)
            Destroy(_PreviewObj.gameObject);


        _craftItemIndex = index;
        _PreviewObj = Instantiate(_craftItem[_craftItemIndex].PreviewPrefab.gameObject, Vector3.zero, _craftItem[_craftItemIndex].PreviewPrefab.transform.rotation).
            GetComponent<PreviewObject>();
        _buildingEnable = true;
    }

    public void BuildDisable()
    {
        if (_PreviewObj != null)
            Destroy(_PreviewObj.gameObject);

        _craftItemIndex = -1;
        _buildingEnable = false;
    }


    /// <summary>건축물 설치 시작했을때 주기적으로 불러오는 함수</summary>
    private void BuildEnable()
    {
        if (!_buildingEnable || _craftItemIndex == -1)
            return;

        //카메라에서 레이를 쏴 맞은 특정 레이어를 가진 오브젝트가 없을 경우 return
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(_ray, out _hit, 10, _layerMask))
            return;

        //ray가 맞은 위치에 미리보기 오브젝트 생성
        Vector3 location = _hit.point;
        location.Set(Mathf.Round(location.x * 10f) * 0.1f, location.y, Mathf.Round(location.z * 10f) * 0.1f);
        _PreviewObj.transform.position = location;

        string itemId = _craftItem[_craftItemIndex].NecessaryItemId;
        int amount = _craftItem[_craftItemIndex].NecessaryItemAmount;
        _PreviewObj.SetItem(itemId, amount);

        //Q, E 키를 입력받아 오브젝트 회전
        if (Input.GetKeyDown(KeyCode.Q))
            _PreviewObj.transform.eulerAngles -= Vector3.up * 30f;

        else if (Input.GetKeyDown(KeyCode.E))
            _PreviewObj.transform.eulerAngles += Vector3.up * 30f;

        //이후 마우스 좌클릭을 누르지 않을 경우 return
        if (!Input.GetMouseButtonDown(0))
            return;

        //건축이 가능한지 확인 후 해당 위치에 건축물 생성
        if (!_PreviewObj.isBuildable())
        {
            UIManager.Instance.ShowCenterText("그곳엔 건설할 수 없습니다.");
            return;
        }

        if (_player.Inventory.UseItemByID(itemId, amount))
        {
            Instantiate(_craftItem[_craftItemIndex].CraftPrefab, _PreviewObj.transform.position, _PreviewObj.transform.rotation);
            SoundManager.Instance.PlayAudio(AudioType.Effect, _buildSuccessClip);
        }
        else
        {
            UIManager.Instance.ShowCenterText("건설 재료가 부족합니다.");
        }
    }
}
