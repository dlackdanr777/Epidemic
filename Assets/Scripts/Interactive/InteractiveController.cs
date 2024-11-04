using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary> IInteractive interface�� ���� ������Ʈ�� �����ϴ� Ŭ���� </summary>
public class InteractiveController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _layerMask;
    
    private Ray _ray;
    private RaycastHit _hit;
    private List<Iinteractive> _interactiveList = new List<Iinteractive>();
    private RaycastHit _tmpRaycastHit;

    private void Update()
    {
        CheckInteractive();
        InputInteractive();
    }


    /// <summary> Ray�� �� ��ȣ�ۿ��� ������ ��ü�� �����ϴ� �Լ� </summary>
    private void CheckInteractive()
    {
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        // ī�޶� ������ ���̸� ���.
        //���� hit�� ������Ʈ�� ���� ���
        if (!Physics.Raycast(_ray, out _hit, _rayDistance, _layerMask))
        {
            //����Ʈ�� ũ�Ⱑ 0�� ��� ����
            if (_interactiveList.Count <= 0)
                return;

            //�ƴҰ�� ��Ȱ��ȭ �Լ��� ������ null�� ����
            for(int i = 0, cnt = _interactiveList.Count; i < cnt; ++i)
            {
                _interactiveList[i].DisableInteraction();
            }
            _interactiveList.Clear();
            return;
        }
        
        if(!_hit.Equals(_tmpRaycastHit))
        {
            for (int i = 0, cnt = _interactiveList.Count; i < cnt; ++i)
            {
                _interactiveList[i].DisableInteraction();
            }
            _interactiveList.Clear();
        }

        Iinteractive[] tmpIinteractive = _hit.transform.GetComponents<Iinteractive>();
        //�ش� ������Ʈ���� Iinteractive �������̽��� ������ Ȱ��ȭ �Լ� ����
        for (int i = 0, cnt = tmpIinteractive.Length; i < cnt; ++i)
        {
            if (!_interactiveList.Contains(tmpIinteractive[i]))
            {
                tmpIinteractive[i].EnableInteraction();
                _interactiveList.Add(tmpIinteractive[i]);
                continue;
            }
        }

        _tmpRaycastHit = _hit;
    }


    /// <summary> Ű�� �Է��� ��ȣ�ۿ��� �ϴ� �Լ� </summary>
    private void InputInteractive()
    {
        if (_interactiveList.Count <= 0)
            return;

        for(int i = 0; i < _interactiveList.Count; ++i)
        {
            if (!Input.GetKeyDown(_interactiveList[i].InputKey))
                continue;

            _interactiveList[i].Interact();
            _interactiveList[i].DisableInteraction();
            _interactiveList.RemoveAt(i);
            i--;
        }


    }
}
