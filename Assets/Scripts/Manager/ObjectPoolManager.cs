using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


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

    private GameObject _zombiePrefab;
    private GameObject _zombieParent;
    private Queue<GameObject> _zombiePool = new Queue<GameObject>();

    private Player _player;

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);

        BulletHoleObjectPooling();
        ZombieObjectPooling();
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
        return bulletHole;
    }



    public void SpawnZombie(Vector3 pos, Quaternion rot)
    {
        if(_player == null)
            _player = FindAnyObjectByType<Player>();

        GameObject zombie;
        if(_zombiePool.Count == 0)
        {
            zombie = Instantiate(_zombiePrefab, _zombieParent.transform);
            _zombiePool.Enqueue(zombie);
            zombie.gameObject.SetActive(false);
        }

        zombie = _zombiePool.Dequeue();
        zombie.gameObject.SetActive(true);

        Enemy enemy = zombie.GetComponent<Enemy>();
        enemy.Navmesh.NaveMeshEnabled(false);
        zombie.transform.position = pos;
        zombie.transform.rotation = rot;
        enemy.Navmesh.NaveMeshEnabled(true);
        enemy.Target = _player.transform;
    }


    public IEnumerator DisableZombie(Enemy enemy) 
    {
        yield return YieldCache.WaitForSeconds(40);
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
