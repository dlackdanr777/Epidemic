using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 시야와 관련된 정보, 기능을 모아둔 클래스 </summary>
public class FieldOfView : MonoBehaviour
{
    [Header("Option")]
    [SerializeField] bool _debugMode = false;
    [SerializeField][Range(0f, 360f)] float _viewAngle = 0f;
    [SerializeField] float _viewRadius = 1f;
    [SerializeField] LayerMask _targetMask;
    [SerializeField] LayerMask _obstacleMask;

    private List<Collider> _hitTargetList = new List<Collider>();
    private Collider[] _targets = new Collider[0];
    private Coroutine _fieldOfViewDetectionRoutine;


    private void OnEnable()
    {
        if (_fieldOfViewDetectionRoutine != null)
            StopCoroutine(_fieldOfViewDetectionRoutine);

        _fieldOfViewDetectionRoutine = StartCoroutine(FieldOfViewDetection());
    }


    private void OnDisable()
    {
        if (_fieldOfViewDetectionRoutine != null)
            StopCoroutine(_fieldOfViewDetectionRoutine);
    }


    /// <summary> 시야각 안에 들어온 타겟 리스트중 첫번째를 반환하는 함수 </summary>
    public Transform GetTargetTransform()
    {
        if (0 < _hitTargetList.Count)
            return _hitTargetList[0].transform;

        return null;
    }


    /// <summary> 시야각안의 오브젝트중, 지정된 레이어들에 속하는 오브젝트를 찾아내 리스트에 넣는 함수 </summary>
    private IEnumerator FieldOfViewDetection()
    {
        while (true)
        {
            yield return YieldCache.WaitForSeconds(1f);
            _hitTargetList.Clear();

            //OverlapSphere를 사용, 현재 오브젝트의 일정 범위내의 지정한 레이어를 가진 오브젝트를 탐지
            Vector3 myPos = transform.position + Vector3.up;
            Vector3 lookDir = AngleToDir(transform.eulerAngles.y);
            _targets = Physics.OverlapSphere(myPos, _viewRadius, _targetMask);

            if (_targets.Length == 0)
                continue;

            //탐지한 오브젝트를 반복문으로 돌린다.
            foreach (Collider EnemyColli in _targets)
            {
                //감지된 오브젝트의 위치와 거리, 방향을 계산하고, 지정한 시야각 안에 들어온 상태라면 리스트에 추가한다.
                Vector3 targetPos = EnemyColli.transform.position + Vector3.up * 1.5f;
                float targetDistance = Vector3.Distance(myPos, targetPos);
                Vector3 targetDir = (targetPos - myPos).normalized;

                float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

                if (targetAngle <= _viewAngle * 0.5f && !Physics.Raycast(myPos, targetDir, targetDistance, _obstacleMask))
                {
                    _hitTargetList.Add(EnemyColli);
                }
            }

        }
    }


    /// <summary> 각도를 방향값으로 변경해 리턴하는 함수 </summary>
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }


    private void OnDrawGizmos()
    {
        if (!_debugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 1.5f;
        Gizmos.DrawWireSphere(myPos, _viewRadius);

        float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + _viewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - _viewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * _viewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * _viewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * _viewRadius, Color.cyan);

        if (_targets.Length < 0)
            return;

        foreach (Collider EnemyColli in _targets)
        {
            Vector3 targetPos = EnemyColli.transform.position + Vector3.up * 1.5f;
            float targetDistance = Vector3.Distance(myPos, targetPos);
            Vector3 targetDir = (targetPos - myPos).normalized;
            Debug.DrawRay(myPos, targetDir * targetDistance, Color.red);
        }
    }
}
