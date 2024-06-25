
using System.Diagnostics;
/// <summary>조준중인 상태를 정의한 상태 클래스 </summary>
public class AimModeLoopState : PlayerUpperState
{
    public AimModeLoopState(Player player, PlayerStateMachine machine) : base(player, machine) { }


    public override void OnStart()
    {

    }


    public override void OnUpdate()
    {
        _player.OnFireHandler?.Invoke();
        _player.OnEnableAimHandler?.Invoke();
    }


    public override void OnFixedUpdate()
    {
        _player.OnRotateHandler?.Invoke();
    }


    public override void OnExit()
    {

    }


    public override void OnStateUpdate()
    {
        if (!_machine.AimModeEnable || _player.GunController.IsReload)
        {
            _machine.ChangeState(_machine.AimModeEndState);
        }

    }
}
