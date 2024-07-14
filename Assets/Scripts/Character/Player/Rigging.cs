using UnityEngine;
using UnityEngine.Animations.Rigging;


/// <summary> 상체의 애니메이션 리깅을 조절하는 함수 </summary>
public class Rigging : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MultiAimConstraint _spineAim1;
    [SerializeField] private MultiAimConstraint _spineAim2;
    [SerializeField] private MultiAimConstraint _headAim;
    [SerializeField] private MultiAimConstraint _headCamera;
    [SerializeField] private MultiAimConstraint _handAim;
    [SerializeField] private TwoBoneIKConstraint _secondHandGrabWeapon;


    /// <summary> 목표를 추적하는 뼈대의 가중치를 설정하는 함수 </summary>
    public bool StartUpperRigWeight()
    {
        float weightLerp = Mathf.Lerp(_spineAim1.weight, 1.1f, Time.deltaTime * 20);
        _spineAim1.weight = weightLerp;
        _spineAim2.weight = weightLerp;
        _headAim.weight = weightLerp;
        _handAim.weight = weightLerp;
        _secondHandGrabWeapon.weight = weightLerp;
        _headCamera.weight = Mathf.Abs(weightLerp - 1);

        return 1 <= weightLerp;
    }


    /// <summary> 목표를 추적하는 뼈대의 가중치를 설정하는 함수 </summary>
    public bool EndUpperRigWeight()
    {
        float weightLerp = Mathf.Lerp(_spineAim1.weight, -0.1f, Time.deltaTime * 20);
        _spineAim1.weight = weightLerp;
        _spineAim2.weight = weightLerp;
        _headAim.weight = weightLerp;
        _handAim.weight = weightLerp;
        _secondHandGrabWeapon.weight = weightLerp;
        _headCamera.weight = Mathf.Abs(weightLerp - 1);

        return weightLerp <= 0;
    }


    /// <summary> 목표를 추적하는 뼈대의 가중치를 머리만 설정하는 함수 </summary>
    public void SetHeadRigWeight(float weight)
    {
        if (_headAim.weight == weight)
            return;

        float weightLerp = Mathf.Lerp(_headAim.weight, weight, Time.deltaTime * 10);
        _headCamera.weight = weightLerp;
        _headAim.weight = Mathf.Abs(weightLerp - 1);
    }
}
