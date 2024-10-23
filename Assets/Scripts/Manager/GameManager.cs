
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static GameManager _instance;

    
    [HideInInspector] public Player Player;
    private bool _cursorVisibled = false;
    public bool CursorVisibled => _cursorVisibled;



    public bool IsGameEnd
    {
        get { return _isGameEnd; }
        set { _isGameEnd = value; }
    }

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
        _cursorVisibled = true;
    }

    public void CursorHidden()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cursorVisibled = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            UserInfo.SaveGame(Player);
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            UserInfo.LoadGame();
        }
    }
}
