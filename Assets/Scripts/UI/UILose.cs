using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILose : MonoBehaviour
{
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _exitButton.onClick.AddListener(onButtonClicked);
    }

    private void onButtonClicked()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("LobbyScene");
    }
}
