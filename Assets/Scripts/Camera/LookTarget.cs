using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _target;


    public void LookAtTarget()
    {
        transform.LookAt(_target);
    }
}
