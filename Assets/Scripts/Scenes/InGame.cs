using Muks.DataBind;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InGame : MonoBehaviour
{
    [Serializable]
    public struct Rounds
    {
        public SpawnZombie[] SpawnZombies;
        public float LimitTime;
        public float WaitingTime;
    }

    [Serializable]
    public struct SpawnZombie
    {
        public EnemyType Type;
        public int SpawnCount;
    }

    public enum RoundType { Wait, Start, Proceeding, End }


    [SerializeField] private UIGame _uiGame;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private Rounds[] _rounds;
    [SerializeField] private AudioClip _bgMusic;
    [SerializeField] private List<Enemy> _defalutEnemyList;
    [SerializeField] private List<DropItem> _dropItemList;


    
    private RoundType _roundType;
    private int _enemyCount;
    private int _currentRound;
    
    private float _currentTime;

    private bool _isEndTime;
    private bool _isGameStart;
    private bool _roundClear;


    public void Start()
    {
        _currentRound = 1;
        _roundType = RoundType.Wait;
        _uiGame.SetZombieCountText("학교를 수색하십시오");
        _isGameStart = false;
        GameManager.Instance.IsGameEnd = false;
        SoundManager.Instance.PlayAudio(AudioType.Background, _bgMusic);
        GameManager.Instance.CursorHidden();
        GameManager.Instance.Player.OnHpMin += EndGame;

        if(UserInfo.IsSaveFileExists())
        {
            DebugLog.Log("저장 파일 존재");
            for(int i = 0, cnt =  _defalutEnemyList.Count; i < cnt; ++i)
            {
                Destroy(_defalutEnemyList[i].gameObject);
            }
            _defalutEnemyList.Clear();

            List<SaveEnemyData> enemyDataList = UserInfo.EnemyDataList;
            for(int i = 0, cnt = enemyDataList.Count; i < cnt; ++i)
            {
                Vector3 pos = enemyDataList[i].EnemyPosition;
                Quaternion rot = enemyDataList[i].EnemyRotation;
                Enemy enemy = ObjectPoolManager.Instance.SpawnZombie(pos, rot);
                float depleteHpValue = enemy.MaxHp - enemyDataList[i].Hp;
                enemy.DepleteHp(null, depleteHpValue);
            }


            for(int i = 0, cnt = _dropItemList.Count; i < cnt; ++i)
            {
                Destroy(_dropItemList[i].gameObject);  
            }
            _dropItemList.Clear();

            List<SaveDropItemData> dropItemDataList = UserInfo.DropItemDataList;
            DebugLog.Log(dropItemDataList.Count);
            for(int i = 0, cnt = dropItemDataList.Count; i < cnt; ++i)
            {
                Vector3 pos = dropItemDataList[i].Position;
                Quaternion rot = dropItemDataList[i].Rotation;
                DropItem dropItem = ObjectPoolManager.Instance.SpawnDropItem(dropItemDataList[i].ItemId, pos, rot);
            }

        }
    }

    public void StartGame()
    {
        _isGameStart = true;
    }


    private void EndGame()
    {
        StartCoroutine(EndGameRoutine());
        GameManager.Instance.Player.OnHpMin -= EndGame;
    }

    private void Update()
    {
        if (!_isGameStart)
            return;

        if (!_roundClear)
        {
            RoundTimer();
            RoundUpdate();
        }
        else
        {
            RoundClear();
        }
    }

    private void FixedUpdate()
    {
        if(_roundType == RoundType.Proceeding)
        {
            _enemyCount = ObjectPoolManager.Instance.ZombieCounting();
            _uiGame.SetZombieCountText("남은 좀비 수: " + _enemyCount);

            if (_enemyCount <= 0)
            {
                _roundClear = true;
            }
                
        }
    }

    private void RoundTimer()
    {
        
        _currentTime -= Time.deltaTime;
        _uiGame.SetGameTimerText(Enum.GetName(typeof(RoundType), _roundType) +": " + Mathf.Floor(_currentTime * 100f) / 100f);
        if (_currentTime <= 0)
            _isEndTime = true;
    }

    private void RoundUpdate()
    {
        if (_isEndTime)
        {
            switch (_roundType)
            {
                case RoundType.Wait:
                    RoundWait(_currentRound);
                    break;
                case RoundType.Start:
                    RoundStart(_currentRound);
                    break;
                case RoundType.Proceeding:
                    RoundProceeding(_currentRound);
                    break;
            }
            _roundType++;
            _roundType = (RoundType)((int)_roundType % 3);

            _isEndTime = false;
        }
    }


    private void RoundStart(int round)
    {
        _currentTime = _rounds[round - 1].LimitTime;
        for(int i = 0; i < _rounds[round - 1].SpawnZombies.Length; i++)
        {
            for(int j = 0; j < _rounds[round - 1].SpawnZombies[i].SpawnCount; j++)
                ObjectPoolManager.Instance.SpawnZombie(_spawnPos.position, Quaternion.Euler(0,180,0), GameManager.Instance.Player.gameObject);
        }
    }

    private void RoundWait(int round)
    {
        _uiGame.SetZombieCountText("곧 좀비가 몰려옵니다...");
        _currentTime = _rounds[round - 1].WaitingTime;
    }

    private void RoundProceeding(int round)
    {
        if(_enemyCount >= 0)
        {
            GameManager.Instance.GameEnd();
            DataBind.GetUnityActionValue("ShowLose")?.Invoke();
            //게임오버
        }
    }

    private float _clearWaitTime;
    private void RoundClear()
    {
        _clearWaitTime += Time.deltaTime;
        if (_clearWaitTime > 5)
        {        
            if(_rounds.Length > _currentRound)
            {
                _clearWaitTime = 0;
                _roundType = RoundType.Wait;
                _currentRound++;
                _isEndTime = true;
                _roundClear = false;
            }
            else
            {
                GameManager.Instance.GameEnd();
                DataBind.GetUnityActionValue("ShowWin")?.Invoke();
                
            }

        }
    }


    private IEnumerator EndGameRoutine()
    {
        yield return YieldCache.WaitForSeconds(3);
        _isGameStart = false;
        DataBind.GetUnityActionValue("ShowLose")?.Invoke();
    }
}
