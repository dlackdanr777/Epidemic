using UnityEngine;


/// <summary> 몬스터의 대기에서 추적 상태로 넘어가는 상태를 정의한 클래스 </summary>
public class EIdleToTrackingState : EnemyState
{

    private float _rotateTime = 10;
    private float _transitionTime = 10;
    private float _transitionTimer;
    private float _rotateTimer;

    public EIdleToTrackingState(Enemy enemy, EnemyStateMachine machine) : base(enemy, machine){}


    public override void OnStart()
    {
        _enemy.EnemySounds.PlayZombieSoundClip(EnemySoundType.Detection);
        _enemy.MeshController.Play("Walk");

        _rotateTime = Random.Range(0.3f, 1f);
        _transitionTime = Random.Range(3f, 4f);
    }


    public override void OnUpdate()
    {
        _rotateTimer += Time.deltaTime;
        _transitionTimer += Time.deltaTime;
    }


    public override void OnFixedUpdate()
    {
        if (_rotateTimer <= _rotateTime)
            return;

        Vector3 targetDir = _enemy.Target.transform.position - _enemy.transform.position;
        _enemy.transform.rotation = Quaternion.Lerp(_enemy.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * 0.5f);
    }


    public override void OnStateUpdate()
    {
        if (_transitionTimer <= _transitionTime)
            return;

        _machine.ChangeState(_machine.TrackingState);
    }


    public override void OnExit()
    {
        _transitionTimer = 0;
        _rotateTimer = 0;
    }
}
