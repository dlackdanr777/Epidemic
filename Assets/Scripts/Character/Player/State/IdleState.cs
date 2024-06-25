/// <summary> 대기하는 상태를 정의한 클래스 </summary>
public class IdleState : PlayerLowerState
{

    public IdleState(Player player, PlayerStateMachine machine) : base(player, machine) { }


    public override void OnStart()
    {
        _player.OnSetRecoilSizeHandler?.Invoke(1f);
        _player.Animator.SetFloat("Horizontal", 0);
        _player.Animator.SetFloat("Vertical", 0);
    }


    public override void OnUpdate()
    {
    }


    public override void OnFixedUpdate()
    {
        if (_machine.UpperCurrentState == _machine.AimModeLoopState)
            _player.PlayerCamera.ZoomIn();
    }


    public override void OnExit()
    {
    }


    public override void OnStateUpdate()
    {
        _machine.ChangeToWalkState();
        _machine.ChangeToRunState();
        _machine.ChangeToCrouchState();
    }

}
