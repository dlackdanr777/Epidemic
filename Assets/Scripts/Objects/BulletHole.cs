using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BulletHole : MonoBehaviour
{
    [SerializeField] private AudioSource _AudioSource;
    [SerializeField] private AudioClip _hitPartClip;
    [SerializeField] private AudioClip _groundClip;
    [SerializeField] private float _disableTime;

    [SerializeField] private ParticleSystem _bloodParticle;
    [SerializeField] private ParticleSystem _bloodCloudParticle;

    private MeshFilter _meshFilter;
    private AnimatedMesh _animatedMesh;
    private int _initialVertexIndex;
    private bool _isAttachedToMesh = false;


    private void OnEnable()
    {
        StartCoroutine(BulletHoleDisable(_disableTime));
        StartCoroutine(TargetHit());
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
        _isAttachedToMesh = false;
    }

    private IEnumerator BulletHoleDisable(float time)
    {
        yield return YieldCache.WaitForSeconds(time);
        ObjectPoolManager.Instance.DespawnBulletHole(gameObject);
    }

    private IEnumerator TargetHit()
    {
        yield return YieldCache.WaitForSeconds(0.01f);

        if (transform.parent.CompareTag("HitPart") || transform.parent.CompareTag("Enemy"))
        {
            Transform tr = transform;

            while (tr != null)
            {
                if (tr.GetComponent<MeshFilter>() == null)
                {
                    tr = tr.parent;
                    continue;
                }

                _meshFilter = tr.GetComponent<MeshFilter>();
                break;

            }

            if (tr == null)
                yield break;

            transform.parent = tr;
            _animatedMesh = transform.parent.GetComponent<AnimatedMesh>();
            _isAttachedToMesh = true;
            // 메시의 정점 중 하나를 초기 위치로 설정 (가장 가까운 정점 찾기)
            _initialVertexIndex = FindClosestVertexIndex(transform.position, _meshFilter, _animatedMesh);

            _AudioSource.PlayOneShot(_hitPartClip);
            PlayParticle(2);
            yield return YieldCache.WaitForSeconds(0.2f);
            PlayParticle(1);
        }
        else if (transform.parent.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _AudioSource.PlayOneShot(_groundClip);
        }
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

        return closestVertexIndex;
    }

    private void Update()
    {
        if (_isAttachedToMesh && _meshFilter != null)
        {
            // 메시 정점 위치 갱신
            Vector3 worldPosition = _meshFilter.transform.TransformPoint(_animatedMesh.GetMeshVertex(_initialVertexIndex));
            transform.position = worldPosition;

            // 정점의 노멀 값을 사용하여 회전 값 계산
            Vector3 normal = _meshFilter.transform.TransformDirection(_animatedMesh.GetMeshNomal(_initialVertexIndex));

            Quaternion rotation = Quaternion.LookRotation(normal);
            transform.rotation = rotation;
        }
    }

    private void PlayParticle(int amount)
    {
        _bloodParticle.Emit(amount);
        _bloodCloudParticle.Emit(amount);
    }
}
