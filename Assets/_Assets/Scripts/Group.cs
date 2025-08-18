using System;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    public bool isActive;
    private List<Enemy> enemies = new List<Enemy>();

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
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].enabled = true;
        }
    }
}
