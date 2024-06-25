using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>몬스터 공격 관련 정보, 함수를 보관한 함수</summary>
public class EnemyAttack : MonoBehaviour, IAttack
{

    [Header("Option")]
    [SerializeField] private float _damage;
    public float Damage => _damage;
    [SerializeField] private float _attackRadius;
    public float attackRadius => _attackRadius;
    [SerializeField] private LayerMask _attackLayerMask;
    [SerializeField] private LayerMask _obstacleMask;

    public event Action OnTargetDamaged;
    public event Action OnAttackHendler;
    private Coroutine CheckPlayerAtAttackRangeRoutine;
    private List<Collider> _hitColliderList = new List<Collider>();


    private void OnEnable()
    {
        //스크립트 활성화시 공격범위 내의 적을 탐지하는 코루틴 실행
        if (CheckPlayerAtAttackRangeRoutine != null)
            StopCoroutine(CheckPlayerAtAttackRangeRoutine);

        CheckPlayerAtAttackRangeRoutine = StartCoroutine(CheckPlayerAtAttackRange(1f));
    }


    private void OnDisable()
    {
        if (CheckPlayerAtAttackRangeRoutine != null)
            StopCoroutine(CheckPlayerAtAttackRangeRoutine);
    }


    /// <summary>해당 IHp를 가진 타겟에게 데미지를 주는 함수</summary>
    public void TargetDamage(IHp iHp, float aomunt)
    {
        if (iHp.Hp < iHp.MinHp)
            return;

        iHp.DepleteHp(this, aomunt);
        OnTargetDamaged?.Invoke();
    }


    /// <summary>공격 가능한 상태인지 반환하는 함수</summary>
    public bool GetCheckPlayerAtAttackRange()
    {

        if (0 < _hitColliderList.Count)
            return true;

        return false;
    }


    /// <summary>공격 가능 리스트를 순회하며 IHp 인터페이스가 존재하면 DepleteHp()를 실행하는 함수(애니메이션 이벤트로 실행)</summary>
    public void StartAttack()
    {
        foreach (Collider collider in _hitColliderList)
        {
            //다수 타격은 하지 않기에 리스트속 IHp를 가진 오브젝트 하나만 공격 후 함수를 종료 한다

            if (!collider.TryGetComponent(out IHp iHp))
                continue;

            iHp.DepleteHp(this, _damage);
            OnAttackHendler?.Invoke();

            return;
        }
    }


    public void OnDrawGizmos()
    {
        Vector3 attackPos = transform.position + transform.up + (transform.forward * attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos, attackRadius);
    }


    /// <summary>공격 범위 내 콜라이더들을 리스트에 넣는 함수</summary>
    private IEnumerator CheckPlayerAtAttackRange(float repeatCycle)
    {
        while (true)
        {
            yield return YieldCache.WaitForSeconds(repeatCycle);
            _hitColliderList.Clear();

            //공격 범위안에 타겟이 있는지 확인 후 없을 경우 아무것도 하지 않는다.
            Vector3 attackPos = transform.position + transform.up + (transform.forward * attackRadius);
            Vector3 myPos = transform.position + Vector3.up;
            Collider[] Targets = Physics.OverlapSphere(attackPos, attackRadius, _attackLayerMask);
            if (Targets.Length <= 0)
                continue;

            //타겟이 있을경우 해당 타겟 전부 순회. 
            foreach (Collider EnemyColli in Targets)
            {
                //해당 타겟으로 레이를 쏴 장애물이 있는지 확인 후 없을 경우 공격 리스트에 추가
                Vector3 targetPos = EnemyColli.transform.position + Vector3.up;
                Vector3 targetDir = (targetPos - myPos).normalized;
                float targetDistance = Vector3.Distance(targetPos, myPos);

                if (!Physics.Raycast(myPos, targetDir, targetDistance, _obstacleMask))
                {
                    _hitColliderList.Add(EnemyColli);
                }
            }
        }
    }
}
