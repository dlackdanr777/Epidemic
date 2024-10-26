using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("ObjectPoolManager");
                _instance = obj.AddComponent<ObjectPoolManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static ObjectPoolManager _instance;

    private GameObject _bulletHoleParent;
    private Queue<GameObject> _bulletHolePool = new Queue<GameObject>();
    private HashSet<GameObject> _useBulletHoleSet = new HashSet<GameObject>();

    private GameObject _zombiePrefab;
    private GameObject _zombieParent;
    private Queue<GameObject> _zombiePool = new Queue<GameObject>();
    private HashSet<GameObject> _useZombieSet = new HashSet<GameObject>();

    private Player _player;

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);

        BulletHoleObjectPooling();
        ZombieObjectPooling();

        LoadingSceneManager.OnChangeSceneHandler += OnChangeSceneEvent;
    }

    private void OnChangeSceneEvent()
    {
        foreach(GameObject obj in _useBulletHoleSet)
            obj.SetActive(false);

        foreach(GameObject obj in _useZombieSet)
        {
            _zombiePool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }

        _useBulletHoleSet.Clear();
        _useZombieSet.Clear();
    }


    public void BulletHoleObjectPooling()
    {
        _bulletHoleParent = new GameObject("BulletHoleParent");
        _bulletHoleParent.transform.parent = transform;

        GameObject bulletHolePrefab = Resources.Load<GameObject>("ObjectPool/BulletHole");

        for(int i = 0; i < 100; i++)
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, Vector3.zero, Quaternion.identity);
            bulletHole.transform.parent = _bulletHoleParent.transform;
            _bulletHolePool.Enqueue(bulletHole);
            bulletHole.SetActive(false);
        }
    }


    private void ZombieObjectPooling()
    {
        _zombieParent = new GameObject("ZombleParent");
        _zombieParent.transform.parent = transform;

        _zombiePrefab = Resources.Load<GameObject>("ObjectPool/BasicZombie");

        for (int i = 0; i < 100; i++)
        {
            GameObject zombie = Instantiate(_zombiePrefab, Vector3.zero, Quaternion.identity);
            zombie.transform.parent = _zombieParent.transform;
            _zombiePool.Enqueue(zombie);
            zombie.SetActive(false);
        }
    }


    public GameObject SpawnBulletHole( Vector3 pos, Quaternion rot)
    {
        GameObject bulletHole = _bulletHolePool.Dequeue();

        bulletHole.SetActive(false);
        bulletHole.SetActive(true);
        bulletHole.transform.position = pos;
        bulletHole.transform.rotation = rot;
        _bulletHolePool.Enqueue(bulletHole);
        _useBulletHoleSet.Add(bulletHole);
        return bulletHole;
    }

    public void DespawnBulletHole(GameObject bulletHole)
    {
        if(!_useBulletHoleSet.Contains(bulletHole))
            throw new Exception("해당 몬스터는 사용중인 셋에 들어있지 않아 오류가 발생합니다." + bulletHole.name);

        _useBulletHoleSet.Remove(bulletHole);
        bulletHole.SetActive(false);
    }



    public Enemy SpawnZombie(Vector3 pos, Quaternion rot)
    {
        GameObject zombie;
        if(_zombiePool.Count == 0)
        {
            zombie = Instantiate(_zombiePrefab, _zombieParent.transform);
            _zombiePool.Enqueue(zombie);
            zombie.gameObject.SetActive(false);
        }

        zombie = _zombiePool.Dequeue();
        _useZombieSet.Add(zombie);
        zombie.gameObject.SetActive(true);

        Enemy enemy = zombie.GetComponent<Enemy>();
        enemy.Navmesh.NaveMeshEnabled(false);
        zombie.transform.position = pos;
        zombie.transform.rotation = rot;
        enemy.Navmesh.NaveMeshEnabled(true);
        return enemy;
    }


    public Enemy SpawnZombie(Vector3 pos, Quaternion rot, GameObject target)
    {
        GameObject zombie;
        if (_zombiePool.Count == 0)
        {
            zombie = Instantiate(_zombiePrefab, _zombieParent.transform);
            _zombiePool.Enqueue(zombie);
            zombie.gameObject.SetActive(false);
        }

        zombie = _zombiePool.Dequeue();
        _useZombieSet.Add(zombie);
        zombie.gameObject.SetActive(true);

        Enemy enemy = zombie.GetComponent<Enemy>();
        enemy.Navmesh.NaveMeshEnabled(false);
        zombie.transform.position = pos;
        zombie.transform.rotation = rot;
        enemy.Navmesh.NaveMeshEnabled(true);
        enemy.Target = target.transform;
        return enemy;
    }


    public IEnumerator DespawnZombie(Enemy enemy) 
    {
        yield return YieldCache.WaitForSeconds(10);

        if (!_useZombieSet.Contains(enemy.gameObject))
            throw new Exception("해당 몬스터는 사용중인 셋에 들어있지 않아 오류가 발생합니다." + enemy.name);

        _useZombieSet.Remove(enemy.gameObject);
        _zombiePool.Enqueue(enemy.gameObject);
        enemy.gameObject.SetActive(false);
    }

    public int ZombieCounting()
    {
        int zombieCount = 0;

        Enemy[] objs = _zombieParent.GetComponentsInChildren<Enemy>();
        zombieCount += objs.Length;

        foreach (var obj in objs)
        {
            if (obj.IsDead)
                zombieCount--;
        }

        return zombieCount;
    }

}
