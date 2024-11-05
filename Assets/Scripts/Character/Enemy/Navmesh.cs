using System.Collections;
using UnityEngine;
using UnityEngine.AI;


/// <summary> 네비메쉬에이전트를 이용, 타겟에게 이동시키는 관련 정보, 기능을 모아둔 클래스 </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Navmesh : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _target;

    private Coroutine _coroutine;
    public void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }


    private void OnEnable()
    {
        NaveMeshEnabled(true);
        _coroutine = StartCoroutine(UpdateDestination());
    }


    private void OnDisable()
    {
        StopCoroutine(_coroutine);
    }

    
    public void NaveMeshEnabled(bool value)
    {
        _target = null;
        _agent.enabled = value;
    }


    public void StartNavMesh(Transform transform)
    {
        _target = transform;
        _agent.SetDestination(_target.position);
        _agent.isStopped = false;
    }

    public void StopNavMesh()
    {
        _target = null;
    }

    public void SetSpeed(float speed)
    {
        _agent.speed = speed;   
    }


    /// <summary> 이동 위치를 일정주기로 갱신하는 함수 </summary>
    private IEnumerator UpdateDestination()
    {
        while (true)
        {
            if(_target == null)
                _agent.SetDestination(transform.position);

            if(_target != null && _agent.enabled)
                _agent.SetDestination(_target.position);


            yield return YieldCache.WaitForSeconds(0.2f);
        }
    }

}
