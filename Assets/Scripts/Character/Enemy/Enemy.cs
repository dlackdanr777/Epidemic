using System;
using System.Collections;
using UnityEngine;


/// <summary>몬스터와 관련된 컴포넌트, 정보를 관리하는 클래스</summary>
public class Enemy : MonoBehaviour, IHp
{
    public event Action<float> OnHpChanged;
    public event Action<object, float> OnHpRecoverd;
    public event Action<object, float> OnHpDepleted;
    public event Action<Vector3> OnRotatedHandler;
    public event Action OnHpMax;
    public event Action OnHpMin;
    public event Action OnTargetDamaged;
    public Action OnTargetLossHandler;
    public Action OnTargetFollowedHandler;

    [Header("Components")]
    public AudioSource AudioSource;
    public Navmesh Navmesh;
    public EnemySounds EnemySounds;
    public FieldOfView FieldOfView;
    public EnemyAttack Attack;
    public AnimatedMeshController MeshController;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private ParticleSystem _deadBloodParticle;

    [Space]
    [Header("Status")]
    [SerializeField] private EnemyData _enemyData;

    public EnemyType EnemyType => _enemyData.EnemyType;
    public float MaxHp => _enemyData.MaxHp;
    public float MinHp => _enemyData.MinHp;

    [SerializeField] private int[] _attackIndex;
    public int[] AttackIndex => _attackIndex;

    private float _hp;
    public float Hp => _hp;

    private bool _isDead;
    public bool IsDead => _isDead;
    private EnemyStateMachine _machine;
    [HideInInspector] public Transform Target;


    public void RecoverHp(object subject, float value)
    {
        if (_isDead)
            return;

        _hp += value;
        OnHpRecoverd?.Invoke(subject, value);
    }


    public void DepleteHp(object subject, float value)
    {
        if (_isDead)
            return;

        if (subject is Player)
        {
            Player player = (Player)subject;
            Target = player.transform;
        }

        _hp -= value;

        if (_hp < 0)
        {
            value = MinHp;
            OnHpMin?.Invoke();
        }

        OnHpChanged?.Invoke(value);
        OnHpDepleted?.Invoke(subject, value);
    }


    private void Awake()
    {
        _machine = new EnemyStateMachine(this);
    }


    private void Start()
    {
        Init();
        ActionInit();
    }


    private void Update()
    {
        if (_isDead && GameManager.Instance.IsGameEnd)
            return;

        _machine.OnUpdate();
    }


    private void FixedUpdate()
    {
        if (_isDead && GameManager.Instance.IsGameEnd)
            return;

        _machine.OnFixedUpdate();
    }

    private void OnEnable()
    {
        Init();
    }


    private void Init()
    {
        _hp = MaxHp;

        _isDead = false;
        _capsuleCollider.enabled = true;
        _machine.ChangeState(_machine.IdleState);
        MeshController.Play("Idle");
    }


    private void ActionInit()
    {
        OnHpMin = () =>
        {
            if (!_isDead)
            {
                StartCoroutine(ObjectPoolManager.Instance.DisableZombie(this));
                StartCoroutine(StartDeadParticle());
                EnemySounds.PlayZombieSoundClip(EnemySoundType.Dead);
                Navmesh.NaveMeshEnabled(false);
                _capsuleCollider.enabled = false;
                _machine.ChangeState(_machine.DeadState);

                MeshController.Play("Die");
                Target = null;
                _isDead = true;
            }
        };

        OnTargetFollowedHandler = () => { if (Target != null) Navmesh.StartNavMesh(Target); };
        OnTargetLossHandler = () => { if (Target == null) Navmesh.StopNavMesh(); };
    }


    /// <summary>사망 시 파티클을 실행하는 코루틴</summary>
    private IEnumerator StartDeadParticle()
    {
        yield return YieldCache.WaitForSeconds(3);
        _deadBloodParticle.Emit(3);
    }
}
