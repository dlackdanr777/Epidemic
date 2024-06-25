
using UnityEngine;
/// <summary>조준 상태가 시작한 상태를 정의한 상태 클래스 </summary>
public class AimModeStartState : PlayerUpperState
{
    public AimModeStartState(Player player, PlayerStateMachine machine) : base(player, machine) { }

    public override void OnStart()
    {
        _player.Animator.SetBool("AimMode", true);
    }


    public override void OnUpdate(){}


    public override void OnFixedUpdate()
    {
        _player.OnRotateHandler?.Invoke();
        _player.OnEnableAimHandler?.Invoke();
        _player.Rigging.SetUpperRigWeight(1.1f);
    }


    public override void OnExit(){}


    public override void OnStateUpdate()
    {
        if (_player.PlayerCamera.ZoomIn())
        {
            _machine.ChangeState(_machine.AimModeLoopState);
        }

    }
}
