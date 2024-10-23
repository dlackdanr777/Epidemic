using UnityEngine;

/// <summary> 몬스터AI를 관리하는 클래스(FSM) </summary>
public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }
    public EnemyState IdleState { get; private set; }
    public EnemyState IdleToTrackingState { get; private set; }
    public EnemyState TrackingState { get; private set; }
    public EnemyState AttackState { get; private set; }
    public EnemyState DeadState { get; private set; }

    private Enemy _enemy;
    private float _changeTimer;


    public EnemyStateMachine(Enemy enemy)
    {
        _enemy = enemy;
        StateInit();
    }


    public void OnUpdate()
    {
        CurrentState.OnStateUpdate();
        CurrentState.OnUpdate();
    }


    public void OnFixedUpdate()
    {
        CurrentState.OnFixedUpdate();
    }
    

    private void StateInit()
    {
        IdleState = new EIdleState(_enemy, this);
        IdleToTrackingState = new EIdleToTrackingState(_enemy, this);
        TrackingState = new ETrackingState(_enemy, this);
        AttackState = new EAttackState(_enemy, this);
        DeadState = new EDeadState(_enemy, this);
        CurrentState = IdleState;
    }


    /// <summary>상태를 변환하는 함수(꼭이걸로 상태를 변화해야함)</summary>
    public void ChangeState(EnemyState nextState) 
    {
        if (CurrentState == nextState) //현재상태와 입력받은 상태가 같을때에는 함수를 종료한다.
        {
            Debug.Log("현재 이미 해당 상태입니다.");
            return;
        }

        CurrentState.OnExit(); //현재 상태의 OnExit를 호출
        nextState.OnStart(); //다음 상태의 OnStart를 호출
        CurrentState = nextState; //현재 상태를 다음 상태로 변환
    }


    /// <summary> 추적 상태로 전이할 수 있으면 참, 아니면 거짓을 반환하는 함수 </summary>
    public bool ChangeTrackingStateCondition()
    {
        if(_enemy.FieldOfView.GetTargetTransform() != null)
        {
            _enemy.Target = _enemy.FieldOfView.GetTargetTransform();
            return true;
        }
        else
        {
            if(_enemy.Target != null)
                return true;

            return false;
        }
    }


    /// <summary> 대기 상태로 전이할 수 있으면 참, 아니면 거짓을 반환하는 함수 </summary>
    public bool ChangeIdleStateCondition()
    {
        if (_enemy.FieldOfView.GetTargetTransform() == null)
        {
            _changeTimer += Time.deltaTime;
            if (100 < _changeTimer)
            {
                _enemy.Target = _enemy.FieldOfView.GetTargetTransform();
                return true;
            }
        }
        else
        {
            _changeTimer = 0;
        }
            return false;
    }


    /// <summary> 공격 상태로 전이할 수 있으면 참, 아니면 거짓을 반환하는 함수 </summary>
    public bool AttackStateCondition()
    {
        return _enemy.Attack.GetCheckPlayerAtAttackRange();
    }
}
