using UnityEngine;
using UnityEngine.UI;

public class UILobbyScene : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _startButton.onClick.AddListener(() => LoadingSceneManager.LoadScene("InGameScene"));
        _exitButton.onClick.AddListener(() => Application.Quit());
    }
}
