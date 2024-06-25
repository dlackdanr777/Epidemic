using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>피격 부위</summary>
public class HitPart : MonoBehaviour, IHp
{
    [Header("Components")]
    [SerializeField] private Enemy _enemy;

    [Space]
    [Header("Options")]
    [SerializeField] private string _partName;
    [SerializeField] private float _multiple;

    public float Hp => _enemy.Hp;
    public float MaxHp => _enemy.MaxHp;
    public float MinHp => _enemy.MinHp;

    public event Action<float> OnHpChanged;
    public event Action<object, float> OnHpRecoverd;
    public event Action<object, float> OnHpDepleted;
    public event Action OnHpMax;
    public event Action OnHpMin;


    public void DepleteHp(object subject, float value)
    {
        _enemy.DepleteHp(subject, value * _multiple);
    }

    public void RecoverHp(object subject, float value)
    {

    }
}
