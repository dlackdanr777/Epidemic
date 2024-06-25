using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FootstepSound : MonoBehaviour
{
    [Serializable]
    public struct Footstep
    {
        public string FloorTagName;
        public AudioClip[] WalkClip;
        public AudioClip[] RunClip;
    }

    public enum StepTyep
    {
        walk,
        run,
    }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Footstep[] _footstepArray;
    [SerializeField] private Vector3 _sensingBoxSize;

    private int _layerMask;
    private void Start()
    {
        int obstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
        int groundLayer = 1 << LayerMask.NameToLayer("Ground");

        _layerMask = obstacleLayer | groundLayer;
    }


    /// <summary>
    /// 발소리가 필요한 상황에서 이 함수를 불러내면 설정에 맞춰 사운드를 재생하는 함수
    /// </summary>
    public void PlayFootstepSound(StepTyep stepTyep)    
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, _sensingBoxSize, Quaternion.identity, _layerMask);
        foreach(Collider collider in colliders)
        {
            foreach (Footstep footstep in _footstepArray)
            {
                if (!collider.CompareTag(footstep.FloorTagName))
                    continue;

                AudioClip clip;

                if (stepTyep == StepTyep.walk)
                {
                    int randPlayIndex = Random.Range(0, footstep.WalkClip.Length);
                    clip = footstep.WalkClip[randPlayIndex];
                }
                else
                {
                    int randPlayIndex = Random.Range(0, footstep.RunClip.Length);
                    clip = footstep.RunClip[randPlayIndex];
                }

                _audioSource.clip = clip;
                _audioSource.Play();
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, _sensingBoxSize);
    }
}
