using System;
using UnityEngine;
using static UnityEditor.Progress;


public class BuildSystem : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Camera _camera;
    [SerializeField] private AudioClip _buildSuccessClip;
    
    private PreviewObject _previewObj;
    private RaycastHit _hit;
    private LayerMask _layerMask;
    private Ray _ray;

    private bool _buildingEnable;
    public bool BuildingEnable => _buildingEnable;

    private BuildData _selectBuildData;

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

    public void SelectCraftItem(BuildData buildData)
    {
        if (_previewObj != null)
            ObjectPoolManager.Instance.DespawnBuildPreviewObject(_previewObj);

        _selectBuildData = buildData;
        _previewObj = ObjectPoolManager.Instance.SpawnBuildPreviewObject(buildData.Id, Vector3.zero, Quaternion.identity);
        _buildingEnable = true;
    }

    public void BuildDisable()
    {
        if (_previewObj != null)
            ObjectPoolManager.Instance.DespawnBuildPreviewObject(_previewObj);

        _previewObj = null;
        _buildingEnable = false;
    }


    /// <summary>건축물 설치 시작했을때 주기적으로 불러오는 함수</summary>
    private void BuildEnable()
    {
        if (!_buildingEnable)
            return;

        //카메라에서 레이를 쏴 맞은 특정 레이어를 가진 오브젝트가 없을 경우 return
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(_ray, out _hit, 10, _layerMask))
            return;

        //ray가 맞은 위치에 미리보기 오브젝트 생성
        Vector3 location = _hit.point;
        location.Set(Mathf.Round(location.x * 10f) * 0.1f, location.y, Mathf.Round(location.z * 10f) * 0.1f);
        _previewObj.transform.position = location;

        //Q, E 키를 입력받아 오브젝트 회전
        if (Input.GetKeyDown(KeyCode.Q))
            _previewObj.transform.eulerAngles -= Vector3.up * 30f;

        else if (Input.GetKeyDown(KeyCode.E))
            _previewObj.transform.eulerAngles += Vector3.up * 30f;

        //이후 마우스 좌클릭을 누르지 않을 경우 return
        if (!Input.GetMouseButtonDown(0))
            return;

        //건축이 가능한지 확인 후 해당 위치에 건축물 생성
        if (!_previewObj.isBuildable())
        {
            UIManager.Instance.ShowCenterText("그곳엔 건설할 수 없습니다.");
            return;
        }

        bool isGiveItem = true;
        for (int i = 0, cnt = _selectBuildData.NeedItemData.Length; i < cnt; ++i)
        {

            if (!_player.Inventory.UseItemByID(_selectBuildData.NeedItemData[i].NeedItemId, _selectBuildData.NeedItemData[i].NeedItemAmount))
            {
                isGiveItem = false;
                break;
            }
        }

        if (isGiveItem)
        {
            BuildObject buildObject = ObjectPoolManager.Instance.SpawnBuildObject(_selectBuildData.Id, _previewObj.transform.position, _previewObj.transform.rotation);
            SoundManager.Instance.PlayAudio(AudioType.Effect, _buildSuccessClip);
        }

        else
        {
            UIManager.Instance.ShowCenterText("건설 재료가 부족합니다.");
            return;
        }
    }
}
