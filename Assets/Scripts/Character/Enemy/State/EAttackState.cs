using UnityEngine;


/// <summary> 몬스터의 공격 상태를 정의한 클래스 </summary>
public class EAttackState : EnemyState
{
    private float _playSoundTime = 10;
    private float _playSoundTimer;
    private bool[] _useAttack;


    public EAttackState(Enemy enemy, EnemyStateMachine machine) : base(enemy, machine){}


    public override void OnStart()
    {
        _useAttack = new bool[_enemy.AttackIndex.Length];
        _enemy.MeshController.Play("Attack");
        _enemy.EnemySounds.PlayZombieSoundClip(EnemySoundType.Attack);
    }


    public override void OnUpdate()
    {
        if (_enemy.MeshController.GetIndex() <= 0)
        {
            for (int i = 0, cnt = _useAttack.Length; i < cnt; ++i)
                _useAttack[i] = false;
        }

        for (int i = 0, cnt = _enemy.AttackIndex.Length; i < cnt; ++i)
        {
            if (!_useAttack[i] && _enemy.AttackIndex[i] == _enemy.MeshController.GetIndex())
            {
                _useAttack[i] = true;
                _enemy.Attack.StartAttack();
            }

        }
    }


    public override void OnFixedUpdate()
    {
        _enemy.OnTargetFollowedHandler?.Invoke();
        PlaySound();
    }


    public override void OnStateUpdate()
    {
        if (!_machine.AttackStateCondition())
            _machine.ChangeState(_machine.TrackingState);
    }


    public override void OnExit()
    {
        _enemy.MeshController.Play("Idle");
    }


    /// <summary> 몬스터 소리 재생 함수 </summary>
    private void PlaySound()
    {
        if (_playSoundTimer <= _playSoundTime)
            return;

        _enemy.EnemySounds.PlayZombieSoundClip(EnemySoundType.Idle);
        _playSoundTime = Random.Range(3f, 4f);
        _playSoundTimer = 0;
    }
}
