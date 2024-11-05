using UnityEngine;


/// <summary> 몬스터의 추적 상태를 정의한 클래스 </summary>
public class ETrackingState : EnemyState
{
    private float _playSoundTime = 0;
    private float _playSoundTimer;


    public ETrackingState(Enemy enemy, EnemyStateMachine machine) : base(enemy, machine){}


    public override void OnStart()
    {
        _enemy.MeshController.Play("Walk");
        _enemy.Navmesh.SetSpeed(_enemy.MoveSpeed);
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
        if (_machine.ChangeIdleStateCondition())
            _machine.ChangeState(_machine.IdleState);

        if (_machine.AttackStateCondition())
            _machine.ChangeState(_machine.AttackState);

    }


    public override void OnExit()
    {
        _playSoundTimer = 0;
    }


    /// <summary> 일정주기마다 소리를 재생하는 함수 </summary>
    private void PlaySound()
    {
        if (_playSoundTimer <= _playSoundTime)
            return;

        _enemy.EnemySounds.PlayZombieSoundClip(EnemySoundType.Tracking);
        _playSoundTime = Random.Range(4f, 5f);
        _playSoundTimer = 0;
    }
}
