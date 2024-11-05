using Muks.PcUI;
using Muks.Tween;
using UnityEngine;
using UnityEngine.UI;

public class UIResult : PcUIView
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private InGame _inGame;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _saveButton;

    public override void Init()
    {
        _exitButton.onClick.AddListener(onButtonClicked);
        _saveButton?.onClick.AddListener(_inGame.OnSaveGame);
        gameObject.SetActive(false);
    }

    public override void Hide()
    {
        VisibleState = VisibleState.Disappeared;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }


    public override void Show()
    {
        VisibleState = VisibleState.Appearing;
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;

        TweenData data = _canvasGroup.TweenAlpha(1, 0.1f);
        data.OnComplete(() =>
        {
            VisibleState = VisibleState.Appeared;
            Time.timeScale = 0;
            _canvasGroup.interactable = true;
        });
    }


    private void onButtonClicked()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("LobbyScene");
    }
}
