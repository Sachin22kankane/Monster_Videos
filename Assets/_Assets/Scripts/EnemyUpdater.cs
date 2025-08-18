using UnityEngine;
using System;

public class EnemyUpdater : MonoBehaviour
{
    public static Action OnEnemyUpdate;

    public void Update()
    {
        OnEnemyUpdate?.Invoke();
    }
}
