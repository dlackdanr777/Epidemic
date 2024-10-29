using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField] private UIStatus _hpBar;
    [SerializeField] private TextMeshProUGUI _bulletCountText;
    [SerializeField] private TextMeshProUGUI _gameTimerText;
    [SerializeField] private TextMeshProUGUI _zombieCountText;

    private Player _player;

    public void Start()
    {
        _player = GameManager.Instance.Player;
        _player.OnHpChanged += _hpBar.AmountChanged;
        _hpBar.Init(_player.MaxHp);

        UserInfo.OnChangeBulletHandler += ShowBulletCount;
        UserInfo.OnChangeLoadBulletHandler += ShowBulletCount;
        ShowBulletCount();

    }

    public void SetGameTimerText(string text)
    {
        _gameTimerText.text = text;
    }

    public void SetZombieCountText(string text)
    {
        _zombieCountText.text = text;
    }

    private void ShowBulletCount()
    {
        _bulletCountText.text = UserInfo.LoadBulletCount + " / " + UserInfo.BulletCount;
    }




}
