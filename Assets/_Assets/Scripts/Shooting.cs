using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations.Rigging;

public class Shooting : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint[] rightHandConstraint,leftHandConstraint;
    [SerializeField] private Transform rightHandTarget, leftHandTarget;
    [SerializeField] private float aimRange = 15f;
    [SerializeField] private LayerMask enemyLayerMask;
    private float currentWeight;
    private Transform rightTarget, leftTarget;
    [SerializeField] private Transform rhBulletPosRef, lhBulletPosRef;
    [SerializeField] private ParticleSystem rightMuzzle, leftMuzzle;
    [SerializeField] private float rightOffset, leftOffset;

    private void Update()
    {
        DetectRightSide();
        DetectLeftSide();
        //DetectEnemies();
        HandelAim();
        CheckForShoot();
    }

    void DetectRightSide()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + transform.right * rightOffset, aimRange, enemyLayerMask);
        rightTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (rightTarget != null)
        {
            rightHandTarget.position = Vector3.Lerp(rightHandTarget.position, rightTarget.position, Time.deltaTime * 10);
        }
    }
    
    void DetectLeftSide()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + transform.right * leftOffset, aimRange, enemyLayerMask);
        leftTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (leftTarget != null)
        {
            leftHandTarget.position = Vector3.Lerp(leftHandTarget.position, leftTarget.position, Time.deltaTime * 10);
        }
    }



    /*void DetectEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aimRange, enemyLayerMask);
         targetsInRange = enemies
            .Select(c => c.transform)
             .OrderBy(t => Vector3.Distance(transform.position, t.position))
             .ToList();
        oneInRange = enemies.Length > 0;
        moreThanOneInRange = enemies.Length > 1;
        if (oneInRange)
        {
            rightHandTarget.position = Vector3.Lerp(rightHandTarget.position,targetsInRange[0].position, Time.deltaTime * 10);
            if (moreThanOneInRange)
            {
                leftHandTarget.position = Vector3.Lerp(leftHandTarget.position,targetsInRange[1].position, Time.deltaTime * 10);
            }
        }
    }*/

    public float fireDelay = 0.25f;
    private float elaspedTime;
    void CheckForShoot()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime >= fireDelay)
        {
            elaspedTime = 0;
            Shoot();
        }
    }
    
    void Shoot()
    {
        if (rightTarget != null)
        {
            Bullet bullet1 = ObjectPooling.Instance.Spawn<Bullet>(rhBulletPosRef.position);
            bullet1.Launch(rightTarget.GetComponent<Enemy>());
            rightMuzzle.Play();
        }
        if (leftTarget != null)
        {
            Bullet bullet2 = ObjectPooling.Instance.Spawn<Bullet>(lhBulletPosRef.position);
            bullet2.Launch(leftTarget.GetComponent<Enemy>());
            leftMuzzle.Play();
        }
    }
    

    void HandelAim()
    {
        SetRightHandWeight();
        SetLeftHandWeight();
    }

    void SetRightHandWeight()
    {
        float targetWeight = rightTarget != null ? 1f : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 10f);
        for (int i = 0; i < rightHandConstraint.Length; i++)
        {
            rightHandConstraint[i].weight = currentWeight;
        }
    }
    
    void SetLeftHandWeight()
    {
        float targetWeight = leftHandTarget != null ? 1f : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 10f);
        for (int i = 0; i < leftHandConstraint.Length; i++)
        {
            leftHandConstraint[i].weight = currentWeight;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + transform.right * rightOffset, aimRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + transform.right *leftOffset, aimRange);
    }
    
}
