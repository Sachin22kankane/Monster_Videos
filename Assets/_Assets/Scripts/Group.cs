using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Group : MonoBehaviour
{
    public bool isActive;
    private List<Enemy> enemies = new List<Enemy>();

    public bool spawnningGroup;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            child.TryGetComponent(out Enemy enemy);
            enemies.Add(enemy);
        }
    }

    public void Activate()
    {
        isActive = true;
        if (spawnningGroup)
        {
            StartCoroutine(StartSpawnning());
        }
        else
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].enabled = true;
            }
        }
    }

    IEnumerator StartSpawnning()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SpawnFromGround();
            BloodParticle spawnEffect = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.spawnEffect, enemies[i].transform.position);
            spawnEffect.Play(Vector3.up);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
