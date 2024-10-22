using UnityEngine;
using UnityEngine.UI;

public class UILobbyScene : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _startButton.onClick.AddListener(OnNewGameButtonClicked);
        _loadButton.onClick.AddListener(OnLoadGameButtonClicked);
        _exitButton.onClick.AddListener(() => Application.Quit());
        _loadButton.interactable = UserInfo.IsSaveFileExists();
    }

    private void OnNewGameButtonClicked()
    {
        UserInfo.DeleteSaveFile();
        UserInfo.ClearData();
        LoadingSceneManager.LoadScene("InGameScene");
    }

    private void OnLoadGameButtonClicked()
    {
        UserInfo.LoadGame();
        LoadingSceneManager.LoadScene("InGameScene");
    }
}
