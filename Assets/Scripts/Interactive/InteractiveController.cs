using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary> IInteractive interface를 가진 오브젝트를 조작하는 클래스 </summary>
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


    /// <summary> Ray를 쏴 상호작용이 가능한 물체를 감지하는 함수 </summary>
    private void CheckInteractive()
    {
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        // 카메라 앞으로 레이를 쏜다.
        //만약 hit된 오브젝트가 없을 경우
        if (!Physics.Raycast(_ray, out _hit, _rayDistance, _layerMask))
        {
            //리스트의 크기가 0일 경우 리턴
            if (_interactiveList.Count <= 0)
                return;

            //아닐경우 비활성화 함수를 실행후 null로 변경
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
        //해당 오브젝트에서 Iinteractive 인터페이스를 참조후 활성화 함수 실행
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


    /// <summary> 키를 입력해 상호작용을 하는 함수 </summary>
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
