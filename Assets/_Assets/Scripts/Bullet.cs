using System;
using System.Collections;
using UnityEngine;

public class Bullet : PoolableObject
{
    public Enemy EnemyTarget;
    public float speed = 20f;
    private Vector3 direction;
    private Coroutine bulletDeactivateCoroutine;
    [SerializeField] private ParticleSystem bulletTrail;
    
    private void Update()
    {
        if (EnemyTarget != null && !EnemyTarget.isDead)
        {
            direction = (EnemyTarget.transform.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward;
        }
        
        transform.position += direction * speed * Time.deltaTime;
        transform.forward = direction;
    }

    public void Launch(Enemy _newTarget)
    {
        EnemyTarget = _newTarget;
        gameObject.SetActive(true);
        bulletTrail.Play();
        bulletDeactivateCoroutine = StartCoroutine(DisableAfterSomeTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Dead();
            EnemyTarget = null;
            bulletTrail.Stop();
            gameObject.SetActive(false);
            pool?.Release(this);
            if (bulletDeactivateCoroutine != null)
            {
                StopCoroutine(bulletDeactivateCoroutine);
            }
        }
    }

    IEnumerator DisableAfterSomeTime()
    {
        yield return new WaitForSeconds(2f);
        EnemyTarget = null;
        bulletDeactivateCoroutine = null;
        bulletTrail.Stop();
        gameObject.SetActive(false);
        pool?.Release(this);
    }
}
