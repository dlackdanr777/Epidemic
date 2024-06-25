using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField] private UIStatus _hpBar;
    [SerializeField] private TextMeshProUGUI _bulletCountText;
    [SerializeField] private TextMeshProUGUI _gameTimerText;
    [SerializeField] private TextMeshProUGUI _zombieCountText;

    [SerializeField] private GameObject _uiWin;
    [SerializeField] private GameObject _uiLose;

    [SerializeField] private GameObject _uiStop;


    private Player _player;

    private int _maxbulletCount;
    private int _currentBulletCount;
    public void Start()
    {
        _player = GameManager.Instance.Player;
        _player.OnHpChanged += _hpBar.AmountChanged;
        _hpBar.Init(_player.MaxHp);

        _player.GunController.OnFireHandler += ShowBulletCount;
        _player.GunController.OnReloadHandler += ShowBulletCount;
        ShowBulletCount();

        _uiWin.gameObject.SetActive(false);
        _uiLose.gameObject.SetActive(false);
        _uiStop.gameObject.SetActive(false);

        GameManager.Instance.Player.OnHpMin += ShowUILose;
    }

    public void SetGameTimerText(string text)
    {
        _gameTimerText.text = text;
    }

    public void SetZombieCountText(string text)
    {
        _zombieCountText.text = text;
    }

    public void ShowUIWin()
    {
        _uiWin.gameObject.SetActive(true);
    }

    public void ShowUILose()
    {
        _uiLose.gameObject.SetActive(true);
    }

    public void ShowUIStop()
    {
        _uiStop.gameObject.SetActive(true);
        GameManager.Instance.CursorVisible();
    }

    public void HiddenUIStop()
    {
        _uiStop.gameObject.SetActive(false);
        GameManager.Instance.CursorHidden();
    }

    private void ShowBulletCount()
    {
        _maxbulletCount = _player.GunController.MaxBulletCount;
        _currentBulletCount = _player.GunController.CurrentBulletCount;

        _bulletCountText.text = _currentBulletCount + " / " + _maxbulletCount;
    }




}
