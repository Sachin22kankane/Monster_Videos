using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;

public enum PoolType
{
    BulletGreen, BulletRed, GreenBlood, ScoreText, yellowBlood, goldCoin, missile, missileExplosion, spawnEffect
}

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance;
    
    [System.Serializable]
    public class PoolEntry
    {
        public PoolType poolType;
        public PoolableObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 50;
        public Transform parent;
    }

    public List<PoolEntry> poolPrefabs;

    private Dictionary<PoolType, ObjectPool<PoolableObject>> poolMap = new();
    //private Dictionary<String, ObjectPool<PoolableObject>> poolMap = new();
    //private Dictionary<String, PoolableObject> prefabMap = new();

    private void Awake()
    {
        Instance = this;
        
        foreach (var entry in poolPrefabs)
        {
            if (poolMap.ContainsKey(entry.poolType))
                continue;

            ObjectPool<PoolableObject> pool = null;

            pool = new ObjectPool<PoolableObject>(
                () => CreateObject(entry.prefab, pool, entry.parent),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                collectionCheck: false,
                defaultCapacity: entry.defaultCapacity,
                maxSize: entry.maxSize
            );

            poolMap[entry.poolType] = pool;
        }
    }

    private PoolableObject CreateObject(PoolableObject prefab, ObjectPool<PoolableObject> pool, Transform parent)
    {
        var instance = Instantiate(prefab,parent);
        instance.SetPool(pool);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnGetFromPool(PoolableObject obj)
    {
        
    }

    private void OnReleaseToPool(PoolableObject obj)
    {
        
    }

    private void OnDestroyPooledObject(PoolableObject obj)
    {
        Destroy(obj.gameObject);
    }

     /// <summary>
    /// Spawn object using string key
    /// </summary>
    public T Spawn<T>(PoolType poolType, Vector3 pos) where T : PoolableObject
    {
        if (!poolMap.TryGetValue(poolType, out var pool))
        {
            Debug.LogError($"No pool registered with ID: {poolType}");
            return null;
        }

        var obj = pool.Get();
        if (obj == null)
        {
            return null;
        }
        obj.transform.position = pos;
        return obj as T;
    }
}
