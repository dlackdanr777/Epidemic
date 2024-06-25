
/// <summary>조준이 끝난 상태를 정의한 상태 클래스 </summary>
public class AimModeEndState : PlayerUpperState
{
    public AimModeEndState(Player player, PlayerStateMachine machine) : base(player, machine) { }


    public override void OnStart()
    {
        _player.Animator.SetBool("AimMode", false);
        _player.OnDisableAimHandler?.Invoke();
    }


    public override void OnUpdate()
    {

    }


    public override void OnFixedUpdate()
    {
        _player.OnRotateHandler?.Invoke();
        _player.Rigging.SetUpperRigWeight(-0.1f);
    }


    public override void OnExit()
    {
    }


    public override void OnStateUpdate()
    {
        if (!_player.PlayerCamera.ZoomOut())
            return;

        if (_player.GunController.IsReload) _machine.ChangeState(_machine.ReloadState);
        else _machine.ChangeState(_machine.BasicUpperState);
    }
}
