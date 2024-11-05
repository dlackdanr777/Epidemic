
/// <summary> ������ ��� ���¸� ������ Ŭ���� </summary>
public class EDeadState : EnemyState
{
    public EDeadState(Enemy enemy, EnemyStateMachine machine) : base(enemy, machine){}

    public override void OnStart()
    {
        _enemy.Navmesh.SetSpeed(0);
        _enemy.Navmesh.StopNavMesh();
    }


    public override void OnUpdate()
    {
    }


    public override void OnFixedUpdate()
    {
    }


    public override void OnStateUpdate()
    {
    }


    public override void OnExit()
    {
    }

}
