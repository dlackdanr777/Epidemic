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

    private GameObject _dropItemParent;
    private Dictionary<string, DropItem> _dropItemPrefabDic = new Dictionary<string, DropItem>();
    private Dictionary<string, Queue<DropItem>> _dropItemPool = new Dictionary<string, Queue<DropItem>>();
    private Dictionary<string, HashSet<DropItem>> _useDropItemDic = new Dictionary<string, HashSet<DropItem>>();


    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);

        BulletHoleObjectPooling();
        ZombieObjectPooling();
        DropItemPooling();

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


        foreach(HashSet<DropItem> dropItemSet in _useDropItemDic.Values)
        {
            if (dropItemSet.Count <= 0)
                continue;

            foreach(DropItem dropItem in dropItemSet)
            {
                if (dropItem.gameObject == null)
                    continue;

                _dropItemPool[dropItem.ItemId].Enqueue(dropItem);
                dropItem.gameObject.SetActive(false);
            }

            dropItemSet.Clear();
        }

        _useBulletHoleSet.Clear();
        _useZombieSet.Clear();
    }


    private void DropItemPooling()
    {
        _dropItemParent = new GameObject("DropItemParent");
        _dropItemParent.transform.parent = transform;
        DropItem[] items = Resources.LoadAll<DropItem>("ObjectPool/DropItem/");

        for(int i = 0, cnt = items.Length; i < cnt; ++i)
        {
            DropItem itemPrefab = items[i];

            if (_dropItemPrefabDic.ContainsKey(itemPrefab.ItemId))
                continue;

            _dropItemPrefabDic.Add(itemPrefab.ItemId, itemPrefab);
            _dropItemPool.Add(itemPrefab.ItemId, new Queue<DropItem>());
            _useDropItemDic.Add(itemPrefab.ItemId, new HashSet<DropItem>());
            for(int j = 0; j < 10; ++j)
            {
                DropItem item = Instantiate(itemPrefab, _dropItemParent.transform);
                _dropItemPool[itemPrefab.ItemId].Enqueue(item);
                item.gameObject.SetActive(false);
            }
        }
    }


    public DropItem SpawnDropItem(string id, Vector3 pos, Quaternion rot)
    {
        if (!_dropItemPrefabDic.TryGetValue(id, out DropItem itemPrefab))
            throw new Exception("해당 아이템 Id를 가진 Drop Item 오브젝트가 존재하지 않습니다: " + id);

        DropItem item;
        if (_dropItemPool[id].Count <= 0)
        {
            item = Instantiate(itemPrefab, _dropItemParent.transform);
            _dropItemPool[itemPrefab.ItemId].Enqueue(item);
            item.gameObject.SetActive(false);
        }

        item = _dropItemPool[id].Dequeue();
        item.transform.position = pos;
        item.transform.rotation = rot;
        _useDropItemDic[id].Add(item);
        item.gameObject.SetActive(true);
        DebugLog.Log(item.ItemId);
        return item;
    }

    public void DespawnDropItem(DropItem dropItem)
    {
        if (!_useDropItemDic[dropItem.ItemId].Contains(dropItem))
            throw new Exception("해당 아이템은 사용중인 셋에 들어있지 않아 오류가 발생합니다." + dropItem.ItemId);

        _useDropItemDic[dropItem.ItemId].Remove(dropItem);
        _dropItemPool[dropItem.ItemId].Enqueue(dropItem);
        dropItem.gameObject.SetActive(false);
    }

    

    private void BulletHoleObjectPooling()
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
