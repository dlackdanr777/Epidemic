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


    /// <summary>���๰ ��ġ ���������� �ֱ������� �ҷ����� �Լ�</summary>
    private void BuildEnable()
    {
        if (!_buildingEnable)
            return;

        //ī�޶󿡼� ���̸� �� ���� Ư�� ���̾ ���� ������Ʈ�� ���� ��� return
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(_ray, out _hit, 10, _layerMask))
            return;

        //ray�� ���� ��ġ�� �̸����� ������Ʈ ����
        Vector3 location = _hit.point;
        location.Set(Mathf.Round(location.x * 10f) * 0.1f, location.y, Mathf.Round(location.z * 10f) * 0.1f);
        _previewObj.transform.position = location;

        //Q, E Ű�� �Է¹޾� ������Ʈ ȸ��
        if (Input.GetKeyDown(KeyCode.Q))
            _previewObj.transform.eulerAngles -= Vector3.up * 30f;

        else if (Input.GetKeyDown(KeyCode.E))
            _previewObj.transform.eulerAngles += Vector3.up * 30f;

        //���� ���콺 ��Ŭ���� ������ ���� ��� return
        if (!Input.GetMouseButtonDown(0))
            return;

        //������ �������� Ȯ�� �� �ش� ��ġ�� ���๰ ����
        if (!_previewObj.isBuildable())
        {
            UIManager.Instance.ShowCenterText("�װ��� �Ǽ��� �� �����ϴ�.");
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
            UIManager.Instance.ShowCenterText("�Ǽ� ��ᰡ �����մϴ�.");
            return;
        }
    }
}
