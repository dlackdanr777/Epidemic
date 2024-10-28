using UnityEngine;


[CreateAssetMenu(fileName = "WeaponItemData", menuName = "Items/EquipItem/WeaponItemData")]
public class WeaponItemData : EquipmentItemData
{
    [SerializeField] private int _damage;
    public int Damage => _damage;

    [SerializeField] private float _range; //사정거리
    public float Range => _range;

    [SerializeField] private float _maxRecoil; //최대 분산도
    public float MaxRecoil => _maxRecoil;

    [SerializeField] private float _minRecoil; //최저 분산도
    public float MinRecoil => _minRecoil;

    [SerializeField] private float _recoil; //반동
    public float Recoil => _recoil;

    [SerializeField] private float _recoilRecoveryAmount; //반동회복량
    public float RecoilRecoveryAmount => _recoilRecoveryAmount;

    [SerializeField] private float _rpm; //분당 발사속도
    public float RPM => _rpm;

    [SerializeField] private float _reloadTime; //재장전 시간
    public float ReloadTime => _reloadTime;

    [SerializeField] private int _reloadBulletCount; //총알의 재장전 갯수
    public int ReloadBulletCount => _reloadBulletCount; 


    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
