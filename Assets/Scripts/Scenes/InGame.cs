using Muks.DataBind;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameState
{
    None,
    Set,
    Wait,
    Start,
    End,
}

public enum RoundState { Wait, Proceeding, End }

public class InGame : MonoBehaviour
{
    [Serializable]
    public struct Rounds
    {
        [SerializeField] private SpawnZombie[] _spawnZombies;
        public SpawnZombie[] SpawnZombies => _spawnZombies;

        [SerializeField] private float _roundTime;
        public float RoundTime => _roundTime;
    }

    [Serializable]
    public struct SpawnZombie
    {
        public EnemyType Type;
        public int SpawnCount;
    }

    [SerializeField] private Player _player;
    [SerializeField] private GameObject _ingameTrigger;
    [SerializeField] private UIGame _uiGame;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private Rounds[] _rounds;
    [SerializeField] private AudioClip _bgMusic;
    [SerializeField] private List<Enemy> _defalutEnemyList;
    [SerializeField] private GameObject _dropItemParent;
    [SerializeField] private List<Door> _doorList;


    private GameState _gameState;
    private RoundState _roundState;
    private int _currentRound;
    private float _currentTime;
    private float _saveTime => 300;
    private float _saveTimer;


    private void Start()
    {
        _gameState = GameState.None;
        _roundState = RoundState.Wait;
        _currentRound = 1;
        _saveTimer = _saveTime;
        _uiGame.SetZombieCountText("학교를 수색하십시오");

        GameManager.Instance.IsGameEnd = false;
        SoundManager.Instance.PlayAudio(AudioType.Background, _bgMusic);
        GameManager.Instance.CursorHidden();
        GameManager.Instance.Player.OnHpMin += EndGame;
        LoadGame();
    }

    private void FixedUpdate()
    {
        if (_gameState == GameState.Set)
        {
            SetRoundState(_roundState);
            _gameState = GameState.Start;
        }

        else if (_gameState == GameState.Start)
        {
            switch (_roundState)
            {
                case RoundState.Wait:
                    RoundTimer();
                    break;

                case RoundState.Proceeding:
                    RoundTimer();
                    RoundProceeding();
                    break;

                case RoundState.End:
                    break;
            }
        }
    }


    private void LoadGame()
    {
        if (!UserInfo.IsSaveFileExists())
            return;

        SaveData saveData = UserInfo.SaveData;
        if (UserInfo.SaveData == null)
            return;
        _ingameTrigger.SetActive(false);
        _gameState = (GameState)saveData.GameStateSaveData.GameState;
        _roundState = (RoundState)saveData.GameStateSaveData.RoundState;
        _currentRound = saveData.GameStateSaveData.CurrentRound;
        SetRoundState(_roundState);
        _currentTime = saveData.GameStateSaveData.CurrentTime;

        for (int i = 0, cnt = _defalutEnemyList.Count; i < cnt; ++i)
        {
            Destroy(_defalutEnemyList[i].gameObject);
        }
        _defalutEnemyList.Clear();

        List<SaveEnemyData> enemyDataList = saveData.EnemyDataList;
        for (int i = 0, cnt = enemyDataList.Count; i < cnt; ++i)
        {
            Vector3 pos = enemyDataList[i].EnemyPosition;
            Quaternion rot = enemyDataList[i].EnemyRotation;
            Enemy enemy = ObjectPoolManager.Instance.SpawnZombie(pos, rot);
            float depleteHpValue = enemy.MaxHp - enemyDataList[i].Hp;
            enemy.DepleteHp(null, depleteHpValue);
        }

        DropItem[] items = _dropItemParent.GetComponentsInChildren<DropItem>();
        for (int i = 0, cnt = items.Length; i < cnt; ++i)
        {
            Destroy(items[i].gameObject);
        }

        List<SaveDropItemData> dropItemDataList = saveData.DropItemDataList;
        for (int i = 0, cnt = dropItemDataList.Count; i < cnt; ++i)
        {
            Vector3 pos = dropItemDataList[i].Position;
            Quaternion rot = dropItemDataList[i].Rotation;
            DropItem dropItem = ObjectPoolManager.Instance.SpawnDropItem(dropItemDataList[i].ItemId, pos, rot);
            dropItem.SetCount(dropItemDataList[i].Count);
        }

        List<SaveDoorData> doorDataList = saveData.DoorDataList;
        for (int i = 0, cnt = doorDataList.Count; i < cnt; ++i)
        {
            if (_doorList[i] == null)
            {
                DebugLog.LogError("Door Index가 부족합니다: " + i);
                continue;
            }

            _doorList[i].SetDoorState(doorDataList[i].IsOpened);

            if (_doorList[i].Barricade == null)
                continue;

            _doorList[i].Barricade.SetHp(doorDataList[i].DoorHp);
        }

        List<SaveBuildObjectData> buildObjectDataList = saveData.BuildObjectDataList;
        for (int i = 0, cnt = buildObjectDataList.Count; i < cnt; ++i)
        {
            Vector3 pos = buildObjectDataList[i].Position;
            Quaternion rot = buildObjectDataList[i].Rotation;
            BuildObject buildObject = ObjectPoolManager.Instance.SpawnBuildObject(buildObjectDataList[i].Id, pos, rot);
            buildObject.SetHp(buildObjectDataList[i].Hp);
        }
    }

    public void OnSaveGame()
    {
        List<Enemy> enemyList = FindObjectsOfType<Enemy>().ToList();
        for (int i = 0, cnt = enemyList.Count; i < cnt; ++i)
        {
            if (!enemyList[i].gameObject.activeSelf || enemyList[i].Hp <= enemyList[i].MinHp)
            {
                enemyList.RemoveAt(i--);
                --cnt;
            }
        }

        DropItem[] items = _dropItemParent.GetComponentsInChildren<DropItem>();
        List<DropItem> dropItemList = new List<DropItem>();

        for (int i = 0, cnt = items.Length; i < cnt; ++i)
        {
            if (items[i] == null)
                continue;

            if (!items[i].gameObject.activeSelf)
                continue;

            dropItemList.Add(items[i]);
        }

        List<BuildObject> buildObjectList = FindObjectsOfType<BuildObject>().ToList();
        for (int i = 0, cnt = buildObjectList.Count; i < cnt; ++i)
        {
            if (!buildObjectList[i].gameObject.activeSelf || buildObjectList[i].Hp <= buildObjectList[i].MinHp)
            {
                buildObjectList.RemoveAt(i--);
                --cnt;
            }
        }
        GameStateSaveData gameStateSaveData = new GameStateSaveData((int)_gameState, (int)_roundState, _currentRound, _currentTime);
        UserInfo.SaveGame(gameStateSaveData, _player, enemyList, dropItemList, _doorList, buildObjectList);

        UIManager.Instance.ShowCenterText("현재 상황이 저장되었습니다.");
    }

    public void StartGame(GameObject triggerObj)
    {
        triggerObj.gameObject.SetActive(false);
        _gameState = GameState.Set;
    }
  

    private void RoundProceeding()
    {
        int _enemyCount = ObjectPoolManager.Instance.GetSpawnZombieCount();
        _uiGame.SetZombieCountText("남은 좀비 수: " + _enemyCount);

        if (_currentTime <= 0 || _enemyCount <= 0)
            SetRoundState(RoundState.End);
    }

    private void RoundTimer()
    {     
        _currentTime = Mathf.Max(0, _currentTime - Time.deltaTime);
        _uiGame.SetGameTimerText(Enum.GetName(typeof(RoundState), _roundState) +": " + Mathf.Floor(_currentTime * 100f) / 100f);
        if (_currentTime <= 0)
            SetRoundState(++_roundState);
    }

    private void SetRoundState(RoundState roundState)
    {
        _roundState = roundState;
        switch (_roundState)
        {
            case RoundState.Wait:
                SetRoundWait(_currentRound);
                break;

            case RoundState.Proceeding:
                SetRoundProceeding(_currentRound);
                break;

            case RoundState.End:
                StartCoroutine(SetRoundEndRoutine());
                break;
        }

    }

    private void SetRoundWait(int round)
    {
        _uiGame.SetZombieCountText("곧 좀비가 몰려옵니다...");
        _currentTime = 30;
    }


    private void SetRoundProceeding(int round)
    {
        _currentTime = _rounds[round - 1].RoundTime;
        for (int i = 0; i < _rounds[round - 1].SpawnZombies.Length; i++)
        {
            for (int j = 0; j < _rounds[round - 1].SpawnZombies[i].SpawnCount; j++)
            {
                bool isSpawnEnabled = true;
                float randX = 0;
                float randZ = 0;
                float count = 0;
                do
                {
                    randX = _spawnPos.position.x + UnityEngine.Random.Range(-10f, 10f);
                    randZ = _spawnPos.position.z + UnityEngine.Random.Range(-10f, 10f);

                    if (0 < Physics.OverlapBox(new Vector3(randX, _spawnPos.position.y, randZ), Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("Enemy", "Obstacle")).Length)
                    {
                        isSpawnEnabled = false;
                    }
                    count++;
                } while (!isSpawnEnabled && count <= 20);

                if(21 <= count)
                {
                    randX = _spawnPos.position.x;
                    randZ = _spawnPos.position.z;
                }
                ObjectPoolManager.Instance.SpawnZombie(new Vector3(randX, _spawnPos.position.y, randZ), Quaternion.Euler(0, 180, 0), GameManager.Instance.Player.gameObject);
            }

        }
    }

    private IEnumerator SetRoundEndRoutine()
    {
        int _enemyCount = ObjectPoolManager.Instance.GetSpawnZombieCount();
        if (0 < _enemyCount)
        {
            EndGame();
            yield break;
        }

        yield return YieldCache.WaitForSeconds(3);
        if (_currentRound < _rounds.Length)
        {
            SetRoundState(RoundState.Wait);
            _gameState = GameState.Start;
            _currentRound++;
        }
        else
        {
            _gameState = GameState.End;
            GameManager.Instance.GameEnd();
            DataBind.GetUnityActionValue("ShowWin")?.Invoke();
        }
    }

    private void EndGame()
    {
        StartCoroutine(EndGameRoutine());
        GameManager.Instance.Player.OnHpMin -= EndGame;
    }


    private IEnumerator EndGameRoutine()
    {
        _gameState = GameState.End;
        yield return YieldCache.WaitForSeconds(3);
        DataBind.GetUnityActionValue("ShowLose")?.Invoke();
    }
}
