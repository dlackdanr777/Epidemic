using System;

/// <summary>공격 관련 인터페이스</summary>
public interface IAttack
{
    float Damage { get; }

    event Action OnTargetDamaged;

    /// <summary>IHp인터페이스의 DepleteHp를 실행시켜 체력을 감소시키는 함수</summary>
    void TargetDamage(IHp iHp, float aomunt);
}
