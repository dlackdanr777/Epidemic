using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class AnimatedMesh : MonoBehaviour
{
    [SerializeField] private AnimatedMeshScriptableObject _animationSO;
    [SerializeField] private MeshCollider _meshCollider;
    private MeshFilter _filter;
     
    [Space]
    [SerializeField] private int _tick = 1;
    [SerializeField] private int _animationIndex;
    public int AnimationIndex => _animationIndex;
    [SerializeField] private string _animationName;

    private List<AnimatedMeshScriptableObject.MeshData> _meshDataList;
    private bool _isLoop;

    private float _lastTickTime;

    public event Action<string> OnAnimationEnd;


    private void Awake()
    {
        _filter = GetComponent<MeshFilter>();
    }


    public void Play(string animationName)
    {
        enabled = true;
        if (_animationName != animationName)
        {
            _animationName = animationName;
            _tick = 1;
            _animationIndex = 0;
            AnimatedMeshScriptableObject.Animation animation = _animationSO.Animations.Find((item) => item.Name.Equals(animationName));
            _isLoop = animation.LoopTime;

            _meshDataList = animation.MeshDataList;
            if (string.IsNullOrEmpty(animation.Name))
                Debug.LogError("애니메이션 이름이 비어있습니다.");
        }
    }

    public List<Vector3> GetMeshVertexList()
    {
        if (_meshDataList == null)
            throw new Exception("버텍스 정보가 없습니다.");

        return _meshDataList[_animationIndex].VertexList;
    }

    public List<Vector3> GetMeshNomalList()
    {
        if (_meshDataList == null)
            throw new Exception("버텍스 정보가 없습니다.");

        return _meshDataList[_animationIndex].NomalList;
    }

    public Vector3 GetMeshVertex(int index)
    {
        if (_meshDataList == null)
            throw new Exception("버텍스 정보가 없습니다.");

        if (_meshDataList[_animationIndex].VertexList.Count <= index || index < 0)
            throw new Exception("인덱스 범위가 작거나 큽니다.");

        return _meshDataList[_animationIndex].VertexList[index];
    }

    public Vector3 GetMeshNomal(int index)
    {
        if (_meshDataList == null)
            throw new Exception("버텍스 정보가 없습니다.");

        if (_meshDataList[_animationIndex].VertexList.Count <= index || index < 0)
            throw new Exception("인덱스 범위가 작거나 큽니다.");

        return _meshDataList[_animationIndex].NomalList[index];
    }


    private void LateUpdate()
    {
        if (_meshDataList == null)
            return;

        if(Time.time >= _lastTickTime + (1f / _animationSO.AnimationFPS))
        {
            if (_meshCollider != null)
                _meshCollider.sharedMesh = _meshDataList[_animationIndex].Mesh;
            _filter.mesh = _meshDataList[_animationIndex].Mesh;

            _animationIndex++;
            if(_animationIndex >= _meshDataList.Count)
            {
                OnAnimationEnd?.Invoke(_animationName);

                if(_isLoop)
                {
                    _animationIndex = 0;
                    return;
                }

                else
                {
                    _animationIndex = _meshDataList.Count - 1;
                    enabled = false;
                }

            }
            _lastTickTime = Time.time;
        }

        _tick++;
    }
}
