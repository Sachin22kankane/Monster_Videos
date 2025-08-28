using System;
using System.Collections;
using UnityEngine;

public class Bullet : PoolableObject
{
    public Transform EnemyTarget;
    public float speed = 20f;
    public float lifetime = 2f;
    private Vector3 direction;
    private Coroutine bulletDeactivateCoroutine;
    [SerializeField] private ParticleSystem bulletTrail;
    public bool isMissile;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float impactRange;
    
    private void Update()
    {
        if (EnemyTarget != null)
        {
            direction = (EnemyTarget.position + new Vector3(0,0.75f,0) - transform.position).normalized;
        }
        transform.position += direction * speed * Time.deltaTime;
        //transform.forward = direction;
    }

    public void Launch(Transform _newTarget)
    {
        EnemyTarget = _newTarget;
        gameObject.SetActive(true);
        bulletTrail.Play();
        bulletDeactivateCoroutine = StartCoroutine(DisableAfterSomeTime());
    }
    
    public void Launch(Vector3 target)
    {
        direction = (target + new Vector3(0,0.75f,0) - transform.position).normalized;
        gameObject.SetActive(true);
        bulletTrail.Play();
        bulletDeactivateCoroutine = StartCoroutine(DisableAfterSomeTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (isMissile)
            {
                Collider[] enemies = Physics.OverlapSphere(transform.position, impactRange, enemyLayerMask);
                foreach (Collider e in enemies)
                {
                    Enemy temp = e.GetComponent<Enemy>();
                    if (temp != null) // only if Enemy component is present
                    {
                        temp.Damage();
                    }
                }
                CameraShake.instance.Shake(2,1.5f,0.5f);
                Vector3 spawnPos = transform.position;
                var explosion = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.missileExplosion,spawnPos);
                explosion.Play(Vector3.zero);
            }
            else
            {
                enemy.Damage();
            }
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
        
        if (other.TryGetComponent(out Barrel barrel))
        {
            barrel.Explode();
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
