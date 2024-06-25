using System;
using UnityEngine;

public enum AudioType
{
    Effect,
    Background,
    Langth
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject("SoundManager");
                _instance = obj.AddComponent<SoundManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static SoundManager _instance;

    private AudioSource[] _source;
    private GameObject _sourceParent;
    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        _sourceParent = new GameObject("Source Parent");
        _sourceParent.transform.parent = transform;
        _source = new AudioSource[(int)AudioType.Langth];

        for (int i = 0, count = _source.Length; i < count; i++)
        {
            GameObject obj = new GameObject(Enum.GetName(typeof(AudioType), i));
            obj.transform.parent = _sourceParent.transform;
            _source[i] = obj.AddComponent<AudioSource>();
            _source[i].playOnAwake = false;
            _source[i].volume = 0.5f;
        }

        _source[(int)AudioType.Background].loop = true;
    }

    public void PlayAudio(AudioType type, AudioClip clip)
    {
        if(type == AudioType.Background)
        {
            _source[(int)type].clip = clip;
            _source[(int)type].Play();
        }
        else
        {
            _source[(int)type].PlayOneShot(clip);
        }
    }

    public void StopAudio(AudioType type)
    {
        _source[(int)type].Stop();
    }
}
