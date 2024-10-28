using System;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{

    [SerializeField] private string _id; //ItemData Id값
    public string Id => _id;

    private int _damage;
    public int Damage => _damage;

    private float _range; //사정거리
    public float Range => _range;

    private float _maxRecoil; //최대 분산도
    public float MaxRecoil => _maxRecoil;

    private float _minRecoil; //최저 분산도
    public float MinRecoil => _minRecoil;

    private float _recoil; //반동
    public float Recoil => _recoil;

    private float _recoilRecoveryAmount; //반동회복량
    public float RecoilRecoveryAmount => _recoilRecoveryAmount;

    private float _rpm; //분당 발사속도
    public float RPM => _rpm;

    private float _reloadTime; //재장전 시간
    public float ReloadTime => _reloadTime;

    private int _reloadBulletCount; //총알의 재장전 갯수
    public int ReloadBulletCount => _reloadBulletCount;

    public VisualEffect MuzzleFlash; //총구 화염 이펙트
    public AudioClip FireSound; //총 발사 소리
    public AudioClip ReloadSound;
    public Transform HandGrabTartget;

    private float _fireRate; //총알 발사 간격
    public float FireRate => _fireRate;

    public void SetWeaponData(WeaponItemData data)
    {
        _damage = data.Damage;
        _range = data.Range;
        _maxRecoil = data.MaxRecoil;
        _minRecoil = data.MinRecoil;
        _recoil = data.Recoil;
        _recoilRecoveryAmount = data.RecoilRecoveryAmount;
        _rpm = data.RPM;
        _reloadTime = data.ReloadTime;
        _reloadBulletCount = data.ReloadBulletCount;

        _fireRate = 1 / (data.RPM / 60);
    }
}
