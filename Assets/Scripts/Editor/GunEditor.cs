using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[CanEditMultipleObjects]
[CustomEditor(typeof(Gun), true)]
public class GunEditor : Editor
{
    private Gun _gun;

    private void OnEnable()
    {
        _gun = target as Gun;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("총기 설정");
        EditorGUILayout.Space();
        _gun.Name = EditorGUILayout.TextField("이름", _gun.Name);
        _gun.Range = EditorGUILayout.FloatField("사정거리", _gun.Range);
        _gun.MinRecoil = EditorGUILayout.FloatField("최저 반동", _gun.MinRecoil);
        _gun.MaxRecoil = EditorGUILayout.FloatField("최대 반동", _gun.MaxRecoil);
        _gun.Recoil = EditorGUILayout.Slider("총기 반동", _gun.Recoil, 0.01f, 1f);
        _gun.RecoilRecoveryAmount = EditorGUILayout.Slider("반동 회복량", _gun.RecoilRecoveryAmount, 0.1f, 1f);
        _gun.RPM = EditorGUILayout.FloatField("분당 발사속도", _gun.RPM);
        _gun.ReloadTime = EditorGUILayout.FloatField("재장전 시간(S)", _gun.ReloadTime);

        EditorGUILayout.Space();
        _gun.Damage = EditorGUILayout.FloatField("발당 데미지", _gun.Damage);

        EditorGUILayout.Space();
        _gun.MaxBulletCount = EditorGUILayout.IntField("탄창 최대 총알 수량", _gun.MaxBulletCount);
        _gun.ReloadBulletCount = EditorGUILayout.IntField("재장전 총알 수량" , _gun.ReloadBulletCount);

        EditorGUILayout.Space();
        _gun.MuzzleFlash = (VisualEffect)EditorGUILayout.ObjectField("총구 화염 이펙트", _gun.MuzzleFlash, typeof(VisualEffect), true);
        _gun.FireSound = (AudioClip)EditorGUILayout.ObjectField("발사 사운드", _gun.FireSound, typeof(AudioClip), true);
        _gun.ReloadSound = (AudioClip)EditorGUILayout.ObjectField("재장전 사운드", _gun.ReloadSound, typeof(AudioClip), true);
        _gun.HandGrabTartget = (Transform)EditorGUILayout.ObjectField("왼손 위치", _gun.HandGrabTartget, typeof(Transform), true);

        EditorGUILayout.LabelField("탄약수: " + _gun.CurrentBulletCount);

        EditorUtility.SetDirty(_gun);
    }
}
