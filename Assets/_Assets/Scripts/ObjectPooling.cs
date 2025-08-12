using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance;
    
    [System.Serializable]
    public class PoolEntry
    {
        public PoolableObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 50;
        public Transform parent;
    }

    public List<PoolEntry> poolPrefabs;

    private Dictionary<Type, ObjectPool<PoolableObject>> poolMap = new();
    private Dictionary<Type, PoolableObject> prefabMap = new();

    private void Awake()
    {
        Instance = this;
        
        foreach (var entry in poolPrefabs)
        {
            var type = entry.prefab.GetType();

            if (prefabMap.ContainsKey(type))
                continue;

            prefabMap[type] = entry.prefab;

            ObjectPool<PoolableObject> pool = null;

            pool = new ObjectPool<PoolableObject>(
                () => CreateObject(entry.prefab, pool,entry.parent),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                collectionCheck: false,
                defaultCapacity: entry.defaultCapacity,
                maxSize: entry.maxSize
            );

            poolMap[type] = pool;
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

    public T Spawn<T>(Vector3 pos) where T : PoolableObject
    {
        var type = typeof(T);
        if (!poolMap.TryGetValue(type, out var pool))
        {
            Debug.LogError($"No pool registered for type: {type.Name}");
            return null;
        }

        var obj = pool.Get();
        obj.transform.position = pos;
        return obj as T;
    }
}
