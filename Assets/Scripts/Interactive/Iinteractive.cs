using UnityEngine;


/// <summary> 상호작용을 위한 인터페이스 </summary>
public interface Iinteractive 
{
    /// <summary> 해당 키를 눌러야 상호작용이 가능 </summary>
    KeyCode InputKey { get; }

    /// <summary> 상호작용됬을때 실행되는 함수 </summary>
    void Interact();

    /// <summary> 상호작용이 가능할때 실행되는 함수 </summary>
    void EnableInteraction();

    /// <summary> 상호작용이 불가능할때 실행되는 함수 </summary>
    void DisableInteraction();
}
