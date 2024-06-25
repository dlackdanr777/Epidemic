
/// <summary> FSM의 상태를 정의한 인터페이스 </summary>
public interface IState
{
    /// <summary> 해당 상태가 시작되면 한번 실행하는 함수 </summary>
    public void OnStart();

    /// <summary> 해당 상태가 종료되면 한번 실행하는 함수 </summary>
    public void OnExit();

    /// <summary> 해당 상태일경우 주기적으로 실행하는 함수 </summary>
    public void OnUpdate();

    /// <summary> 해당 상태일경우 주기적으로 실행하는 함수 </summary>
    public void OnFixedUpdate();

    /// <summary> 해당 상태일경우 주기적으로 실행하며 상태 변화를 확인하는 함수 </summary>
    public void OnStateUpdate();
}

/// <summary> 플레이어 FSM의 상체 상태를 정의한 인터페이스 </summary>
public interface IUpperState : IState { }

/// <summary> 플레이어 FSM의 하체 상태를 정의한 인터페이스 </summary>
public interface ILowerState : IState { }
