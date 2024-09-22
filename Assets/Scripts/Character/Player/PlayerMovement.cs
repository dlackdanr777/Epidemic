using System;
using UnityEngine;


/// <summary> 플레이어의 움직임을 주는 클래스 </summary>
public class PlayerMovement : MonoBehaviour, IMovement
{
    [Header("Components")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Camera _mainCamera;

    [Space]
    [Header("Option")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _runSpeedMul;
    public float RunSpeedMul => _runSpeedMul;
    [SerializeField] private float _rotateSpeed;


    public void Movement(float horizontalInput, float verticalInput, float moveSpeedMul)
    {
        Vector3 moveDir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        moveDir = transform.TransformDirection(moveDir) * _moveSpeed * moveSpeedMul * UserInfo.Speed;
        _controller.Move(moveDir * Time.deltaTime);
    }


    public void Rotate()
    {
        Vector3 cameraRotation = new Vector3(0, _mainCamera.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(cameraRotation), Time.deltaTime * _rotateSpeed);
    }
}
