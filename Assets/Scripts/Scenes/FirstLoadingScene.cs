using UnityEngine;

public class FirstLoadingScene : MonoBehaviour
{
    public void Start()
    {
        Application.targetFrameRate = 144;
        LoadingSceneManager.LoadScene("LobbyScene");
    }
}
