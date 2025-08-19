using System;
using System.Collections;
using UnityEngine;

public class Bullet : PoolableObject
{
    public Enemy EnemyTarget;
    public float speed = 20f;
    public float lifetime = 2f;
    private Vector3 direction;
    private Coroutine bulletDeactivateCoroutine;
    [SerializeField] private ParticleSystem bulletTrail;
    
    private void Update()
    {
        if (EnemyTarget != null && !EnemyTarget.isDead)
        {
            direction = (EnemyTarget.transform.position + new Vector3(0,0.5f,0) - transform.position).normalized;
        }
        transform.position += direction * speed * Time.deltaTime;
        //transform.forward = direction;
    }

    public void Launch(Enemy _newTarget)
    {
        EnemyTarget = _newTarget;
        gameObject.SetActive(true);
        bulletTrail.Play();
        bulletDeactivateCoroutine = StartCoroutine(DisableAfterSomeTime());
    }
    
    public void Launch(Vector3 target)
    {
        direction = (target + new Vector3(0,0.5f,0) - transform.position).normalized;
        gameObject.SetActive(true);
        bulletTrail.Play();
        bulletDeactivateCoroutine = StartCoroutine(DisableAfterSomeTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Damage();
            EnemyTarget = null;
            bulletTrail.Stop();
            if (bulletDeactivateCoroutine != null)
            {
                StopCoroutine(bulletDeactivateCoroutine);
                bulletDeactivateCoroutine = null;
            }
            pool?.Release(this);
            gameObject.SetActive(false);
        }
    }

    IEnumerator DisableAfterSomeTime()
    {
        yield return new WaitForSeconds(lifetime);
        EnemyTarget = null;
        bulletTrail.Stop();
        bulletDeactivateCoroutine = null;
        pool?.Release(this);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (bulletDeactivateCoroutine != null)
        {
            StopCoroutine(bulletDeactivateCoroutine);
            bulletDeactivateCoroutine = null;
        }
    }
}
