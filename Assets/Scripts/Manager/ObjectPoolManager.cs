using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    private GameObject _bulletHolePrefab;
    private Queue<GameObject> _bulletHolePool = new Queue<GameObject>();
    private HashSet<GameObject> _useBulletHoleSet = new HashSet<GameObject>();


    private GameObject _bulletParent;
    private Bullet _bulletPrefab;
    private Queue<Bullet> _bulletPool = new Queue<Bullet>();
    private HashSet<Bullet> _useBulletSet = new HashSet<Bullet>();

    private GameObject _zombieParent;
    private Enemy _zombiePrefab;
    private Queue<Enemy> _zombiePool = new Queue<Enemy>();
    private HashSet<Enemy> _useZombieSet = new HashSet<Enemy>();

    private GameObject _dropItemParent;
    private Dictionary<string, DropItem> _dropItemPrefabDic = new Dictionary<string, DropItem>();
    private Dictionary<string, Queue<DropItem>> _dropItemPool = new Dictionary<string, Queue<DropItem>>();
    private Dictionary<string, HashSet<DropItem>> _useDropItemDic = new Dictionary<string, HashSet<DropItem>>();


    private GameObject _buildObjectParent;
    private Dictionary<string, BuildObject> _buildObjectPrefabDic = new Dictionary<string, BuildObject>();
    private Dictionary<string, Queue<BuildObject>> _buildObjectPool = new Dictionary<string, Queue<BuildObject>>();
    private Dictionary<string, HashSet<BuildObject>> _useBuildObjectDic = new Dictionary<string, HashSet<BuildObject>>();

    private GameObject _buildPreviewParent;
    private Dictionary<string, PreviewObject> _buildPreviewPrefabDic = new Dictionary<string, PreviewObject>();
    private Dictionary<string, Queue<PreviewObject>> _buildPreviewPool = new Dictionary<string, Queue<PreviewObject>>();
    private Dictionary<string, HashSet<PreviewObject>> _useBuildPreviewDic = new Dictionary<string, HashSet<PreviewObject>>();

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);

        BulletHoleObjectPooling();
        BulletPooling();
        ZombieObjectPooling();
        DropItemPooling();
        BuildObjectPooling();
        BuildPreviewObjectPooling();
        LoadingSceneManager.OnChangeSceneHandler += OnChangeSceneEvent;
    }

    private void OnChangeSceneEvent()
    {
        foreach(GameObject obj in _useBulletHoleSet)
        {
            if (obj == null)
                continue;

            obj.transform.parent = _bulletHoleParent.transform;
            obj.SetActive(false);
        }
        _useBulletHoleSet.Clear();

        foreach (Bullet bullet in _useBulletSet)
        {
            if (bullet == null)
                continue;

            bullet.gameObject.SetActive(false);
        }
        _useBulletSet.Clear();

        foreach(Enemy obj in _useZombieSet)
        {
            _zombiePool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
        _useZombieSet.Clear();


        foreach (HashSet<DropItem> dropItemSet in _useDropItemDic.Values)
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

        foreach (HashSet<BuildObject> hashSet in _useBuildObjectDic.Values)
        {
            if (hashSet.Count <= 0)
                continue;

            foreach (BuildObject buildObject in hashSet)
            {
                if (buildObject.gameObject == null)
                    continue;

                _buildObjectPool[buildObject.BuildData.Id].Enqueue(buildObject);
                buildObject.gameObject.SetActive(false);
            }
            hashSet.Clear();
        }

        foreach (HashSet<PreviewObject> hashSet in _useBuildPreviewDic.Values)
        {
            if (hashSet.Count <= 0)
                continue;

            foreach (PreviewObject buildPreview in hashSet)
            {
                if (buildPreview.gameObject == null)
                    continue;

                _buildPreviewPool[buildPreview.BuildData.Id].Enqueue(buildPreview);
                buildPreview.gameObject.SetActive(false);
            }
            hashSet.Clear();
        }
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
            throw new Exception("�ش� ������ Id�� ���� Drop Item ������Ʈ�� �������� �ʽ��ϴ�: " + id);

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
        return item;
    }

    public void DespawnDropItem(DropItem dropItem)
    {
        if (!_useDropItemDic[dropItem.ItemId].Contains(dropItem))
        {
            Destroy(dropItem.gameObject);
            DebugLog.Log("�ش� �������� ������� �¿� ������� �ʾ� �ı��մϴ�." + dropItem.ItemId);
            return;
        }

        _useDropItemDic[dropItem.ItemId].Remove(dropItem);
        _dropItemPool[dropItem.ItemId].Enqueue(dropItem);
        dropItem.gameObject.SetActive(false);
    }


    private void BuildObjectPooling()
    {
        _buildObjectParent = new GameObject("BuildParent");
        _buildObjectParent.transform.parent = transform;
        BuildData[] items = Resources.LoadAll<BuildData>("BuildDatas/");

        for (int i = 0, cnt = items.Length; i < cnt; ++i)
        {
            BuildData buildData = items[i];
            if (_buildObjectPrefabDic.ContainsKey(buildData.Id))
                continue;

            _buildObjectPrefabDic.Add(buildData.Id, buildData.BuildObjectPrefab);
            _buildObjectPool.Add(buildData.Id, new Queue<BuildObject>());
            _useBuildObjectDic.Add(buildData.Id, new HashSet<BuildObject>());
            for (int j = 0; j < 10; ++j)
            {
                BuildObject build = Instantiate(buildData.BuildObjectPrefab, _buildObjectParent.transform);
                build.SetData(buildData);
                _buildObjectPool[buildData.Id].Enqueue(build);
                build.gameObject.SetActive(false);
            }
        }
    }


    public BuildObject SpawnBuildObject(string id, Vector3 pos, Quaternion rot)
    {
        if (!_buildObjectPrefabDic.TryGetValue(id, out BuildObject buildPrefab))
            throw new Exception("�ش� Id�� ���� Build ������Ʈ�� �������� �ʽ��ϴ�: " + id);

        BuildObject buildObject;
        if (_buildObjectPool[id].Count <= 0)
        {
            buildObject = Instantiate(buildPrefab, _buildObjectParent.transform);
            _buildObjectPool[id].Enqueue(buildObject);
            buildObject.gameObject.SetActive(false);
        }

        buildObject = _buildObjectPool[id].Dequeue();
        buildObject.transform.position = pos;
        buildObject.transform.rotation = rot;
        buildObject.SetHp(buildObject.MaxHp);
        _useBuildObjectDic[id].Add(buildObject);
        buildObject.gameObject.SetActive(true);
        return buildObject;
    }


    public void DespawnBuildObject(BuildObject buildObject)
    {
        string id = buildObject.BuildData.Id;
        if (!_useBuildObjectDic[id].Contains(buildObject))
            throw new Exception("�ش� ���๰�� ������� �¿� ������� �ʾ� ������ �߻��մϴ�." + id);

        _useBuildObjectDic[id].Remove(buildObject);
        _buildObjectPool[id].Enqueue(buildObject);
        buildObject.gameObject.SetActive(false);
    }



    private void BuildPreviewObjectPooling()
    {
        _buildPreviewParent = new GameObject("BuildPreviewParent");
        _buildPreviewParent.transform.parent = transform;
        BuildData[] items = Resources.LoadAll<BuildData>("BuildDatas/");

        for (int i = 0, cnt = items.Length; i < cnt; ++i)
        {
            BuildData buildData = items[i];
            if (_buildPreviewPrefabDic.ContainsKey(buildData.Id))
                continue;

            _buildPreviewPrefabDic.Add(buildData.Id, buildData.PreviewPrefab);
            _buildPreviewPool.Add(buildData.Id, new Queue<PreviewObject>());
            _useBuildPreviewDic.Add(buildData.Id, new HashSet<PreviewObject>());
            for (int j = 0; j < 1; ++j)
            {
                PreviewObject build = Instantiate(buildData.PreviewPrefab, _buildPreviewParent.transform);
                build.SetBuildData(buildData);
                _buildPreviewPool[buildData.Id].Enqueue(build);
                build.gameObject.SetActive(false);
            }
        }
    }


    public PreviewObject SpawnBuildPreviewObject(string id, Vector3 pos, Quaternion rot)
    {
        if (!_buildPreviewPrefabDic.TryGetValue(id, out PreviewObject buildPrefab))
            throw new Exception("�ش� Id�� ���� Build ������Ʈ�� �������� �ʽ��ϴ�: " + id);

        PreviewObject buildObject;
        if (_buildObjectPool[id].Count <= 0)
        {
            buildObject = Instantiate(buildPrefab, _buildPreviewParent.transform);
            _buildPreviewPool[id].Enqueue(buildObject);
            buildObject.gameObject.SetActive(false);
        }

        buildObject = _buildPreviewPool[id].Dequeue();
        buildObject.transform.position = pos;
        buildObject.transform.rotation = rot;
        _useBuildPreviewDic[id].Add(buildObject);
        buildObject.gameObject.SetActive(true);
        return buildObject;
    }

    public void DespawnBuildPreviewObject(PreviewObject previewObject)
    {
        string id = previewObject.BuildData.Id;
        if (!_useBuildPreviewDic[id].Contains(previewObject))
            throw new Exception("�ش� ���๰�� ������� �¿� ������� �ʾ� ������ �߻��մϴ�." + id);

        _useBuildPreviewDic[id].Remove(previewObject);
        _buildPreviewPool[id].Enqueue(previewObject);
        previewObject.gameObject.SetActive(false);
    }


    private void BulletHoleObjectPooling()
    {
        _bulletHoleParent = new GameObject("BulletHoleParent");
        _bulletHoleParent.transform.parent = transform;

        _bulletHolePrefab = Resources.Load<GameObject>("ObjectPool/BulletHole");

        for(int i = 0; i < 100; i++)
        {
            GameObject bulletHole = Instantiate(_bulletHolePrefab, Vector3.zero, Quaternion.identity);
            bulletHole.transform.parent = _bulletHoleParent.transform;
            _bulletHolePool.Enqueue(bulletHole);
            bulletHole.SetActive(false);
        }
    }


    private void BulletPooling()
    {
        _bulletParent = new GameObject("BulletParent");
        _bulletParent.transform.parent = transform;

        Bullet bulletPrefab = Resources.Load<Bullet>("ObjectPool/Bullet");
        for (int i = 0; i < 100; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            bullet.transform.parent = _bulletParent.transform;
            _bulletPool.Enqueue(bullet);
            bullet.gameObject.SetActive(false);
        }
    }


    private void ZombieObjectPooling()
    {
        _zombieParent = new GameObject("ZombleParent");
        _zombieParent.transform.parent = transform;

        _zombiePrefab = Resources.Load<Enemy>("ObjectPool/BasicZombie");

        for (int i = 0; i < 100; i++)
        {
            Enemy zombie = Instantiate(_zombiePrefab, Vector3.zero, Quaternion.identity);
            zombie.transform.parent = _zombieParent.transform;
            _zombiePool.Enqueue(zombie);
            zombie.gameObject.SetActive(false);
        }
    }


    public GameObject SpawnBulletHole( Vector3 pos, Quaternion rot)
    {
        GameObject bulletHole;
        if (_bulletHolePool.Count <= 0)
        {
            bulletHole = Instantiate(_bulletHolePrefab, Vector3.zero, Quaternion.identity);
            bulletHole.transform.parent = _bulletHoleParent.transform;
            _bulletHolePool.Enqueue(bulletHole);
        }

        bulletHole = _bulletHolePool.Dequeue();

        bulletHole.SetActive(false);
        bulletHole.SetActive(true);
        bulletHole.transform.position = pos;
        bulletHole.transform.rotation = rot;
        _bulletHolePool.Enqueue(bulletHole);
        _useBulletHoleSet.Add(bulletHole);
        return bulletHole;
    }

    public Bullet SpawnBullet(float speed, LayerMask layerMask, float distance, Vector3 targetPos, Vector3 pos, Quaternion rot)
    {
        Bullet bullet = _bulletPool.Dequeue();

        bullet.gameObject.SetActive(false);
        bullet.gameObject.SetActive(true);
        bullet.transform.position = pos;
        bullet.transform.rotation = rot;
        _bulletPool.Enqueue(bullet);
        _useBulletSet.Add(bullet);
        bullet.SetBullet(speed, layerMask, distance, targetPos);
        return bullet;
    }

    public void DespawnBulletHole(GameObject bulletHole)
    {
        if(!_useBulletHoleSet.Contains(bulletHole))
            throw new Exception("�ش� ź��� ������� �¿� ������� �ʾ� ������ �߻��մϴ�." + bulletHole.name);

        _useBulletHoleSet.Remove(bulletHole);
        bulletHole.SetActive(false);
    }

    public void DespawnBullet(Bullet bullet)
    {
        if (!_useBulletSet.Contains(bullet))
            throw new Exception("�ش� �Ѿ��� ������� �¿� ������� �ʾ� ������ �߻��մϴ�." + bullet.name);

        _useBulletSet.Remove(bullet);
        bullet.gameObject.SetActive(false);
    }



    public Enemy SpawnZombie(Vector3 pos, Quaternion rot)
    {
        Enemy zombie;
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
        Enemy zombie;
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

        if (!_useZombieSet.Contains(enemy))
        {
            Destroy(enemy.gameObject);
            throw new Exception("�ش� ���ʹ� ������� �¿� ������� �ʾ� ������ �߻��մϴ�." + enemy.name);
        }

        _useZombieSet.Remove(enemy);
        _zombiePool.Enqueue(enemy);
        enemy.gameObject.SetActive(false);
    }

    public int GetSpawnZombieCount()
    {
        int zombieCount = 0;

        foreach(Enemy enemy in _useZombieSet)
        {
            if (enemy.Hp <= enemy.MinHp)
                continue;

            zombieCount++;
        }

        return zombieCount;
    }

}
