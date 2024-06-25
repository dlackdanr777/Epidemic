using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>피격 부위</summary>
public class HeadHitPart : MonoBehaviour, IHp
{
    [Header("Components")]
    [SerializeField] private Enemy _enemy;
    [SerializeField] private AnimatedMesh _animatedMesh;
    private MeshFilter _meshFilter;


    [Space]
    [Header("Options")]
    [SerializeField] private string _partName;
    [SerializeField] private float _multiple;

    public float Hp => _enemy.Hp;
    public float MaxHp => _enemy.MaxHp;
    public float MinHp => _enemy.MinHp;

    public event Action<float> OnHpChanged;
    public event Action<object, float> OnHpRecoverd;
    public event Action<object, float> OnHpDepleted;
    public event Action OnHpMax;
    public event Action OnHpMin;

    private int _tmpIndex = -1;
    private Vector3 _tmpPos;


    public void DepleteHp(object subject, float value)
    {
        _enemy.DepleteHp(subject, value * _multiple);
    }

    public void RecoverHp(object subject, float value)
    {

    }


    private void Start()
    {
        Init();
    }


    private void Init()
    {
        _meshFilter = _animatedMesh.GetComponent<MeshFilter>();

        _tmpIndex = FindClosestVertexIndex(transform.position, _meshFilter, _animatedMesh);
    }


    private int FindClosestVertexIndex(Vector3 position, MeshFilter meshFilter, AnimatedMesh animatedMesh)
    {
        List<Vector3> vertices = animatedMesh.GetMeshVertexList();

        int closestVertexIndex = 0;
        float minDistance = Vector3.Distance(position, meshFilter.transform.TransformPoint(vertices[0]));

        for (int i = 1; i < vertices.Count; i++)
        {
            float distance = Vector3.Distance(position, meshFilter.transform.TransformPoint(vertices[i]));
            if (distance < minDistance)
            {
                closestVertexIndex = i;
                minDistance = distance;
            }
        }

        _tmpPos = position - meshFilter.transform.TransformPoint(vertices[closestVertexIndex]);

        return closestVertexIndex;
    }

    private void Update()
    {
        if (_tmpIndex == -1)
            return;

        // 메시 정점 위치 갱신
        Vector3 worldPosition = _meshFilter.transform.TransformPoint(_animatedMesh.GetMeshVertex(_tmpIndex));
        transform.position = _tmpPos + worldPosition;
    }
}
