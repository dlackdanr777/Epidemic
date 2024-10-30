using System;
using System.Collections;
using UnityEngine;


public class Barricade : MonoBehaviour, IHp
{
    public float Hp
    {
        get => _hp;
        private set
        {
            if (_hp == value)
                return;

            _hp = Mathf.Clamp(value, _minHp, _maxHp);

            _hp = value;
            OnHpChanged?.Invoke(value);
            if (_hp == _maxHp)
                OnHpMax?.Invoke();
            else if (_hp == _minHp)
                OnHpMin?.Invoke();
        }
    }

    public float MaxHp => _maxHp;

    public float MinHp => _minHp;

    public event Action<float> OnHpChanged;
    public event Action<object, float> OnHpRecoverd;
    public event Action<object, float> OnHpDepleted;
    public event Action OnHpMax;
    public event Action OnHpMin;

    private float _hp;
    [SerializeField] private GameObject _targetObject;
    [SerializeField] protected float _maxHp;
    [SerializeField] protected float _minHp = 0;


    public void Awake()
    {
        Hp = _maxHp;
    }


    public void SetHp(float value)
    {
        if (value == 0)
        {
            OnHpMinEvcent();
            return;
        }

        Hp = value;
    }

    public void DepleteHp(object subject, float value)
    {
        Hp -= value;
        OnHpDepleted?.Invoke(subject, value);
    }


    public void RecoverHp(object subject, float value)
    {
        OnHpRecoverd?.Invoke(subject, value);
    }


    protected virtual void OnHpMinEvcent()
    {
        Destroy(_targetObject);
    }
}
