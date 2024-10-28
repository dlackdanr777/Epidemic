using UnityEngine;


[CreateAssetMenu(fileName = "WeaponItemData", menuName = "Items/EquipItem/WeaponItemData")]
public class WeaponItemData : EquipmentItemData
{
    [SerializeField] private int _damage;
    public int Damage => _damage;

    [SerializeField] private float _range; //�����Ÿ�
    public float Range => _range;

    [SerializeField] private float _maxRecoil; //�ִ� �л굵
    public float MaxRecoil => _maxRecoil;

    [SerializeField] private float _minRecoil; //���� �л굵
    public float MinRecoil => _minRecoil;

    [SerializeField] private float _recoil; //�ݵ�
    public float Recoil => _recoil;

    [SerializeField] private float _recoilRecoveryAmount; //�ݵ�ȸ����
    public float RecoilRecoveryAmount => _recoilRecoveryAmount;

    [SerializeField] private float _rpm; //�д� �߻�ӵ�
    public float RPM => _rpm;

    [SerializeField] private float _reloadTime; //������ �ð�
    public float ReloadTime => _reloadTime;

    [SerializeField] private int _reloadBulletCount; //�Ѿ��� ������ ����
    public int ReloadBulletCount => _reloadBulletCount; 


    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
