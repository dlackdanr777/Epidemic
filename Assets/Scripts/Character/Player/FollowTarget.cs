using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private bool _targetRotationCopy;


    public void ChangeTarget(Transform target)
    {
        if(target == _target) return;
        if(target == null) return;

        _target = target;
    }

    private void Update()
    {
        transform.position = _target.position;

        if(_targetRotationCopy)
            transform.rotation = _target.rotation;
    }
}
