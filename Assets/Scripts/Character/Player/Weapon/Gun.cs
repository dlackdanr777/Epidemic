using System;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    public string Name; //이름
    public float Range; //사정거리
    public float MaxRecoil; //최대 분산도
    public float MinRecoil; //최저 분산도
    public float Recoil; //반동
    public float RecoilRecoveryAmount; //반동회복량
    public float RPM; //분당 발사속도
    public float ReloadTime; //재장전 시간



    public int MaxBulletCount; //총알의 최대 갯수
    public int ReloadBulletCount; //총알의 재장전 갯수
    public int CurrentBulletCount; //탄창에 남아있는 총알의 갯수 

    public Animator Anim;
    public VisualEffect MuzzleFlash; //총구 화염 이펙트
    public AudioClip FireSound; //총 발사 소리
    public AudioClip ReloadSound;
    public Transform HandGrabTartget;

    public float FireRate; //총알 발사 간격
    public float Damage; //총의 데미지


    public void Awake()
    {
        Anim = GetComponent<Animator>();
    }


    public void Start()
    {
        FireRate = 1 / (RPM / 60);
    }



}
