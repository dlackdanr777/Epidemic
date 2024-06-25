
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonHandler<GameManager>
{

    public bool IsGameEnd
    {
        get { return _isGameEnd; }
        set { _isGameEnd = value; }
    }

    public Player Player;

    private bool _isGameEnd;
    public void GameEnd()
    {
        CursorVisible();
        _isGameEnd = true;
    }

    public void CursorVisible()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CursorHidden()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
