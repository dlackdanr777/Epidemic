
/// <summary> 앉아서 대기하는 상태를 정의한 클래스 </summary>
public class CrouchIdleState : PlayerLowerState
{
    public CrouchIdleState(Player player, PlayerStateMachine machine) : base(player, machine) { }


    public override void OnStart()
    {
        _player.Animator.SetBool("Crouch", true);
        _player.OnSetRecoilSizeHandler?.Invoke(0.5f);
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
        if ((_machine.HorizontalInput != 0 || _machine.VerticalInput != 0))
            _machine.ChangeState(_machine.CrouchWalkState);

        if (_machine.CrouchKeyPressed)
        {
            _machine.ChangeState(_machine.IdleState);
            _player.Animator.SetBool("Crouch", false);
        }

    }
}
