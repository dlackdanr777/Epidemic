
/// <summary> 걷는 상태를 정의한 클래스 </summary>
public class WalkState : PlayerLowerState
{
    public WalkState(Player player, PlayerStateMachine machine) : base(player, machine) { }

    public override void OnStart()
    {
        _player.OnSetRecoilSizeHandler?.Invoke(2f);
    }


    public override void OnUpdate()
    {
    }


    public override void OnFixedUpdate()
    {
        if (_machine.UpperCurrentState == _machine.AimModeLoopState)
            _player.OnMovedHandler?.Invoke(_machine.HorizontalInput, _machine.VerticalInput, 0.5f);
        else
            _player.OnMovedHandler?.Invoke(_machine.HorizontalInput, _machine.VerticalInput, 1);

        if (_machine.UpperCurrentState == _machine.AimModeLoopState)
            _player.PlayerCamera.ZoomIn();

        _player.OnRotateHandler?.Invoke();
    }


    public override void OnExit()
    {
    }


    public override void OnStateUpdate()
    {
        _machine.ChangeToIdleState();
        _machine.ChangeToRunState();
        _machine.ChangeToCrouchState();
    }
}
