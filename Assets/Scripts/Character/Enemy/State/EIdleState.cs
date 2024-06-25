using UnityEngine;

/// <summary> 몬스터 대기 상태 클래스 </summary>
public class EIdleState : EnemyState
{
    private float _playSoundTime = 10;
    private float _playSoundTimer;

    public EIdleState(Enemy enemy, EnemyStateMachine machine) : base(enemy, machine){}


    public override void OnStart()
    {
        _playSoundTime = Random.Range(8f, 15f);
        _enemy.MeshController.Play("Idle");
        _enemy.OnTargetLossHandler();
    }


    public override void OnUpdate()
    {
    }


    public override void OnFixedUpdate()
    {
        _playSoundTimer += Time.deltaTime;
        PlaySound();
    }


    public override void OnStateUpdate()
    {
        if (_machine.ChangeTrackingStateCondition())
            _machine.ChangeState(_machine.IdleToTrackingState);
    }


    public override void OnExit()
    {
        _playSoundTimer = 0;
    }


    /// <summary> 일정 시간 마다 좀비 대기 상태 소리를 실행하는 함수 </summary>
    private void PlaySound()
    {
        if (_playSoundTimer <= _playSoundTime)
            return;

        _enemy.EnemySounds.PlayZombieSoundClip(EnemySoundType.Idle);
        _playSoundTime = Random.Range(8f, 15f);
        _playSoundTimer = 0;
    }

}
