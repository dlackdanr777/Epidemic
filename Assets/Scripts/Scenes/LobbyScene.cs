using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private UILobbyScene _uiLobbyScene;

    [SerializeField] private AudioClip _bgClip;

    private void Start()
    {
        GameManager.Instance.CursorVisible();
        SoundManager.Instance.PlayAudio(AudioType.Background, _bgClip);
    }
}
