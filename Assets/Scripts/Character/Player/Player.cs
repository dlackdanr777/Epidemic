using System;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary> 플레이어 관련 데이터, 컴포넌트를 모아둔 클래스 </summary>
public class Player : MonoBehaviour, IHp
{
    public event Action OnHpMax;
    public event Action OnHpMin;
    public event Action<float> OnHpChanged;
    public event Action<object, float> OnHpRecoverd;
    public event Action<object, float> OnHpDepleted;
    public Action<float, float, float> OnMovedHandler;
    public Action<float> OnSetRecoilSizeHandler;
    public Action OnRotateHandler;
    public Action OnEnableAimHandler;
    public Action OnDisableAimHandler;
    public Action OnFollowAimHandler;
    public Action OnFireHandler;


    [Header("Components")]
    public GridInventory Inventory;
    public Camera MainCamera;
    public CinemachineCamera PlayerCamera;
    public Animator Animator;
    public GunController GunController;
    public Rigging Rigging;
    public PlayerMovement PlayerMovement;
    public FlashLight FlashLight;
    public PlayerStateMachine Machine;
    public CharacterController CharacterController;
    public AudioSource AudioSource;
    [SerializeField] private AudioClip[] _hitSoundClips;
    [SerializeField] private float _maxHp;
    public float MaxHp => _maxHp;
    [SerializeField] private float _minHp = 0;
    public float MinHp => _minHp;


    private float _hp;
    public float Hp
    {
         get => _hp;
        private set
        {
            if (_hp == value)
                return;

            if(value > _maxHp)
                value = _maxHp;

            else if(value < _minHp)
                value = _minHp;

            _hp = value;
            OnHpChanged?.Invoke(value);
            if (_hp == _maxHp)
                OnHpMax?.Invoke();

            else if(_hp == _minHp)
                OnHpMin?.Invoke();
        }
    }


    private void Awake()
    {
        Machine = new PlayerStateMachine(this);
        GameManager.Instance.Player = this;
    }


    private void Start()
    {  
        Init();
        ActionInit();
        GunController.DisableCrossHair();
    }



    private void Update()
    {
        if (GameManager.Instance.IsGameEnd)
            return;

        Machine.OnUpdate();
        OnFollowAimHandler?.Invoke();
        FlashLight.ControllFlash();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameEnd)
            return;

        Machine.OnFixedUpdate();
    }


    private void Init()
    {
        _hp = _maxHp;
    }


    private void ActionInit()
    {
        OnRotateHandler = PlayerMovement.Rotate;      
        
        OnMovedHandler = PlayerMovement.Movement;
        OnMovedHandler += (horizontal, vertical, speedMul) =>
        {
            Animator.SetFloat("Vertical", vertical);
            Animator.SetFloat("Horizontal", horizontal);
        };

        OnEnableAimHandler = GunController.EnableCrossHair;
        OnDisableAimHandler = GunController.DisableCrossHair;
        OnFollowAimHandler = GunController.FollowCrossHair;

        OnFireHandler = GunController.TryFire;
        OnSetRecoilSizeHandler = GunController.SetRecoilMul;

        GunController.OnFireHandler += PlayerCamera.CameraShakeStart;

        OnHpMin += () =>
        {
            if (!GameManager.Instance.IsGameEnd)
            {
                Animator.SetBool("IsDead", true);
                GameManager.Instance.IsGameEnd = true;
                GameManager.Instance.CursorVisible();
                Machine.ChangeState(Machine.DeadState);
                CharacterController.enabled = false;
            }
        };
    }


    public void RecoverHp(object subject, float value)
    {
        Hp += value;
        OnHpRecoverd?.Invoke(subject, value);
    }


    public void DepleteHp(object subject, float value)
    {
        value -= value * UserInfo.Armor * 0.01f;
        Hp -= value;
        int randIndex = Random.Range(0, _hitSoundClips.Length);
        AudioSource.PlayOneShot(_hitSoundClips[randIndex]);
        OnHpDepleted?.Invoke(subject, value);
    }

}

