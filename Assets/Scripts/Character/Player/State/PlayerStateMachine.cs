using UnityEngine;


/// <summary> 플레이어 병행상태기계를 관리, 조작하는 클래스 </summary>
public class PlayerStateMachine
{
    private Player _player;

    public float HorizontalInput;
    public float VerticalInput;
    public bool CrouchKeyPressed;
    public bool RunEnable;
    public bool AimModeEnable;

    //하체 상태
    public PlayerLowerState LowerCurrentState { get; private set; }
    public PlayerLowerState IdleState { get; private set; }
    public PlayerLowerState WalkState { get; private set; }
    public PlayerLowerState CrouchIdleState { get; private set; }
    public PlayerLowerState CrouchWalkState { get; private set; }
    public PlayerLowerState RunState { get; private set; }
    public PlayerLowerState DeadState { get; private set; }


    //상체 상태
    public PlayerUpperState UpperCurrentState { get; private set; }
    public PlayerUpperState BasicUpperState { get; private set; }
    public PlayerUpperState AimModeStartState { get; private set; }
    public PlayerUpperState AimModeLoopState { get; private set; }
    public PlayerUpperState AimModeEndState { get; private set; }
    public PlayerUpperState ReloadState { get; private set; }


    public void OnUpdate()
    {
        InputKey();
        LowerCurrentState.OnUpdate();
        LowerCurrentState.OnStateUpdate();

        UpperCurrentState.OnUpdate();
        UpperCurrentState.OnStateUpdate();
    }


    public void OnFixedUpdate()
    {
        LowerCurrentState.OnFixedUpdate();
        UpperCurrentState.OnFixedUpdate();
    }


    public PlayerStateMachine(Player player)
    {
        _player = player;
        StateInit();
    }


    private void StateInit()
    {
        IdleState = new IdleState(_player, this);
        WalkState = new WalkState(_player, this);
        CrouchIdleState = new CrouchIdleState(_player, this);
        CrouchWalkState = new CrouchWalkState(_player, this);
        RunState = new RunState(_player, this);
        DeadState = new DeadState(_player, this);

        BasicUpperState = new BasicUpperState(_player, this);
        AimModeStartState = new AimModeStartState(_player, this);
        AimModeLoopState = new AimModeLoopState(_player, this);
        AimModeEndState = new AimModeEndState(_player, this);
        ReloadState = new ReloadState(_player, this);
        
        LowerCurrentState = IdleState;
        UpperCurrentState = BasicUpperState;
    }


    public void InputKey()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        CrouchKeyPressed = Input.GetKeyDown(KeyCode.LeftControl);
        RunEnable = Input.GetKey(KeyCode.LeftShift) && VerticalInput > 0.1f && UpperCurrentState != ReloadState
            && LowerCurrentState != CrouchIdleState && LowerCurrentState != CrouchWalkState;
        AimModeEnable = Input.GetMouseButton(1) && LowerCurrentState != RunState && UpperCurrentState != ReloadState && !GameManager.Instance.CursorVisibled;
        //AimModeEnable = true;
    }


    /// <summary> 하체 상태를 변화시키는 함수 </summary>
    public void ChangeState(PlayerLowerState nextState)
    {
        if (LowerCurrentState == nextState) //현재상태와 입력받은 상태가 같을때에는 함수를 종료한다.
        {
            Debug.Log("현재 이미 해당 상태입니다.");
            return;
        }

        LowerCurrentState.OnExit(); //현재 상태의 OnExit를 호출
        nextState.OnStart(); //다음 상태의 OnStart를 호출
        LowerCurrentState = nextState; //현재 상태를 다음 상태로 변환
    }


    /// <summary> 상체 상태를 변화시키는 함수 </summary>
    public void ChangeState(PlayerUpperState nextState)
    {
        if (UpperCurrentState == nextState) //현재상태와 입력받은 상태가 같을때에는 함수를 종료한다.
        {
            Debug.Log("현재 이미 해당 상태입니다.");
            return;
        }

        UpperCurrentState.OnExit(); //현재 상태의 OnExit를 호출
        nextState.OnStart(); //다음 상태의 OnStart를 호출
        UpperCurrentState = nextState; //현재 상태를 다음 상태로 변환
    }


    public void ChangeToIdleState()
    {
        if (HorizontalInput == 0 && VerticalInput == 0)
            ChangeState(IdleState);
    }


    public void ChangeToWalkState()
    {
        if ((HorizontalInput != 0 || VerticalInput != 0) && !RunEnable)
            ChangeState(WalkState);
    }


    public void ChangeToRunState()
    {
        if (RunEnable)
            ChangeState(RunState);
    }


    public void ChangeToCrouchState()
    {
        if (CrouchKeyPressed)
            ChangeState(CrouchIdleState);
    }


    public void ChangeToAimModeState()
    {
        if (AimModeEnable)
            ChangeState(AimModeStartState);
    }

}
