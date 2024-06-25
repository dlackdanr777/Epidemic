using System;


/// <summary>체력 관련 인터페이스</summary>
public interface IHp 
{
    float Hp { get; }
    float MaxHp { get; }
    float MinHp { get; }

    event Action<float> OnHpChanged;
    event Action<object, float> OnHpRecoverd;
    event Action<object, float> OnHpDepleted;
    event Action OnHpMax;
    event Action OnHpMin;

    /// <summary>체력을 회복하는 함수</summary>
    void RecoverHp(object subject, float value);

    /// <summary>체력을 감소시키는 함수</summary>
    void DepleteHp(object subject, float value);
}
