using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolableObject : MonoBehaviour
{
    protected IObjectPool<PoolableObject> pool;

    public void SetPool(IObjectPool<PoolableObject> objectPool)
    {
        pool = objectPool;
    }
}
