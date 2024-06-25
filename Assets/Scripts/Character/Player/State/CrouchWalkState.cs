
/// <summary> 앉아서 걷는 상태를 정의한 클래스 </summary>
public class CrouchWalkState : PlayerLowerState
{
    public CrouchWalkState(Player player, PlayerStateMachine machine) : base(player, machine) { }

    public override void OnStart()
    {
        _player.OnSetRecoilSizeHandler?.Invoke(1f);
    }


    public override void OnUpdate()
    {

    }


    public override void OnFixedUpdate()
    {
        _player.OnMovedHandler?.Invoke(_machine.HorizontalInput, _machine.VerticalInput, 0.5f);

        _player.OnRotateHandler?.Invoke();

        if (_machine.UpperCurrentState == _machine.AimModeLoopState)
            _player.PlayerCamera.ZoomIn();
    }


    public override void OnExit()
    {

    }


    public override void OnStateUpdate()
    {
        if (_machine.HorizontalInput == 0 && _machine.VerticalInput == 0)
            _machine.ChangeState(_machine.CrouchIdleState);

        if (_machine.CrouchKeyPressed)
        {
            _machine.ChangeState(_machine.IdleState);
            _player.Animator.SetBool("Crouch", false);
        }
    }
}
