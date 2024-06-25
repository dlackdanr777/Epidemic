using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary> Gun class를 관리, 총기 관련 기능을 가지고 있는 클래스 </summary>
public class GunController : MonoBehaviour, IAttack
{
    public event Action OnFireHandler;
    public event Action OnReloadHandler;
    public event Action OnTargetDamaged;

    [Header("Components")]
    [SerializeField] private Gun _currentGun; //현재 들고있는 총
    [SerializeField] private CrossHair _crossHair;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private AudioSource _audioSource;

    [Header("Option")]
    [SerializeField] private LayerMask _hitLayerMask;


    private Vector3 fireDirection;
    private bool _isReload;
    public bool IsReload => _isReload;
    private float _currentFireRate; // 이값이 0이어야 총 발사 가능
    private float _nowRecoil; //현재 반동
    private float _recoilMul; //반동 배수
    private int _getCarryBulletCount => GameManager.Instance.Player.Inventory.FindItemCountByID(20);

    public float Damage => _currentGun.Damage;
    public int MaxBulletCount => _currentGun.MaxBulletCount;
    public int CurrentBulletCount => _currentGun.CurrentBulletCount;


    private void Start()
    {
        _nowRecoil = _currentGun.MinRecoil;
        _crossHair.Init(this);
    }


    private void Update()
    {
        if (GameManager.Instance.IsGameEnd)
            return;

        FireStabilization();
        GunFireRateCalc();
        TryReload();
    }


    public void TargetDamage(IHp ihp, float aomunt)
    {
        ihp.DepleteHp(this, aomunt);
        OnTargetDamaged?.Invoke();
    }


    /// <summary> 총기 발사 간격 쿨타임을 감소시키는 함수 </summary>
    private void GunFireRateCalc()
    {
        if (_currentFireRate <= 0)
            return;

        _currentFireRate -= Time.deltaTime;
    }


    /// <summary> 좌클릭시 총을 발사하게 해주는 함수 </summary>
    public void TryFire() 
    {
        if (Input.GetButton("Fire1") && _currentFireRate <= 0 && !_isReload)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (_isReload)
            return;

        if (_currentGun.CurrentBulletCount > 0)
        {
            Shoot();
        }
        else
        {
            StartCoroutine(ReloadRoutine());
        }
    }


    /// <summary> 방향, 탄착군을 계산하여 총알을 발사하는 함수 </summary>
    public void Shoot()
    {
        _currentFireRate = _currentGun.FireRate; //발사간의 딜레이를 현재 사용하는 총기의 딜레이로 설정한다
        _currentGun.CurrentBulletCount--; //총알을 감소시킨다
        PlaySound(_currentGun.FireSound); //발사 사운드 재생
        _currentGun.MuzzleFlash.Play(); //이펙트 재생
        _playerAnimator.SetTrigger("Fire"); //애니메이터의 트리거를 설정한다.

        float xError = GetRandomNormalDistribution(0f, _nowRecoil); //정규분포도를 이용해 탄튐 거리x를 설정
        float yError = GetRandomNormalDistribution(0f, _nowRecoil); //정규분포도를 이용해 탄튐 거리y를 설정

        //크로스헤어와 총구의 거리를 계산하고 최대사거리와 현재 거리의 비율을 계산한다.
        float targetDistance = Vector3.Distance(_currentGun.MuzzleFlash.transform.position, _crossHair.transform.position); 
        float distanceScale = targetDistance / _currentGun.Range;

        //위에서 계산한 값으로 총구와 탄착지점의 방향을 계산한다.
        //거리 비율을 곱하여 거리가 가까워질수록 탄착 지점을 좁혀 원뿔형의 형태로 탄이 튀도록 한다.
        fireDirection = _crossHair.transform.position - _currentGun.MuzzleFlash.transform.position;
        fireDirection = Quaternion.AngleAxis(yError * distanceScale, Vector3.up) * fireDirection;
        fireDirection = Quaternion.AngleAxis(xError * distanceScale, Vector3.right) * fireDirection;

        //총이 발사됬을때 설정한 반동값만큼 탄착지점의 넓이를 점차적으로 늘려 연속 발사시 명중률을 떨어트리게 한다.
        if(_nowRecoil < _currentGun.MaxRecoil)
        {
            _nowRecoil += _currentGun.Recoil;
            if (_nowRecoil > _currentGun.MaxRecoil)
                _nowRecoil = _currentGun.MaxRecoil;
        }

        RaycastHit hit;
        Ray ray = new Ray(_currentGun.MuzzleFlash.transform.position, fireDirection);
        float distance = _currentGun.Range;

        //대리자를 사용하여 이 클래스를 참조하는 클래스에서 발사함수에 코드를 추가할 수 있도록 한다.
        OnFireHandler?.Invoke();

        //최종적으로 레이를 발사하여 물체에 맞았을 경우 풀링한 탄흔을 해당위치에 소환하고
        //만약 IHp인터페이스를 가진 물체 였다면 액션을 수행하게 한다.
        if (Physics.Raycast(ray, out hit, distance, _hitLayerMask))
        {
            Quaternion bulletHoleRotation = Quaternion.LookRotation(ray.direction);
            GameObject bulletHole = ObjectPoolManager.Instance.SpawnBulletHole(hit.point, bulletHoleRotation);
            bulletHole.transform.parent = hit.transform;
            if (hit.transform.GetComponent<IHp>() != null)
            {
                TargetDamage(hit.transform.GetComponent<IHp>(), _currentGun.Damage);
            }

        }
    }


    /// <summary> 재장전을 시도하는 함수 </summary>
    public void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !_isReload &&_currentGun.CurrentBulletCount < _currentGun.ReloadBulletCount)
        {
            StartCoroutine(ReloadRoutine());
        }
    }


    /// <summary> 탄착군을 계산해 반환하는 함수 </summary>
    public float GetRandomNormalDistribution(float mean, float standard)
    {
        float x1 = Random.Range(0, 1f);
        float x2 = Random.Range(0, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }

    /// <summary> 크로스헤어 활성화 </summary>
    public void EnableCrossHair()
    {
        _crossHair.Visibility();
    }


    /// <summary> 크로스헤어 비활성화 </summary>
    public void DisableCrossHair()
    {
        _crossHair.Hidden();
    }


    /// <summary> 크로스헤어의 벌어짐 배수를 설정하는 함수 </summary>
    public void SetRecoilMul(float value)
    {
        _recoilMul = value;
    }



    /// <summary> 재장전 코루틴 </summary>
    private IEnumerator ReloadRoutine()
    {
        if(_getCarryBulletCount > 0)
        {
            _playerAnimator.SetTrigger("Reload");
            _isReload = true;
            AddCarryBullets(_currentGun.CurrentBulletCount);
            _currentGun.CurrentBulletCount = 0;
            _audioSource.PlayOneShot(_currentGun.ReloadSound);

            yield return new WaitForSeconds(_currentGun.ReloadTime);

            if(_getCarryBulletCount >= _currentGun.ReloadBulletCount)
            {
                _currentGun.CurrentBulletCount = _currentGun.ReloadBulletCount;
                SubCarryBullets(_currentGun.ReloadBulletCount);
            }
            else
            {
                _currentGun.CurrentBulletCount = _getCarryBulletCount;
                SubCarryBullets(_getCarryBulletCount);
            }
            _isReload = false;

            OnReloadHandler?.Invoke();
        }
        else
        {
            UIManager.Instance.ShowCenterText("총알이 부족합니다.");
        }
    }


    /// <summary> 사운드 재생 </summary>
    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.PlayOneShot(clip);
    }


    /// <summary> 반동 범위를 감소시키는 함수 </summary>
    private void FireStabilization()
    {
        if (_nowRecoil > _currentGun.MinRecoil * _recoilMul)
            _nowRecoil -= _currentGun.RecoilRecoveryAmount * Time.deltaTime;

        if (_nowRecoil < _currentGun.MinRecoil * _recoilMul)
            _nowRecoil = _currentGun.MinRecoil * _recoilMul;
    }


    /// <summary> 크로스헤어가 화면을 따라올 수 있게 하는 함수 </summary>
    public void FollowCrossHair()
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)); //카메라 정중앙에 레이를 위치시킨다
        float distance = _currentGun.Range;
        Debug.DrawRay(_currentGun.MuzzleFlash.transform.position, _currentGun.MuzzleFlash.transform.forward * distance, Color.red);

        //레이캐스트에 해당하는 레이어를 가진 오브젝트가 닿았을경우?
        if (Physics.Raycast(ray, out hit, distance, _hitLayerMask))
        {
            Vector3 hitPos = hit.point;

            // 카메라 현재 위치와 hit point간의 거리를 계산
            float hitDistance = Vector3.Distance(_mainCamera.transform.position, hit.point);

            // 크로스 헤어를 hit point에 위치하게 한다.
            _crossHair.transform.position = hitPos;

            // 거리 / 총기 사거리 로 거리 비율(0~1)을 계산한 뒤 크로스 헤어의 크기 및 넓이를 조절한다
            float distanceScale = hitDistance / distance;
            _crossHair.transform.localScale = Vector3.one * distanceScale;

            _crossHair.CrossHairAeraSize(_nowRecoil * distanceScale);
            _crossHair.transform.LookAt(_mainCamera.transform.position);
        }

        //아닐 경우?
        else
        {
            //크로스 헤어를 카메라 앞쪽에 배치하고 현재 반동 수치만큼 넓이를 넓힌다.
            _crossHair.transform.position = _mainCamera.transform.position + _mainCamera.transform.forward * distance;
            _crossHair.transform.localScale = Vector3.one;

            _crossHair.CrossHairAeraSize(_nowRecoil);
            _crossHair.transform.LookAt(_mainCamera.transform.position);
        }
    }


    /// <summary> 인벤토리 총알 감소 </summary>
    private void SubCarryBullets(int value)
    {
        GameManager.Instance.Player.Inventory.SubItemByID(20, value);
    }


    /// <summary> 인벤토리 총알 추가 </summary>
    private void AddCarryBullets(int value)
    {
        GameManager.Instance.Player.Inventory.AddItemByID(20, value);
    }

}