/// <summary> 죽은 상태를 정의한 클래스 </summary>
public class DeadState : PlayerLowerState
{
    public DeadState(Player player, PlayerStateMachine machine) : base(player, machine) { }


    public override void OnStart()
    {
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
