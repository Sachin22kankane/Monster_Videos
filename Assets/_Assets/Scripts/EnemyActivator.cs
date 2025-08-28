using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyActivator : MonoBehaviour
{
    public static EnemyActivator Instance;
    public List<Group> groups;
    [SerializeField] float minimumDistanceToActivate;
    [SerializeField] private Transform player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(CheckForActivateGroup());
    }

    public void EnableEnemy()
    {
        foreach (Group group in groups)
        {
            group.gameObject.SetActive(true);
            group.Activate();
        }
    }

    IEnumerator CheckForActivateGroup()
    {
        while (true)
        {
            foreach (Group group in groups)
            {
                if (group.isActive == true)
                {
                    continue;
                }
                if (Vector3.Distance(player.position, group.transform.position) < minimumDistanceToActivate)
                {
                    group.Activate();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
